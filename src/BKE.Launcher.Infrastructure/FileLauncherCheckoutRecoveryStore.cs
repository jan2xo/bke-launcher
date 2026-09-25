using System.Text.Json;
using BKE.Launcher.Application;

namespace BKE.Launcher.Infrastructure;

public sealed class FileLauncherCheckoutRecoveryStore : ILauncherCheckoutRecoveryStore
{
    private readonly string _path;
    private readonly string _legacyPath;

    public FileLauncherCheckoutRecoveryStore(string? path = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            var root = LauncherStoragePaths.UserDataRoot();
            _path = Path.Combine(root, "checkout-recovery.json");
            _legacyPath = Path.Combine(root, "checkout-recovery.id");
        }
        else
        {
            _path = Path.GetFullPath(path);
            _legacyPath = _path + ".legacy";
        }
    }

    public LauncherCheckoutRecoveryState? Read()
    {
        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            LauncherCheckoutRecoveryState? state;
            try
            {
                state = JsonSerializer.Deserialize<LauncherCheckoutRecoveryState>(
                    json);
            }
            catch (JsonException error)
            {
                throw new InvalidDataException(
                    "Launcher checkout recovery state is invalid.",
                    error);
            }

            if (state is null)
            {
                throw new InvalidDataException(
                    "Launcher checkout recovery state is empty.");
            }

            Validate(state);
            return state;
        }

        if (!File.Exists(_legacyPath))
        {
            return null;
        }

        var correlationId = File.ReadAllText(_legacyPath).Trim();
        ValidateCorrelation(correlationId);
        return new LauncherCheckoutRecoveryState(
            correlationId,
            null,
            null,
            Array.Empty<string>());
    }

    public void Write(LauncherCheckoutRecoveryState state)
    {
        Validate(state);

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
                JsonSerializer.Serialize(state));
            File.Move(temporaryPath, _path, true);
            if (File.Exists(_legacyPath))
            {
                File.Delete(_legacyPath);
            }
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
        if (File.Exists(_legacyPath))
        {
            File.Delete(_legacyPath);
        }
    }

    private static void Validate(LauncherCheckoutRecoveryState state)
    {
        ValidateCorrelation(state.CorrelationId);

        var hasAnyIntent =
            state.PurchasePlanId is not null ||
            state.PurchaseMode is not null ||
            state.LegalVersionIds is { Count: > 0 };
        if (!hasAnyIntent)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(state.PurchasePlanId) ||
            state.PurchasePlanId.Length > 256 ||
            state.PurchaseMode is not ("SELF" or "GIFT") ||
            state.LegalVersionIds is not { Count: >= 2 and <= 3 } ||
            state.LegalVersionIds.Any(string.IsNullOrWhiteSpace) ||
            state.LegalVersionIds.Any(value => value.Length > 256) ||
            state.LegalVersionIds.Distinct(StringComparer.Ordinal).Count() !=
                state.LegalVersionIds.Count)
        {
            throw new InvalidDataException(
                "Launcher checkout recovery intent is invalid.");
        }
    }

    private static void ValidateCorrelation(string correlationId)
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
