using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int academics = 0;
    public int social = 0;
    public int love = 0;

    public TMP_Text academicText;
    public TMP_Text socialText;
    public TMP_Text loveText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddAcademics(int amount)
    {
        academics += amount;
        UpdateUI();
    }

    public void AddSocial(int amount)
    {
        social += amount;
        UpdateUI();
    }

    public void AddLove(int amount)
    {
        love += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        academicText.text = "Academics: " + academics;
        socialText.text = "Social: " + social;
        loveText.text = "Love: " + love;
    }
}