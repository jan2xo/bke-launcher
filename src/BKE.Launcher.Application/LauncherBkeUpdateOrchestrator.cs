using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherBkeUpdateOrchestrator
{
    private static readonly Regex VersionPartPattern = new(@"^\d+$", RegexOptions.Compiled);
    private static readonly Regex Sha256Pattern = new(@"^[0-9a-fA-F]{64}$", RegexOptions.Compiled);

    private readonly HttpClient _httpClient;
    private readonly string _stagingDirectory;

    public LauncherBkeUpdateOrchestrator(
        HttpClient? httpClient = null,
        string? stagingDirectory = null)
    {
        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        _stagingDirectory = stagingDirectory ?? DefaultStagingDirectory();
    }

    private static string DefaultStagingDirectory()
    {
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(local, "BKE Digital Solutions", "Launcher", "updates", "staging");
    }

    public string StagingDirectory => _stagingDirectory;

    public static int CompareVersions(string v1, string v2)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(v1);
        ArgumentException.ThrowIfNullOrWhiteSpace(v2);

        // Strip leading 'v' if present
        var clean1 = v1.Trim().TrimStart('v', 'V');
        var clean2 = v2.Trim().TrimStart('v', 'V');

        // Split into main version and prerelease
        var parts1 = clean1.Split('-', 2);
        var parts2 = clean2.Split('-', 2);

        var numParts1 = parts1[0].Split('.');
        var numParts2 = parts2[0].Split('.');

        var maxLen = Math.Max(numParts1.Length, numParts2.Length);
        for (var i = 0; i < maxLen; i++)
        {
            var p1 = i < numParts1.Length ? numParts1[i] : "0";
            var p2 = i < numParts2.Length ? numParts2[i] : "0";

            var n1 = int.TryParse(p1, out var val1) ? val1 : 0;
            var n2 = int.TryParse(p2, out var val2) ? val2 : 0;

            if (n1 > n2) return 1;
            if (n1 < n2) return -1;
        }

        // If numeric parts are equal, check prerelease
        var hasPre1 = parts1.Length > 1;
        var hasPre2 = parts2.Length > 1;

        if (!hasPre1 && hasPre2) return 1; // Release is newer than prerelease
        if (hasPre1 && !hasPre2) return -1;
        if (hasPre1 && hasPre2)
        {
            return string.CompareOrdinal(parts1[1], parts2[1]);
        }

        return 0;
    }

    public static bool IsUpdateApplicable(string currentVersion, string targetVersion) =>
        CompareVersions(targetVersion, currentVersion) > 0;

    public async Task<BkeUpdateCheckResult> CheckForUpdateAsync(
        string currentVersion,
        string? manifestUrlOrPath = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentVersion);

        try
        {
            var manifestSource = manifestUrlOrPath
                ?? Environment.GetEnvironmentVariable("BKE_UPDATE_MANIFEST_URL")
                ?? Environment.GetEnvironmentVariable("BKE_UPDATE_RELEASE_JSON");

            if (string.IsNullOrWhiteSpace(manifestSource))
            {
                // In preproduction/local testing without external URL, check eng/preproduction-parent-release.json
                var localManifest = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "eng", "preproduction-parent-release.json");
                if (!File.Exists(localManifest))
                {
                    localManifest = Path.Combine(Directory.GetCurrentDirectory(), "eng", "preproduction-parent-release.json");
                }

                if (File.Exists(localManifest))
                {
                    manifestSource = localManifest;
                }
            }

            if (string.IsNullOrWhiteSpace(manifestSource))
            {
                return BkeUpdateCheckResult.UpToDate();
            }

            string jsonContent;
            if (manifestSource.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                manifestSource.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                using var response = await _httpClient.GetAsync(manifestSource, cancellationToken);
                response.EnsureSuccessStatusCode();
                jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            else if (File.Exists(manifestSource))
            {
                jsonContent = await File.ReadAllTextAsync(manifestSource, cancellationToken);
            }
            else
            {
                return BkeUpdateCheckResult.UpToDate();
            }

            using var doc = JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;

            // Support either bke.preproduction-release-request.v2 or bke.parent-package-boundary.v2 or generic update release
            string? targetVer = null;
            string? sha256 = null;
            long sizeBytes = 0;
            string? fileName = null;
            string? downloadUrl = null;

            if (root.TryGetProperty("target_version", out var tvProp))
            {
                targetVer = tvProp.GetString();
            }
            else if (root.TryGetProperty("bke_version", out var bvProp))
            {
                targetVer = bvProp.GetString();
            }
            else if (root.TryGetProperty("tag", out var tagProp))
            {
                var match = Regex.Match(tagProp.GetString() ?? "", @"^bke-v?(\d+\.\d+\.\d+)");
                if (match.Success)
                {
                    targetVer = match.Groups[1].Value;
                }
            }

            if (root.TryGetProperty("installer", out var instProp))
            {
                if (instProp.TryGetProperty("file", out var fProp)) fileName = fProp.GetString();
                if (instProp.TryGetProperty("sha256", out var sProp)) sha256 = sProp.GetString();
                if (instProp.TryGetProperty("bytes", out var bProp)) sizeBytes = bProp.GetInt64();
                if (instProp.TryGetProperty("download_url", out var dProp)) downloadUrl = dProp.GetString();
            }

            if (string.IsNullOrWhiteSpace(targetVer))
            {
                return BkeUpdateCheckResult.UpToDate();
            }

            if (!IsUpdateApplicable(currentVersion, targetVer))
            {
                return BkeUpdateCheckResult.UpToDate();
            }

            downloadUrl ??= fileName ?? $"BKE-{targetVer}-PREPRODUCTION-Windows.exe";
            sha256 ??= string.Empty;

            var summary = root.TryGetProperty("release_summary", out var sumProp)
                ? sumProp.GetString() ?? $"BKE {targetVer} update is available."
                : $"BKE {targetVer} update is available.";

            var publishedAt = root.TryGetProperty("published_at", out var pubProp) &&
                              DateTimeOffset.TryParse(pubProp.GetString(), out var dt)
                ? dt
                : DateTimeOffset.UtcNow;

            var channel = root.TryGetProperty("channel", out var chanProp)
                ? chanProp.GetString() ?? "STABLE"
                : "STABLE";

            var release = new BkeUpdateRelease(
                currentVersion,
                targetVer,
                channel,
                downloadUrl,
                sha256,
                sizeBytes,
                summary,
                publishedAt);

            return BkeUpdateCheckResult.Available(release);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return BkeUpdateCheckResult.Failed($"Failed to check for BKE updates: {ex.Message}");
        }
    }

    public async Task<string> StageUpdateAsync(
        BkeUpdateRelease release,
        string? sourceFileOrDirectory = null,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(release);
        if (!IsUpdateApplicable(release.CurrentVersion, release.TargetVersion))
        {
            throw new InvalidOperationException(
                $"Cannot stage downgrade or equal version: current '{release.CurrentVersion}', target '{release.TargetVersion}'.");
        }

        Directory.CreateDirectory(_stagingDirectory);

        var destinationFileName = Path.GetFileName(release.DownloadUrl);
        if (string.IsNullOrWhiteSpace(destinationFileName) || destinationFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            destinationFileName = $"BKE-{release.TargetVersion}-PREPRODUCTION-Windows.exe";
        }

        var stagedFilePath = Path.Combine(_stagingDirectory, destinationFileName);

        // If local source file is provided and exists, use it directly (e.g. testing or local repo artifact)
        if (!string.IsNullOrWhiteSpace(sourceFileOrDirectory) && File.Exists(sourceFileOrDirectory))
        {
            File.Copy(sourceFileOrDirectory, stagedFilePath, overwrite: true);
            progress?.Report(1.0);
        }
        else if (release.DownloadUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                 release.DownloadUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            using var response = await _httpClient.GetAsync(release.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? release.SizeBytes;
            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = new FileStream(stagedFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

            var buffer = new byte[81920];
            long totalRead = 0;
            int read;
            while ((read = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                totalRead += read;
                if (totalBytes > 0 && progress != null)
                {
                    progress.Report((double)totalRead / totalBytes);
                }
            }
        }
        else
        {
            // Local file resolution
            var sourceCandidate = release.DownloadUrl;
            if (!File.Exists(sourceCandidate) && !string.IsNullOrWhiteSpace(sourceFileOrDirectory))
            {
                sourceCandidate = File.Exists(sourceFileOrDirectory)
                    ? sourceFileOrDirectory
                    : Path.Combine(sourceFileOrDirectory, destinationFileName);
            }

            if (!File.Exists(sourceCandidate))
            {
                throw new FileNotFoundException($"Source installer package not found at '{sourceCandidate}'.");
            }

            File.Copy(sourceCandidate, stagedFilePath, overwrite: true);
            progress?.Report(1.0);
        }

        // Verify staged artifact hash
        if (!string.IsNullOrWhiteSpace(release.Sha256))
        {
            var verified = VerifyPackageHash(stagedFilePath, release.Sha256);
            if (!verified)
            {
                // Purge corrupted package immediately to fail closed
                if (File.Exists(stagedFilePath))
                {
                    File.Delete(stagedFilePath);
                }
                throw new InvalidDataException(
                    $"Installer package SHA-256 verification failed. File '{stagedFilePath}' does not match expected hash '{release.Sha256}'.");
            }
        }

        return stagedFilePath;
    }

    public static bool VerifyPackageHash(string filePath, string expectedSha256)
    {
        if (!File.Exists(filePath)) return false;
        if (string.IsNullOrWhiteSpace(expectedSha256)) return false;

        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hashBytes = sha256.ComputeHash(stream);
        var actualHash = Convert.ToHexStringLower(hashBytes);

        return string.Equals(actualHash, expectedSha256.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public static Process? LaunchSilentInstaller(string stagedInstallerPath)
    {
        if (!File.Exists(stagedInstallerPath))
        {
            throw new FileNotFoundException($"Staged installer not found: {stagedInstallerPath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = stagedInstallerPath,
            Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART",
            UseShellExecute = true,
            Verb = "runas"
        };

        return Process.Start(startInfo);
    }

    public void CleanStaging()
    {
        try
        {
            if (Directory.Exists(_stagingDirectory))
            {
                Directory.Delete(_stagingDirectory, recursive: true);
            }
        }
        catch
        {
            // Best effort cleanup
        }
    }
}
