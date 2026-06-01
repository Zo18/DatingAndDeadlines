using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager_Scene4 : MonoBehaviour
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
    public Image characterLeft;        // Sarah
    public Image characterRight;       // Lecturer, then Dean or Greg

    [Header("Character Sprites")]
    public Sprite lecturerSprite;
    public Sprite deanSprite;
    public Sprite gregSprite;

    [Header("Slide Animation")]
    public float slideStartOffset = 800f;
    public float slideDuration = 0.5f;

    [Header("Fade Out")]
    public Image fadeOverlay;
    public float fadeDuration = 1.5f;

    [Header("Name and Dialogue Box")]
    public RectTransform nameBox;
    public RectTransform dialogueBox;

    [Header("Settings")]
    public float textSpeed = 0.03f;

    [Header("Next Scene")]
    public string nextSceneName = "";

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

    // === Line indices for the branches ===
    // Common opening: 0-4
    // Sit choice: 5
    // Dean branch (front row): 6-13
    // Greg branch (back row): 14-21
    // Converge - lecturer returns: 22-25
    // Fade out: 26
    private string[] lines = {
        // ---- COMMON OPENING (0-4) ----
        "LECTURER_ENTER",                                                          // 0
        "Settle down... settle down students. Welcome everyone. It's great to see so many new faces.", // 1
        "LECTURER_EXIT",                                                           // 2
        "He seems cool.",                                                          // 3
        "CHOICE_SEAT",                                                             // 4 - branches here

        // ---- DEAN BRANCH (front row) (5-13) ----
        "DEAN_ENTER",                                                              // 5
        "You can share with me if you didn't get the reading list yet. It's... a lot. My name's Dean.", // 6
        "Thanks, I'm Sarah. You actually did the pre-reading?",                    // 7
        "Yea.. I did.",                                                            // 8
        "CHOICE_DEAN",                                                             // 9
        "DEAN_EXIT",                                                               // 10
        "GOTO_LECTURER",                                                           // 11 - jumps to line 21

        // ---- GREG BRANCH (back row) (12-19) ----
        "GREG_ENTER",                                                              // 12
        "Smart move. Back here, the lecturer can't see your face when you fall asleep.", // 13
        "You've clearly thought about this.",                                      // 14
        "I'm a strategist. Greg by the way.",                                      // 15
        "CHOICE_GREG",                                                             // 16
        "GREG_EXIT",                                                               // 17
        "GOTO_LECTURER",                                                           // 18 - jumps to line 21

        // padding (not used)
        "",                                                                        // 19
        "",                                                                        // 20

        // ---- LECTURER RETURNS (21-24) ----
        "LECTURER_ENTER",                                                          // 21
        "Your first assignment is due Friday. Yes, this Friday.",                  // 22
        "(Groans across the room.)",                                               // 23
        "FADE_OUT"                                                                 // 24
    };

    private string[] speakers = {
        "",                   // 0 LECTURER_ENTER
        "Lecturer",           // 1
        "",                   // 2 LECTURER_EXIT
        "Sarah (Thinking)",   // 3
        "",                   // 4 CHOICE_SEAT

        "",                   // 5 DEAN_ENTER
        "Dean",               // 6
        "Sarah",              // 7
        "Dean",               // 8
        "",                   // 9 CHOICE_DEAN
        "",                   // 10 DEAN_EXIT
        "",                   // 11 GOTO_LECTURER

        "",                   // 12 GREG_ENTER
        "Greg",               // 13
        "Sarah",              // 14
        "Greg",               // 15
        "",                   // 16 CHOICE_GREG
        "",                   // 17 GREG_EXIT
        "",                   // 18 GOTO_LECTURER

        "",                   // 19
        "",                   // 20

        "",                   // 21 LECTURER_ENTER
        "Lecturer",           // 22
        "",                   // 23
        ""                    // 24 FADE_OUT
    };

    void Start()
    {
        Debug.Log("DialogueManager_Scene4 Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);

        if (characterRight != null)
            characterRightHome = characterRight.rectTransform.anchoredPosition;

        if (characterRight != null) characterRight.gameObject.SetActive(false);

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

        if (line == "CHOICE_SEAT") { ShowSeatChoice(); return; }
        if (line == "CHOICE_DEAN") { ShowDeanChoice(); return; }
        if (line == "CHOICE_GREG") { ShowGregChoice(); return; }
        if (line == "FADE_OUT")    { StartCoroutine(FadeOutAndLoadNext()); return; }
        if (line == "GOTO_LECTURER") { currentLine = 21; ShowLine(); return; }

        if (line == "LECTURER_ENTER") { StartCoroutine(CharacterEnter(lecturerSprite)); return; }
        if (line == "DEAN_ENTER")     { StartCoroutine(CharacterEnter(deanSprite));     return; }
        if (line == "GREG_ENTER")     { StartCoroutine(CharacterEnter(gregSprite));     return; }

        if (line == "LECTURER_EXIT" || line == "DEAN_EXIT" || line == "GREG_EXIT")
        {
            StartCoroutine(CharacterExit());
            return;
        }

        // Skip blank padding lines
        if (string.IsNullOrEmpty(line))
        {
            AdvancePastTag();
            return;
        }

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

    IEnumerator CharacterEnter(Sprite sprite)
    {
        isSliding = true;

        if (characterRight != null)
        {
            characterRight.sprite = sprite;
            characterRight.gameObject.SetActive(true);
            characterRight.rectTransform.anchoredPosition =
                characterRightHome + new Vector2(slideStartOffset, 0);

            Vector2 start = characterRight.rectTransform.anchoredPosition;
            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / slideDuration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                characterRight.rectTransform.anchoredPosition = Vector2.Lerp(start, characterRightHome, eased);
                yield return null;
            }
            characterRight.rectTransform.anchoredPosition = characterRightHome;
        }

        isSliding = false;
        AdvancePastTag();
    }

    IEnumerator CharacterExit()
    {
        isSliding = true;

        if (characterRight != null && characterRight.gameObject.activeSelf)
        {
            Vector2 start = characterRight.rectTransform.anchoredPosition;
            Vector2 target = characterRightHome + new Vector2(slideStartOffset, 0);
            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / slideDuration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                characterRight.rectTransform.anchoredPosition = Vector2.Lerp(start, target, eased);
                yield return null;
            }
            characterRight.gameObject.SetActive(false);
        }

        isSliding = false;
        AdvancePastTag();
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
    void ShowSeatChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "Where do you sit?";
        choice1Text.text = "Front row";
        choice2Text.text = "Back row";
        choice3Text.text = "";  // unused for this choice
    }

    void ShowDeanChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you respond to Dean?";
        choice1Text.text = "Honestly, that's kind of impressive.";
        choice2Text.text = "Wow. You're THAT guy.";
        choice3Text.text = "";
    }

    void ShowGregChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you respond to Greg?";
        choice1Text.text = "I'm Sarah. And I actually want to pass.";
        choice2Text.text = "Haha, I'm Sarah, and that sounds like a good strategy to me.";
        choice3Text.text = "";
    }

    // ============================================================
    // CHOICE BUTTON HANDLERS
    // ============================================================
    public void OnChoice1Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_SEAT")
        {
            // Front row -> Dean branch
            if (StatsManager.Instance != null) StatsManager.Instance.ModifyAcademics(1);
            choicePanel.SetActive(false);
            waitingForChoice = false;
            currentLine = 5;  // start of Dean branch
            ShowLine();
        }
        else if (tag == "CHOICE_DEAN")
        {
            if (StatsManager.Instance != null) StatsManager.Instance.ModifyLove(1);
            AfterChoice();
        }
        else if (tag == "CHOICE_GREG")
        {
            if (StatsManager.Instance != null) StatsManager.Instance.ModifyAcademics(1);
            AfterChoice();
        }
    }

    public void OnChoice2Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_SEAT")
        {
            // Back row -> Greg branch
            if (StatsManager.Instance != null) StatsManager.Instance.ModifySocial(1);
            choicePanel.SetActive(false);
            waitingForChoice = false;
            currentLine = 12;  // start of Greg branch
            ShowLine();
        }
        else if (tag == "CHOICE_DEAN")
        {
            if (StatsManager.Instance != null) StatsManager.Instance.ModifySocial(1);
            AfterChoice();
        }
        else if (tag == "CHOICE_GREG")
        {
            if (StatsManager.Instance != null) StatsManager.Instance.ModifyLove(1);
            AfterChoice();
        }
    }

    public void OnChoice3Selected()
    {
        // Scene 4 only uses choice 1 and 2; choice 3 button can be hidden
        // or just do nothing if pressed
    }

    void AfterChoice()
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        currentLine++;
        if (currentLine < lines.Length)
            ShowLine();
    }

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

    public int GetCurrentLine()
    {
        return currentLine;
    }
}