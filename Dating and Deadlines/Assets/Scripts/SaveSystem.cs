using UnityEngine;

/// <summary>
/// Simple save/load system using PlayerPrefs.
/// Stores which scene was active, which dialogue line the player was on,
/// and their current stats so they can resume later.
/// 
/// This is static (no MonoBehaviour) so you can call it from anywhere:
///     SaveSystem.Save("Scene6", currentLine);
///     SaveSystem.Load();
/// </summary>
public static class SaveSystem
{
    private const string KEY_HAS_SAVE   = "HasSave";
    private const string KEY_SCENE      = "SavedScene";
    private const string KEY_LINE       = "SavedLine";
    private const string KEY_ACADEMICS  = "SavedAcademics";
    private const string KEY_SOCIAL     = "SavedSocial";
    private const string KEY_LOVE       = "SavedLove";

    /// <summary>
    /// Save the current scene name, dialogue line, and stats.
    /// </summary>
    public static void Save(string sceneName, int currentLine)
    {
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.SetString(KEY_SCENE, sceneName);
        PlayerPrefs.SetInt(KEY_LINE, currentLine);

        if (StatsManager.Instance != null)
        {
            PlayerPrefs.SetInt(KEY_ACADEMICS, StatsManager.Instance.academics);
            PlayerPrefs.SetInt(KEY_SOCIAL,    StatsManager.Instance.social);
            PlayerPrefs.SetInt(KEY_LOVE,      StatsManager.Instance.love);
        }

        PlayerPrefs.Save();
        Debug.Log("Game saved.");
    }

    /// <summary>
    /// Returns true if there is a saved game to continue from.
    /// </summary>
    public static bool HasSave()
    {
        return PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1;
    }

    public static string GetSavedScene() => PlayerPrefs.GetString(KEY_SCENE, "");
    public static int GetSavedLine()     => PlayerPrefs.GetInt(KEY_LINE, 0);
    public static int GetSavedAcademics() => PlayerPrefs.GetInt(KEY_ACADEMICS, 0);
    public static int GetSavedSocial()    => PlayerPrefs.GetInt(KEY_SOCIAL, 0);
    public static int GetSavedLove()      => PlayerPrefs.GetInt(KEY_LOVE, 0);

    /// <summary>
    /// Clears the save (used when starting a new game).
    /// </summary>
    public static void ClearSave()
    {
        PlayerPrefs.DeleteKey(KEY_HAS_SAVE);
        PlayerPrefs.DeleteKey(KEY_SCENE);
        PlayerPrefs.DeleteKey(KEY_LINE);
        PlayerPrefs.DeleteKey(KEY_ACADEMICS);
        PlayerPrefs.DeleteKey(KEY_SOCIAL);
        PlayerPrefs.DeleteKey(KEY_LOVE);
        PlayerPrefs.Save();
    }
}