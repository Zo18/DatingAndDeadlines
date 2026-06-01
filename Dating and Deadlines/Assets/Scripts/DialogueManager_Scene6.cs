using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager_Scene6 : MonoBehaviour
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
    public Image characterLeft;
    public Image characterRight;
    public Image characterRight2;

    [Header("Character Sprites")]
    public Sprite chloeSprite;
    public Sprite mayaSprite;
    public Sprite deanSprite;
    public Sprite gregSprite;

    [Header("Slide Animation")]
    public float slideStartOffset = 800f;
    public float slideDuration = 0.5f;

    [Header("Name and Dialogue Box")]
    public RectTransform nameBox;
    public RectTransform dialogueBox;

    [Header("Settings")]
    public float textSpeed = 0.03f;

    [Header("Stat Thresholds")]
    public int deanThreshold = 3;
    public int gregThreshold = 3;

    [Header("End Of Game")]
    public string endSceneName = "MainMenuScene";

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
    private bool gameEnded = false;

    private Vector2 characterRightHome;
    private Vector2 characterRight2Home;

    private string[] lines = {
        "Music, lights, so many people, this is going to be a good night!",
        "CHLOE_AND_MAYA_ENTER",
        "You made it. I was about to send a search party.",
        "I was the search party.",
        "Okay, big night, our first first-year party!",
        "Yay!",
        "I'm so excited!",
        "CHOICE_ROUTE",

        "CHLOE_AND_MAYA_EXIT",
        "DEAN_ENTER",
        "Ah, I see you came.",
        "Yes, yes I did indeed.",
        "So how have your first few days been?",
        "They've been good, I'm excited to see what this year holds for me.",
        "Yeah, me too.",
        "I didn't really expect to see you here, honestly.",
        "Why's that?",
        "I dunno. You seem like the type who'd be home with a textbook.",
        "CHOICE_DEAN",
        "Okay, fair.",
        "This is gonna sound weird, but I was kind of hoping you'd show up.",
        "Yeah?",
        "Yeah.",
        "Okay. That's... a lot for a Tuesday.",
        "Anyway. We should swap numbers. For class stuff. Or... not just class stuff.",
        "Smooth.",
        "I've been practising.",
        "See you in lecture tomorrow?",
        "See you tomorrow, Dean.",
        "Day two of uni. So far, so good.",
        "END_GAME",

        "CHLOE_AND_MAYA_EXIT",
        "GREG_ENTER",
        "Well, well. Look who showed up.",
        "Were you waiting for me?",
        "Maybe. Don't let it go to your head.",
        "So, surviving first year so far?",
        "Two days in, I'm undefeated.",
        "Bold.",
        "I'm just figuring it out as I go.",
        "Honestly? Same. I just hide it better.",
        "Ohhh, you're so mysterious.",
        "Haha, very funny. But honestly I don't usually do the whole... get-to-know-you thing at parties.",
        "What do you usually do?",
        "Avoid it. You're easy to talk to, though.",
        "CHOICE_GREG",
        "Don't get used to it.",
        "Put your number in my phone. In case you wanna keep talking when there's no bass involved.",
        "Smooth.",
        "I have my moments.",
        "Find me in class on Monday?",
        "Back row?",
        "Obviously.",
        "Deal.",
        "That wasn't what I expected. In a good way.",
        "END_GAME",

        "Honestly? I just wanna hang out with you two tonight.",
        "Aww, babe.",
        "That's the best answer.",
        "Okay but we ARE dancing. Non-negotiable.",
        "I have so many regrettable moves prepared.",
        "What ARE you doing?",
        "It's called expression, Sarah.",
        "It's called being unwell.",
        "Two days ago I didn't know either of them. Now I'm yelling a chorus at the ceiling with them.",
        "Okay. Promise me. Every party. The three of us.",
        "Pinky promise.",
        "Pinky promise.",
        "There's a whole year for everything else. Tonight is just this.",
        "Another song! Go go go!",
        "END_GAME"
    };

    private string[] speakers = {
        "Sarah (Thinking)",
        "",
        "Chloe",
        "Maya",
        "Chloe",
        "Maya",
        "Sarah",
        "",
        "",
        "",
        "Dean",
        "Sarah",
        "Dean",
        "Sarah",
        "Dean",
        "Dean",
        "Sarah",
        "Dean",
        "",
        "Dean",
        "Dean",
        "Sarah",
        "Dean",
        "Sarah (Thinking)",
        "Dean",
        "Sarah",
        "Dean",
        "Dean",
        "Sarah",
        "Sarah (Thinking)",
        "",
        "",
        "",
        "Greg",
        "Sarah",
        "Greg",
        "Greg",
        "Sarah",
        "Greg",
        "Sarah",
        "Greg",
        "Sarah",
        "Greg",
        "Sarah",
        "Greg",
        "",
        "Greg",
        "Greg",
        "Sarah",
        "Greg",
        "Greg",
        "Sarah",
        "Greg",
        "Sarah",
        "Sarah (Thinking)",
        "",
        "Sarah",
        "Chloe",
        "Maya",
        "Chloe",
        "Maya",
        "Sarah",
        "Maya",
        "Chloe",
        "Sarah (Thinking)",
        "Chloe",
        "Maya",
        "Sarah",
        "Sarah (Thinking)",
        "Chloe",
        ""
    };

    void Start()
    {
        Debug.Log("DialogueManager_Scene6 Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);

        if (characterRight != null)
            characterRightHome = characterRight.rectTransform.anchoredPosition;
        if (characterRight2 != null)
            characterRight2Home = characterRight2.rectTransform.anchoredPosition;

        if (characterRight != null) characterRight.gameObject.SetActive(false);
        if (characterRight2 != null) characterRight2.gameObject.SetActive(false);

        ShowLine();
    }

    void Update()
    {
        if (waitingForChoice || isSliding || gameEnded) return;

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

        if (line == "CHOICE_ROUTE")    { ShowRouteChoice(); return; }
        if (line == "CHOICE_DEAN")     { ShowDeanChoice(); return; }
        if (line == "CHOICE_GREG")     { ShowGregChoice(); return; }
        if (line == "END_GAME")        { EndGame(); return; }

        if (line == "CHLOE_AND_MAYA_ENTER") { StartCoroutine(ChloeAndMayaEnter()); return; }
        if (line == "CHLOE_AND_MAYA_EXIT")  { StartCoroutine(ChloeAndMayaExit()); return; }
        if (line == "DEAN_ENTER")           { StartCoroutine(SingleCharacterEnter(deanSprite)); return; }
        if (line == "GREG_ENTER")           { StartCoroutine(SingleCharacterEnter(gregSprite)); return; }

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
        else
        {
            EndGame();
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
    // CHARACTER ANIMATIONS
    // ============================================================
    IEnumerator ChloeAndMayaEnter()
    {
        yield return ChloeAndMayaEnterRoutine();
        AdvancePastTag();
    }

    IEnumerator ChloeAndMayaExit()
    {
        yield return ChloeAndMayaExitRoutine();
        AdvancePastTag();
    }

    IEnumerator SingleCharacterEnter(Sprite spriteToShow)
    {
        yield return SingleCharacterEnterRoutine(spriteToShow);
        AdvancePastTag();
    }

    IEnumerator ChloeAndMayaEnterRoutine()
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
    }

    IEnumerator ChloeAndMayaExitRoutine()
    {
        isSliding = true;

        Vector2 offscreenRight  = characterRightHome  + new Vector2(slideStartOffset, 0);
        Vector2 offscreenRight2 = characterRight2Home + new Vector2(slideStartOffset, 0);

        yield return SlideBothTo(offscreenRight, offscreenRight2);

        if (characterRight != null) characterRight.gameObject.SetActive(false);
        if (characterRight2 != null) characterRight2.gameObject.SetActive(false);

        isSliding = false;
    }

    IEnumerator SingleCharacterEnterRoutine(Sprite spriteToShow)
    {
        isSliding = true;

        if (characterRight2 != null) characterRight2.gameObject.SetActive(false);

        if (characterRight != null)
        {
            characterRight.sprite = spriteToShow;
            characterRight.gameObject.SetActive(true);
            characterRight.rectTransform.anchoredPosition =
                characterRightHome + new Vector2(slideStartOffset, 0);
        }

        yield return SlideOneTo(characterRight, characterRightHome);
        isSliding = false;
    }

    IEnumerator SingleCharacterExitRoutine()
    {
        isSliding = true;

        Vector2 offscreen = characterRightHome + new Vector2(slideStartOffset, 0);
        yield return SlideOneTo(characterRight, offscreen);

        if (characterRight != null) characterRight.gameObject.SetActive(false);
        isSliding = false;
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

    IEnumerator SlideOneTo(Image character, Vector2 target)
    {
        if (character == null) yield break;

        Vector2 start = character.rectTransform.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            character.rectTransform.anchoredPosition = Vector2.Lerp(start, target, eased);
            yield return null;
        }
        character.rectTransform.anchoredPosition = target;
    }

    void AdvancePastTag()
    {
        if (currentLine < lines.Length - 1)
        {
            currentLine++;
            ShowLine();
        }
        else
        {
            EndGame();
        }
    }

    // ============================================================
    // CHOICES
    // ============================================================
    void ShowRouteChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How will you spend tonight?";
        choice1Text.text = "I'm going to go talk to Dean.";
        choice2Text.text = "I'm going to find Greg.";
        choice3Text.text = "I can't wait to spend the night with you guys!";
    }

    void ShowDeanChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you respond to Dean?";
        choice1Text.text = "Wow. Called out.";
        choice2Text.text = "Maybe I made an exception.";
        choice3Text.text = "Says the guy nursing one drink in the corner.";
    }

    void ShowGregChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you respond to Greg?";
        choice1Text.text = "Is that a compliment?";
        choice2Text.text = "You too, surprisingly.";
        choice3Text.text = "I think that's the nicest thing you've said all night.";
    }

    public void OnChoice1Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_ROUTE")
        {
            if (StatsManager.Instance != null && StatsManager.Instance.academics >= deanThreshold)
                JumpTo(8);
            else
                FailedCheck("Dean");
        }
        else if (tag == "CHOICE_DEAN" || tag == "CHOICE_GREG")
        {
            AfterChoice();
        }
    }

    public void OnChoice2Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_ROUTE")
        {
            if (StatsManager.Instance != null && StatsManager.Instance.social >= gregThreshold)
                JumpTo(31);
            else
                FailedCheck("Greg");
        }
        else if (tag == "CHOICE_DEAN" || tag == "CHOICE_GREG")
        {
            AfterChoice();
        }
    }

    public void OnChoice3Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_ROUTE")
        {
            JumpTo(56);
        }
        else if (tag == "CHOICE_DEAN" || tag == "CHOICE_GREG")
        {
            AfterChoice();
        }
    }

    void AfterChoice()
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        currentLine++;
        if (currentLine < lines.Length)
            ShowLine();
        else
            EndGame();
    }

    void JumpTo(int index)
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        currentLine = index;
        ShowLine();
    }

    // ============================================================
    // FAILED STAT CHECK - proper character swap sequence
    // ============================================================
    void FailedCheck(string characterName)
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        StartCoroutine(FailedCheckSequence(characterName));
    }

    IEnumerator FailedCheckSequence(string characterName)
    {
        Sprite spriteToShow = (characterName == "Dean") ? deanSprite : gregSprite;

        // 1. Chloe and Maya slide out
        yield return ChloeAndMayaExitRoutine();

        // 2. Dean or Greg slides in
        yield return SingleCharacterEnterRoutine(spriteToShow);

        // 3. They type the brush-off line, wait for player click
        nameText.text = characterName;
        SetOtherLayout();
        dialogueText.text = "";
        isTyping = true;
        foreach (char letter in "Hey - Sarah, right? Glad you came. Catch you in class.")
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
        nextArrow.SetActive(true);

        yield return null;
        while (!Mouse.current.leftButton.wasPressedThisFrame)
            yield return null;
        nextArrow.SetActive(false);

        // 4. Dean or Greg slides out
        yield return SingleCharacterExitRoutine();

        // 5. Chloe and Maya slide back in
        yield return ChloeAndMayaEnterRoutine();

        // 6. Roll into the Solo ending (set line directly so we don't kill this coroutine)
        currentLine = 56;
        ShowLine();
    }

    // ============================================================
    // END GAME
    // ============================================================
    void EndGame()
    {
        gameEnded = true;
        dialogueText.text = "";
        nameText.text = "";
        nextArrow.SetActive(false);
        StartCoroutine(LoadEndSceneAfterDelay(2f));
    }

    IEnumerator LoadEndSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!string.IsNullOrEmpty(endSceneName))
            SceneManager.LoadScene(endSceneName);
    }

    public int GetCurrentLine()
    {
        return currentLine;
    }
}