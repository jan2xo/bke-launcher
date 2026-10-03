using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using BKE.Launcher.Presentation;
using Microsoft.Win32;

namespace BKE.Launcher.Desktop;

internal sealed class SystemTelemetrySampler : IDisposable
{
    private readonly bool _gpuAvailable;
    private readonly List<PerformanceCounter> _gpuCounters = [];
    private ulong? _previousIdle;
    private ulong? _previousKernel;
    private ulong? _previousUser;
    private DateTimeOffset _refreshGpuCountersAt = DateTimeOffset.MinValue;

    public SystemTelemetrySampler()
    {
        if (OperatingSystem.IsWindows())
        {
            _gpuAvailable = DetectGpuAdapter();
        }
    }

    public SystemTelemetrySnapshot Sample()
    {
        if (!OperatingSystem.IsWindows())
        {
            return new SystemTelemetrySnapshot(
                0,
                0,
                0,
                0,
                false,
                null,
                TimeSpan.FromMilliseconds(Math.Max(0, Environment.TickCount64)),
                DateTimeOffset.UtcNow);
        }

        return SampleWindows();
    }

    public void Dispose()
    {
        if (OperatingSystem.IsWindows())
        {
            DisposeWindows();
        }
    }

    [SupportedOSPlatform("windows")]
    private SystemTelemetrySnapshot SampleWindows()
    {
        var cpu = SampleCpu();
        var memory = SampleMemory();
        var gpu = SampleGpu();

        return new SystemTelemetrySnapshot(
            cpu,
            memory.Percent,
            memory.UsedGiB,
            memory.TotalGiB,
            _gpuAvailable,
            gpu,
            TimeSpan.FromMilliseconds(Math.Max(0, Environment.TickCount64)),
            DateTimeOffset.UtcNow);
    }

    [SupportedOSPlatform("windows")]
    private void DisposeWindows()
    {
        foreach (var counter in _gpuCounters)
        {
            counter.Dispose();
        }

        _gpuCounters.Clear();
    }

    [SupportedOSPlatform("windows")]
    private double SampleCpu()
    {
        if (!GetSystemTimes(out var idleTime, out var kernelTime, out var userTime))
        {
            return 0;
        }

        var idle = ToUInt64(idleTime);
        var kernel = ToUInt64(kernelTime);
        var user = ToUInt64(userTime);

        if (_previousIdle is not ulong previousIdle ||
            _previousKernel is not ulong previousKernel ||
            _previousUser is not ulong previousUser)
        {
            _previousIdle = idle;
            _previousKernel = kernel;
            _previousUser = user;
            return 0;
        }

        var idleDelta = idle - previousIdle;
        var kernelDelta = kernel - previousKernel;
        var userDelta = user - previousUser;
        var total = kernelDelta + userDelta;

        _previousIdle = idle;
        _previousKernel = kernel;
        _previousUser = user;

        if (total == 0)
        {
            return 0;
        }

        var busy = total > idleDelta
            ? total - idleDelta
            : 0;

        return Math.Clamp(busy * 100d / total, 0, 100);
    }

    [SupportedOSPlatform("windows")]
    private static MemorySample SampleMemory()
    {
        var status = new MemoryStatusEx
        {
            Length = (uint)Marshal.SizeOf<MemoryStatusEx>(),
        };

        if (!GlobalMemoryStatusEx(ref status) || status.TotalPhysical == 0)
        {
            return default;
        }

        var total = status.TotalPhysical;
        var available = Math.Min(status.AvailablePhysical, total);
        var used = total - available;
        const double gib = 1024d * 1024d * 1024d;

        return new MemorySample(
            Math.Clamp(used * 100d / total, 0, 100),
            used / gib,
            total / gib);
    }

    [SupportedOSPlatform("windows")]
    private double? SampleGpu()
    {
        if (!_gpuAvailable)
        {
            return null;
        }

        RefreshGpuCountersIfNeeded();
        if (_gpuCounters.Count == 0)
        {
            return null;
        }

        var total = 0d;
        var sampled = false;

        foreach (var counter in _gpuCounters)
        {
            try
            {
                var value = counter.NextValue();
                if (float.IsFinite(value) && value >= 0)
                {
                    total += value;
                    sampled = true;
                }
            }
            catch (InvalidOperationException)
            {
                // Performance-counter instances can disappear as processes exit.
            }
        }

        return sampled
            ? Math.Clamp(total, 0, 100)
            : null;
    }

    [SupportedOSPlatform("windows")]
    private void RefreshGpuCountersIfNeeded()
    {
        var now = DateTimeOffset.UtcNow;
        if (now < _refreshGpuCountersAt)
        {
            return;
        }

        _refreshGpuCountersAt = now.AddSeconds(20);

        foreach (var counter in _gpuCounters)
        {
            counter.Dispose();
        }
        _gpuCounters.Clear();

        try
        {
            if (!PerformanceCounterCategory.Exists("GPU Engine"))
            {
                return;
            }

            var category = new PerformanceCounterCategory("GPU Engine");
            var instances = category
                .GetInstanceNames()
                .Where(IsUsefulGpuEngine)
                .OrderBy(name => name, StringComparer.Ordinal)
                .Take(64)
                .ToArray();

            foreach (var instance in instances)
            {
                try
                {
                    var counter = new PerformanceCounter(
                        "GPU Engine",
                        "Utilization Percentage",
                        instance,
                        readOnly: true);
                    _ = counter.NextValue();
                    _gpuCounters.Add(counter);
                }
                catch (InvalidOperationException)
                {
                    // Ignore transient instances and keep the remaining counters.
                }
            }
        }
        catch (InvalidOperationException)
        {
            // GPU performance counters are optional; the dashboard remains usable
            // with availability-only GPU state.
        }
    }

    private static bool IsUsefulGpuEngine(string instance) =>
        instance.Contains("engtype_3D", StringComparison.OrdinalIgnoreCase) ||
        instance.Contains("engtype_Compute", StringComparison.OrdinalIgnoreCase) ||
        instance.Contains("engtype_CUDA", StringComparison.OrdinalIgnoreCase);

    [SupportedOSPlatform("windows")]
    private static bool DetectGpuAdapter()
    {
        try
        {
            using var root = Registry.LocalMachine.OpenSubKey(
                @"SYSTEM\CurrentControlSet\Control\Video");
            if (root is null)
            {
                return false;
            }

            foreach (var adapterKeyName in root.GetSubKeyNames())
            {
                using var adapterKey = root.OpenSubKey(adapterKeyName);
                if (adapterKey is null)
                {
                    continue;
                }

                foreach (var childName in adapterKey.GetSubKeyNames())
                {
                    using var child = adapterKey.OpenSubKey(childName);
                    var description = child?.GetValue("Device Description") as string;
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        return true;
                    }
                }
            }
        }
        catch (System.Security.SecurityException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }

        return false;
    }

    private static ulong ToUInt64(FileTime value) =>
        ((ulong)(uint)value.High << 32) | (uint)value.Low;

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemTimes(
        out FileTime idleTime,
        out FileTime kernelTime,
        out FileTime userTime);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(
        ref MemoryStatusEx buffer);

    [StructLayout(LayoutKind.Sequential)]
    private struct FileTime
    {
        public uint Low;
        public uint High;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MemoryStatusEx
    {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhysical;
        public ulong AvailablePhysical;
        public ulong TotalPageFile;
        public ulong AvailablePageFile;
        public ulong TotalVirtual;
        public ulong AvailableVirtual;
        public ulong AvailableExtendedVirtual;
    }

    private readonly record struct MemorySample(
        double Percent,
        double UsedGiB,
        double TotalGiB);
}
