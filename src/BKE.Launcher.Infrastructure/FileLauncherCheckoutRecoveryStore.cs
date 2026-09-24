using BKE.Launcher.Application;

namespace BKE.Launcher.Infrastructure;

public sealed class FileLauncherCheckoutRecoveryStore : ILauncherCheckoutRecoveryStore
{
    private readonly string _path;

    public FileLauncherCheckoutRecoveryStore(string? path = null)
    {
        _path = string.IsNullOrWhiteSpace(path)
            ? Path.Combine(
                LauncherStoragePaths.UserDataRoot(),
                "checkout-recovery.id")
            : Path.GetFullPath(path);
    }

    public string? ReadCorrelationId()
    {
        if (!File.Exists(_path))
        {
            return null;
        }

        var value = File.ReadAllText(_path).Trim();
        Validate(value);
        return value;
    }

    public void WriteCorrelationId(string correlationId)
    {
        Validate(correlationId);

        var directory = Path.GetDirectoryName(_path)
            ?? throw new InvalidOperationException(
                "Checkout recovery directory is unavailable.");
        Directory.CreateDirectory(directory);

        var temporaryPath =
            _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(
                temporaryPath,
                correlationId + Environment.NewLine);
            File.Move(temporaryPath, _path, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    public void Clear()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }

    private static void Validate(string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId) ||
            correlationId.Length != 32 ||
            !Guid.TryParseExact(correlationId, "N", out _))
        {
            throw new InvalidDataException(
                "Launcher checkout recovery correlation is invalid.");
        }
    }
}
