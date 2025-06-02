using System.IO;

public static class SettingsPersistence
{
    private const string LastSettingsFile = ".last_settings";

    public static void SaveLastSettings(string path)
    {
        File.WriteAllText(LastSettingsFile, path);
    }

    public static string LoadLastSettings()
    {
        return File.Exists(LastSettingsFile) ? File.ReadAllText(LastSettingsFile).Trim() : null;
    }
}