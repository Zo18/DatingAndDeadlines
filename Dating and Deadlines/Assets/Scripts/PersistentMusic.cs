using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    public static PersistentMusic Instance;

    void Awake()
    {
        // Singleton: only one music player ever exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // A duplicate tried to spawn (e.g. user went back to main menu).
            // Destroy this duplicate so the original keeps playing.
            Destroy(gameObject);
        }
    }
}