namespace BKE.Launcher.Infrastructure;

public static class LauncherStoragePaths
{
    public static string UserDataRoot()
    {
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(local))
        {
            throw new InvalidOperationException("Local application data directory is unavailable.");
        }

        return Path.Combine(local, "BKE Digital Solutions", "Launcher");
    }
}
