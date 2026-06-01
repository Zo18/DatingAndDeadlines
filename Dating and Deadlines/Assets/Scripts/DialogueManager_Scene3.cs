using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager_Scene3 : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public GameObject nextArrow;

    [Header("Choice Panel")]
    public GameObject choicePanel;
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;
    public TextMeshProUGUI choice3Text;

    [Header("Characters")]
    public Image characterLeft;       // Sarah (always on left)
    public Image characterRight;      // Unused in Scene 3 but kept for layout consistency

    [Header("Fade Out")]
    [Tooltip("A black UI Image covering the whole screen. Starts transparent, fades to opaque at end.")]
    public Image fadeOverlay;
    public float fadeDuration = 1.5f;

    [Header("Name and Dialogue Box")]
    public RectTransform nameBox;
    public RectTransform dialogueBox;

    [Header("Settings")]
    public float textSpeed = 0.03f;

    [Header("Next Scene")]
    [Tooltip("Name of the scene to load after this one ends (e.g. Scene 4 / Lecture).")]
    public string nextSceneName = "";

    // === Layout values (matched to Scene 1 / Scene 6) ===
    private float mc_NB_Left = 5.734863f;
    private float mc_NB_Top = 2.77824f;
    private float mc_NB_Right = 1535.739f;
    private float mc_NB_Bottom = 86.83746f;

    private float mc_DB_Left = 368.3978f;
    private float mc_DB_Top = 24.3934f;
    private float mc_DB_Right = 39.97833f;
    private float mc_DB_Bottom = 24.3934f;

    private float other_NB_Left = 1541.477f;
    private float other_NB_Top = -0.0001487773f;
    private float other_NB_Right = -0.003051758f;
    private float other_NB_Bottom = 89.61584f;

    private float other_DB_Left = 44.18805f;
    private float other_DB_Top = 24.3934f;
    private float other_DB_Right = 364.188f;
    private float other_DB_Bottom = 24.3934f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool waitingForChoice = false;
    private bool isFading = false;

    // === Dialogue lines ===
    private string[] lines = {
        "First box unpacked. A thousand to go.",                       // 0
        "Proud of you, sweetheart. Sleep well. Study hard.",    // 1
        "CHOICE_FEELING",                                              // 2
        "Anyways, let me get some sleep. Big day tomorrow.",           // 3
        "FADE_OUT"                                                     // 4
    };

    private string[] speakers = {
        "Sarah (Thinking)",   // 0
        "Mom (Text)",         // 1
        "",                   // 2 CHOICE_FEELING
        "Sarah (Thinking)",   // 3
        ""                    // 4 FADE_OUT
    };

    void Start()
    {
        Debug.Log("DialogueManager_Scene3 Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);

        // Make sure the fade overlay starts fully transparent
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(true);
        }

        // Right-side character not used in this scene
        if (characterRight != null) characterRight.gameObject.SetActive(false);

        ShowLine();
    }

    void Update()
    {
        if (waitingForChoice || isFading) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = lines[currentLine];
                isTyping = false;
                nextArrow.SetActive(true);
            }
            else
            {
                NextLine();
            }
        }
    }

    void ShowLine()
    {
        string line = lines[currentLine];

        if (line == "CHOICE_FEELING") { ShowFeelingChoice(); return; }
        if (line == "FADE_OUT")       { StartCoroutine(FadeOutAndLoadNext()); return; }

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        nextArrow.SetActive(false);

        string speaker = speakers[currentLine];
        nameText.text = speaker;
        dialogueText.text = "";

        if (speaker == "Sarah" || speaker == "Sarah (Thinking)" ||
            speaker == "Mom (Text)" || speaker == "")
        {
            SetMCLayout();
        }
        else
        {
            SetOtherLayout();
        }

        foreach (char letter in lines[currentLine].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
        nextArrow.SetActive(true);
    }

    void NextLine()
    {
        if (currentLine < lines.Length - 1)
        {
            currentLine++;
            ShowLine();
        }
    }

    void SetMCLayout()
    {
        nameBox.offsetMin = new Vector2(mc_NB_Left, mc_NB_Bottom);
        nameBox.offsetMax = new Vector2(-mc_NB_Right, -mc_NB_Top);
        dialogueBox.offsetMin = new Vector2(mc_DB_Left, mc_DB_Bottom);
        dialogueBox.offsetMax = new Vector2(-mc_DB_Right, -mc_DB_Top);
    }

    void SetOtherLayout()
    {
        nameBox.offsetMin = new Vector2(other_NB_Left, other_NB_Bottom);
        nameBox.offsetMax = new Vector2(-other_NB_Right, -other_NB_Top);
        dialogueBox.offsetMin = new Vector2(other_DB_Left, other_DB_Bottom);
        dialogueBox.offsetMax = new Vector2(-other_DB_Right, -other_DB_Top);
    }

    // ============================================================
    // CHOICE
    // ============================================================
    void ShowFeelingChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How are you feeling?";
        choice1Text.text = "I've got this. Early night, ready for class.";
        choice2Text.text = "Maybe I'll text Chloe about that party rumour.";
        choice3Text.text = "I just hope I make real friends here.";
    }

    // ============================================================
    // CHOICE BUTTON HANDLERS - HOOK THESE UP IN THE INSPECTOR
    // ============================================================
    public void OnChoice1Selected()
    {
        Debug.Log("Scene 3 Choice 1 clicked!");
        Debug.Log("StatsManager.Instance is null? " + (StatsManager.Instance == null));
    
    if (StatsManager.Instance != null)
    {
        Debug.Log("Current academics before adding: " + StatsManager.Instance.academics);
    }
    
    StatsManager.Instance.ModifyAcademics(1);
    AfterChoice();
}

    public void OnChoice2Selected()
    {
        StatsManager.Instance.ModifySocial(1);
        AfterChoice();
    }

    public void OnChoice3Selected()
    {
        StatsManager.Instance.ModifyLove(1);
        AfterChoice();
    }

    void AfterChoice()
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        currentLine++;
        if (currentLine < lines.Length)
            ShowLine();
    }

    // ============================================================
    // FADE OUT
    // ============================================================
    IEnumerator FadeOutAndLoadNext()
    {
        isFading = true;
        nextArrow.SetActive(false);
        nameText.text = "";
        dialogueText.text = "";

        if (fadeOverlay != null)
        {
            float elapsed = 0f;
            Color c = fadeOverlay.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadeOverlay.color = c;
                yield return null;
            }

            c.a = 1f;
            fadeOverlay.color = c;
        }

        // Load the next scene if one is set
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// Used by the PauseMenu to save which line was active.
    /// </summary>
    public int GetCurrentLine()
    {
        return currentLine;
    }

}