using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager_Scene5 : MonoBehaviour
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
    public Image characterLeft;        // Sarah (always on left)
    public Image characterRight;       // Chloe
    public Image characterRight2;      // Maya

    [Header("Character Sprites")]
    public Sprite chloeSprite;
    public Sprite mayaSprite;

    [Header("Slide Animation")]
    public float slideStartOffset = 800f;
    public float slideDuration = 0.5f;

    [Header("Fade Out")]
    [Tooltip("Optional - a black UI Image covering the whole screen for end-of-scene fade.")]
    public Image fadeOverlay;
    public float fadeDuration = 1.5f;

    [Header("Name and Dialogue Box")]
    public RectTransform nameBox;
    public RectTransform dialogueBox;

    [Header("Settings")]
    public float textSpeed = 0.03f;

    [Header("Next Scene")]
    [Tooltip("Name of the scene to load after this one ends. Leave blank to just fade out.")]
    public string nextSceneName = "";

    // === Layout values (matched to other scenes) ===
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
    private bool isSliding = false;
    private bool isFading = false;

    private Vector2 characterRightHome;
    private Vector2 characterRight2Home;

    // === Dialogue lines ===
    private string[] lines = {
        "CHLOE_AND_MAYA_ENTER",                                              // 0
        "Okay, big news. There's a party Friday. Everyone's going.",         // 1
        "The assignment's due Friday.",                                      // 2
        "Which is why you submit early and live a little.",                  // 3
        "CHOICE_PARTY",                                                      // 4
        "Whatever you decide, decide for you. Not for Chloe.",               // 5
        "Rude. Accurate. But rude.",                                         // 6
        "How's the studying going? Remember our deal, books before fun!",    // 7
        "CHOICE_MOM",                                                        // 8
        "FADE_OUT"                                                           // 9
    };

    private string[] speakers = {
        "",                   // 0 CHLOE_AND_MAYA_ENTER
        "Chloe",              // 1
        "Maya",               // 2
        "Chloe",              // 3
        "",                   // 4 CHOICE_PARTY
        "Maya",               // 5
        "Chloe",              // 6
        "Mom (Text)",         // 7
        "",                   // 8 CHOICE_MOM
        ""                    // 9 FADE_OUT
    };

    void Start()
    {
        Debug.Log("DialogueManager_Scene5 Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);

        // Cache home positions
        if (characterRight != null)
            characterRightHome = characterRight.rectTransform.anchoredPosition;
        if (characterRight2 != null)
            characterRight2Home = characterRight2.rectTransform.anchoredPosition;

        // Both characters hidden at start; CHLOE_AND_MAYA_ENTER slides them in
        if (characterRight != null) characterRight.gameObject.SetActive(false);
        if (characterRight2 != null) characterRight2.gameObject.SetActive(false);

        // Fade overlay starts transparent
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(true);
        }

        ShowLine();
    }

    void Update()
    {
        if (waitingForChoice || isSliding || isFading) return;

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

        if (line == "CHOICE_PARTY")          { ShowPartyChoice(); return; }
        if (line == "CHOICE_MOM")            { ShowMomChoice(); return; }
        if (line == "FADE_OUT")              { StartCoroutine(FadeOutAndLoadNext()); return; }
        if (line == "CHLOE_AND_MAYA_ENTER")  { StartCoroutine(ChloeAndMayaEnter()); return; }

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
    // CHARACTER ENTRANCE ANIMATION
    // ============================================================
    IEnumerator ChloeAndMayaEnter()
    {
        isSliding = true;

        if (characterRight != null) characterRight.sprite = chloeSprite;
        if (characterRight2 != null) characterRight2.sprite = mayaSprite;

        if (characterRight != null)
        {
            characterRight.gameObject.SetActive(true);
            characterRight.rectTransform.anchoredPosition =
                characterRightHome + new Vector2(slideStartOffset, 0);
        }
        if (characterRight2 != null)
        {
            characterRight2.gameObject.SetActive(true);
            characterRight2.rectTransform.anchoredPosition =
                characterRight2Home + new Vector2(slideStartOffset, 0);
        }

        yield return SlideBothTo(characterRightHome, characterRight2Home);

        isSliding = false;
        AdvancePastTag();
    }

    IEnumerator SlideBothTo(Vector2 target1, Vector2 target2)
    {
        Vector2 start1 = characterRight  != null ? characterRight.rectTransform.anchoredPosition  : Vector2.zero;
        Vector2 start2 = characterRight2 != null ? characterRight2.rectTransform.anchoredPosition : Vector2.zero;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            if (characterRight != null)
                characterRight.rectTransform.anchoredPosition = Vector2.Lerp(start1, target1, eased);
            if (characterRight2 != null)
                characterRight2.rectTransform.anchoredPosition = Vector2.Lerp(start2, target2, eased);

            yield return null;
        }

        if (characterRight != null) characterRight.rectTransform.anchoredPosition = target1;
        if (characterRight2 != null) characterRight2.rectTransform.anchoredPosition = target2;
    }

    void AdvancePastTag()
    {
        if (currentLine < lines.Length - 1)
        {
            currentLine++;
            ShowLine();
        }
    }

    // ============================================================
    // CHOICES
    // ============================================================
    void ShowPartyChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you respond?";
        choice1Text.text = "I should finish the assignment first.";
        choice2Text.text = "One night won't hurt. I'm in.";
        choice3Text.text = "Will... certain people be there?";
    }

    void ShowMomChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you reply to Mom?";
        choice1Text.text = "All under control, Mom.";
        choice2Text.text = "Define 'fun'.";
        choice3Text.text = "Ignore";
    }

    // ============================================================
    // CHOICE BUTTON HANDLERS - HOOK THESE UP IN THE INSPECTOR
    // ============================================================
    public void OnChoice1Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_PARTY")
            StatsManager.Instance.ModifyAcademics(1);
        else if (tag == "CHOICE_MOM")
            StatsManager.Instance.ModifyAcademics(1);

        AfterChoice();
    }

    public void OnChoice2Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_PARTY")
            StatsManager.Instance.ModifySocial(1);
        else if (tag == "CHOICE_MOM")
            StatsManager.Instance.ModifySocial(1);

        AfterChoice();
    }

    public void OnChoice3Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_PARTY")
            StatsManager.Instance.ModifyLove(1);
        // CHOICE_MOM option 3 ("Ignore") adds nothing

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