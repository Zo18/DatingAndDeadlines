using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance { get; private set; }

    public int Academics { get; private set; }
    public int Social { get; private set; }
    public int Love { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddAcademics(int amount) => Academics += amount;
    public void AddSocial(int amount)    => Social += amount;
    public void AddLove(int amount)      => Love += amount;

    public void ResetStats()
    {
        Academics = 0;
        Social = 0;
        Love = 0;
    }
}