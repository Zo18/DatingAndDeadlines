using UnityEngine;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI academicsText;
    public TextMeshProUGUI socialText;
    public TextMeshProUGUI loveText;

    void Update()
    {
        if (StatsManager.Instance == null) return;

        if (academicsText) academicsText.text = "Academics: " + StatsManager.Instance.academics;
        if (socialText)    socialText.text    = "Social: " + StatsManager.Instance.social;
        if (loveText)      loveText.text      = "Love: " + StatsManager.Instance.love;
    }
}