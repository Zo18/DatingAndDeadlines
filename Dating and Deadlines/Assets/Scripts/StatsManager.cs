using UnityEngine;
using TMPro;
 
/// <summary>
/// Tracks Sarah's stats (Academics, Social, Love) across every scene in the game.
/// Persists between scene loads via DontDestroyOnLoad.
/// 
/// Add this script to a single empty GameObject in your FIRST scene only
/// (e.g. main menu or Scene 1). It will survive all subsequent scene loads.
/// 
/// Usage from any script:
///     StatsManager.Instance.ModifyAcademics(1);    // add to stats
///     int a = StatsManager.Instance.academics;     // read stats
/// </summary>
public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
 
    [Header("Stats")]
    public int academics = 0;
    public int social = 0;
    public int love = 0;
 
    [Header("UI (optional - leave empty in scenes that don't show stats)")]
    public TextMeshProUGUI academicsText;
    public TextMeshProUGUI socialText;
    public TextMeshProUGUI loveText;
 
    void Awake()
    {
        // Singleton pattern: only one StatsManager exists at any time.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // A second StatsManager tried to spawn (e.g. you put one in
            // Scene 6 for testing). Destroy this duplicate so only the
            // original persists.
            Destroy(gameObject);
        }
    }
 
    void Start()
    {
        UpdateUI();
    }
 
    // ============================================================
    // MODIFY STATS — called from dialogue choice handlers
    // ============================================================
    public void ModifyAcademics(int amount)
    {
        academics += amount;
        academics = Mathf.Clamp(academics, 0, 100);
        UpdateUI();
    }
 
    public void ModifySocial(int amount)
    {
        social += amount;
        social = Mathf.Clamp(social, 0, 100);
        UpdateUI();
    }
 
    public void ModifyLove(int amount)
    {
        love += amount;
        love = Mathf.Clamp(love, 0, 100);
        UpdateUI();
    }
 
    // ============================================================
    // RESET — call this from the main menu's "New Game" button
    // so a fresh playthrough doesn't inherit old stats
    // ============================================================
    public void ResetStats()
    {
        academics = 0;
        social = 0;
        love = 0;
        UpdateUI();
    }
 
    // ============================================================
    // UI REFRESH
    // ============================================================
    void UpdateUI()
    {
        if (academicsText != null)
            academicsText.text = "Academics: " + academics;
        if (socialText != null)
            socialText.text = "Social: " + social;
        if (loveText != null)
            loveText.text = "Love: " + love;
    }
}