using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BKE.Launcher.Presentation;

public sealed record SystemTelemetrySnapshot(
    double CpuPercent,
    double MemoryPercent,
    double MemoryUsedGiB,
    double MemoryTotalGiB,
    bool GpuAvailable,
    double? GpuPercent,
    TimeSpan Uptime,
    DateTimeOffset CapturedAt);

public sealed class SystemTelemetryViewModel : INotifyPropertyChanged
{
    private double _cpuPercent;
    private double _memoryPercent;
    private double _memoryUsedGiB;
    private double _memoryTotalGiB;
    private bool _gpuAvailable;
    private double? _gpuPercent;
    private TimeSpan _uptime;
    private DateTimeOffset _capturedAt;

    public event PropertyChangedEventHandler? PropertyChanged;

    public double CpuPercent
    {
        get => _cpuPercent;
        private set => SetField(ref _cpuPercent, value);
    }

    public double MemoryPercent
    {
        get => _memoryPercent;
        private set => SetField(ref _memoryPercent, value);
    }

    public double MemoryUsedGiB
    {
        get => _memoryUsedGiB;
        private set => SetField(ref _memoryUsedGiB, value);
    }

    public double MemoryTotalGiB
    {
        get => _memoryTotalGiB;
        private set => SetField(ref _memoryTotalGiB, value);
    }

    public bool GpuAvailable
    {
        get => _gpuAvailable;
        private set => SetField(ref _gpuAvailable, value);
    }

    public double? GpuPercent
    {
        get => _gpuPercent;
        private set => SetField(ref _gpuPercent, value);
    }

    public TimeSpan Uptime
    {
        get => _uptime;
        private set => SetField(ref _uptime, value);
    }

    public DateTimeOffset CapturedAt
    {
        get => _capturedAt;
        private set => SetField(ref _capturedAt, value);
    }

    public string CpuLabel => $"{CpuPercent:0}%";

    public string MemoryLabel =>
        MemoryTotalGiB > 0
            ? $"{MemoryUsedGiB:0.0} / {MemoryTotalGiB:0.#} GB"
            : "Unavailable";

    public string GpuLabel =>
        !GpuAvailable
            ? "Not detected"
            : GpuPercent is double value
                ? $"{value:0}%"
                : "Available";

    public string UptimeLabel =>
        Uptime.TotalDays >= 1
            ? $"{(int)Uptime.TotalDays}d {Uptime.Hours}h"
            : Uptime.TotalHours >= 1
                ? $"{(int)Uptime.TotalHours}h {Uptime.Minutes}m"
                : $"{Math.Max(0, Uptime.Minutes)}m";

    public string HealthLabel =>
        CpuPercent >= 92 || MemoryPercent >= 92
            ? "SYSTEM BUSY"
            : "SYSTEM HEALTHY";

    public string HealthDetail =>
        CpuPercent >= 92
            ? "CPU load is very high."
            : MemoryPercent >= 92
                ? "Memory pressure is very high."
                : "BKE is keeping an eye on this machine without getting in the way.";

    public void Apply(SystemTelemetrySnapshot snapshot)
    {
        CpuPercent = Clamp(snapshot.CpuPercent);
        MemoryPercent = Clamp(snapshot.MemoryPercent);
        MemoryUsedGiB = Math.Max(0, snapshot.MemoryUsedGiB);
        MemoryTotalGiB = Math.Max(0, snapshot.MemoryTotalGiB);
        GpuAvailable = snapshot.GpuAvailable;
        GpuPercent = snapshot.GpuPercent is double gpu
            ? Clamp(gpu)
            : null;
        Uptime = snapshot.Uptime < TimeSpan.Zero
            ? TimeSpan.Zero
            : snapshot.Uptime;
        CapturedAt = snapshot.CapturedAt;

        Raise(nameof(CpuLabel));
        Raise(nameof(MemoryLabel));
        Raise(nameof(GpuLabel));
        Raise(nameof(UptimeLabel));
        Raise(nameof(HealthLabel));
        Raise(nameof(HealthDetail));
    }

    private static double Clamp(double value) =>
        double.IsFinite(value)
            ? Math.Clamp(value, 0, 100)
            : 0;

    private void SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        Raise(propertyName);
    }

    private void Raise(string? propertyName)
    {
        if (!string.IsNullOrWhiteSpace(propertyName))
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
