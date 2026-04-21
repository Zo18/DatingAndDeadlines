using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Stats")]
    public int academics = 0;
    public int social = 0;
    public int love = 0;

    [Header("UI")]
    public TextMeshProUGUI academicsText;
    public TextMeshProUGUI socialText;
    public TextMeshProUGUI loveText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

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

    void UpdateUI()
    {
        if (academicsText != null)
            academicsText.text = "📚 Academics: " + academics;
        if (socialText != null)
            socialText.text = "👥 Social: " + social;
        if (loveText != null)
            loveText.text = "💕 Love: " + love;
    }
}

