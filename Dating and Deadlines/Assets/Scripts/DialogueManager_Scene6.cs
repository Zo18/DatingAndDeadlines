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
    public Image characterLeft;        // Sarah (always on left)
    public Image characterRight;       // Slot 1 on right (Chloe / Dean / Greg)
    public Image characterRight2;      // Slot 2 on right (Maya, only used with Chloe)

    [Header("Character Sprites")]
    public Sprite chloeSprite;
    public Sprite mayaSprite;
    public Sprite deanSprite;
    public Sprite gregSprite;

    [Header("Slide Animation")]
    [Tooltip("How far off-screen to the right (in pixels) characters start before sliding in.")]
    public float slideStartOffset = 800f;
    [Tooltip("How long the slide animation takes, in seconds.")]
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
    public string endSceneName = "MainMenu";

    // === Layout values (matched to Scene 1) ===
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
    private bool isSliding = false;     // blocks click-to-advance during slide animations

    // Original positions of the right-side characters (captured at Start)
    private Vector2 characterRightHome;
    private Vector2 characterRight2Home;

    // === Dialogue lines with entrance tags ===
    private string[] lines = {
        // ---- OPENING - Sarah alone thinking ----
        "Music, lights, so many people, this is going to be a good night!",   // 0

        // ---- Chloe and Maya slide in ----
        "CHLOE_AND_MAYA_ENTER",                                                // 1
        "You made it. I was about to send a search party.",                    // 2
        "I was the search party.",                                             // 3
        "Okay, big night, our first first-year party!",                        // 4
        "Yay!",                                                                // 5
        "I'm so excited!",                                                     // 6

        // ---- THE ROUTE CHOICE ----
        "CHOICE_ROUTE",                                                        // 7

        // ==== DEAN ENDING ====
        "CHLOE_AND_MAYA_EXIT",                                                 // 8
        "DEAN_ENTER",                                                          // 9
        "Ah, I see you came.",                                                 // 10
        "Yes, yes I did indeed.",                                              // 11
        "So how have your first few days been?",                              // 12
        "They've been good, I'm excited to see what this year holds for me.", // 13
        "Yeah, me too.",                                                       // 14
        "I didn't really expect to see you here, honestly.",                  // 15
        "Why's that?",                                                         // 16
        "I dunno. You seem like the type who'd be home with a textbook.",     // 17
        "CHOICE_DEAN",                                                         // 18
        "Okay, fair.",                                                         // 19
        "This is gonna sound weird, but I was kind of hoping you'd show up.", // 20
        "Yeah?",                                                               // 21
        "Yeah.",                                                               // 22
        "Okay. That's... a lot for a Tuesday.",                               // 23
        "Anyway. We should swap numbers. For class stuff. Or... not just class stuff.", // 24
        "Smooth.",                                                             // 25
        "I've been practising.",                                               // 26
        "See you in lecture tomorrow?",                                        // 27
        "See you tomorrow, Dean.",                                             // 28
        "Day two of uni. So far, so good.",                                    // 29
        "END_GAME",                                                            // 30

        // ==== GREG ENDING ====
        "CHLOE_AND_MAYA_EXIT",                                                 // 31
        "GREG_ENTER",                                                          // 32
        "Well, well. Look who showed up.",                                     // 33
        "Were you waiting for me?",                                            // 34
        "Maybe. Don't let it go to your head.",                                // 35
        "So, surviving first year so far?",                                    // 36
        "Two days in, I'm undefeated.",                                        // 37
        "Bold.",                                                               // 38
        "I'm just figuring it out as I go.",                                   // 39
        "Honestly? Same. I just hide it better.",                              // 40
        "Ohhh, you're so mysterious.",                                         // 41
        "Haha, very funny. But honestly I don't usually do the whole... get-to-know-you thing at parties.", // 42
        "What do you usually do?",                                             // 43
        "Avoid it. You're easy to talk to, though.",                           // 44
        "CHOICE_GREG",                                                         // 45
        "Don't get used to it.",                                               // 46
        "Put your number in my phone. In case you wanna keep talking when there's no bass involved.", // 47
        "Smooth.",                                                             // 48
        "I have my moments.",                                                  // 49
        "Find me in class on Monday?",                                         // 50
        "Back row?",                                                           // 51
        "Obviously.",                                                          // 52
        "Deal.",                                                               // 53
        "That wasn't what I expected. In a good way.",                         // 54
        "END_GAME",                                                            // 55

        // ==== SOLO ENDING - Chloe and Maya already on screen from opening ====
        "Honestly? I just wanna hang out with you two tonight.",               // 56
        "Aww, babe.",                                                          // 57
        "That's the best answer.",                                             // 58
        "Okay but we ARE dancing. Non-negotiable.",                            // 59
        "I have so many regrettable moves prepared.",                          // 60
        "What ARE you doing?",                                                 // 61
        "It's called expression, Sarah.",                                      // 62
        "It's called being unwell.",                                           // 63
        "Two days ago I didn't know either of them. Now I'm yelling a chorus at the ceiling with them.", // 64
        "Okay. Promise me. Every party. The three of us.",                     // 65
        "Pinky promise.",                                                      // 66
        "Pinky promise.",                                                      // 67
        "There's a whole year for everything else. Tonight is just this.",     // 68
        "Another song! Go go go!",                                             // 69
        "END_GAME"                                                             // 70
    };

    private string[] speakers = {
        "Sarah (Thinking)",      // 0
        "",                      // 1 CHLOE_AND_MAYA_ENTER
        "Chloe",                 // 2
        "Maya",                  // 3
        "Chloe",                 // 4
        "Maya",                  // 5
        "Sarah",                 // 6
        "",                      // 7 CHOICE_ROUTE
        "",                      // 8 CHLOE_AND_MAYA_EXIT
        "",                      // 9 DEAN_ENTER
        "Dean",                  // 10
        "Sarah",                 // 11
        "Dean",                  // 12
        "Sarah",                 // 13
        "Dean",                  // 14
        "Dean",                  // 15
        "Sarah",                 // 16
        "Dean",                  // 17
        "",                      // 18 CHOICE_DEAN
        "Dean",                  // 19
        "Dean",                  // 20
        "Sarah",                 // 21
        "Dean",                  // 22
        "Sarah (Thinking)",      // 23
        "Dean",                  // 24
        "Sarah",                 // 25
        "Dean",                  // 26
        "Dean",                  // 27
        "Sarah",                 // 28
        "Sarah (Thinking)",      // 29
        "",                      // 30 END_GAME
        "",                      // 31 CHLOE_AND_MAYA_EXIT
        "",                      // 32 GREG_ENTER
        "Greg",                  // 33
        "Sarah",                 // 34
        "Greg",                  // 35
        "Greg",                  // 36
        "Sarah",                 // 37
        "Greg",                  // 38
        "Sarah",                 // 39
        "Greg",                  // 40
        "Sarah",                 // 41
        "Greg",                  // 42
        "Sarah",                 // 43
        "Greg",                  // 44
        "",                      // 45 CHOICE_GREG
        "Greg",                  // 46
        "Greg",                  // 47
        "Sarah",                 // 48
        "Greg",                  // 49
        "Greg",                  // 50
        "Sarah",                 // 51
        "Greg",                  // 52
        "Sarah",                 // 53
        "Sarah (Thinking)",      // 54
        "",                      // 55 END_GAME
        "Sarah",                 // 56
        "Chloe",                 // 57
        "Maya",                  // 58
        "Chloe",                 // 59
        "Maya",                  // 60
        "Sarah",                 // 61
        "Maya",                  // 62
        "Chloe",                 // 63
        "Sarah (Thinking)",      // 64
        "Chloe",                 // 65
        "Maya",                  // 66
        "Sarah",                 // 67
        "Sarah (Thinking)",      // 68
        "Chloe",                 // 69
        ""                       // 70 END_GAME
    };

    void Start()
    {
        Debug.Log("DialogueManager_Scene6 Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);

        // Cache home positions before hiding (so slide-in knows where to land)
        if (characterRight != null)
            characterRightHome = characterRight.rectTransform.anchoredPosition;
        if (characterRight2 != null)
            characterRight2Home = characterRight2.rectTransform.anchoredPosition;

        // Sarah is alone thinking at the start
        if (characterRight != null) characterRight.gameObject.SetActive(false);
        if (characterRight2 != null) characterRight2.gameObject.SetActive(false);

        ShowLine();
    }

    void Update()
    {
        if (waitingForChoice || isSliding) return;

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
    // CHARACTER ENTRANCE / EXIT ANIMATIONS
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

    IEnumerator ChloeAndMayaExit()
    {
        isSliding = true;

        Vector2 offscreenRight  = characterRightHome  + new Vector2(slideStartOffset, 0);
        Vector2 offscreenRight2 = characterRight2Home + new Vector2(slideStartOffset, 0);

        yield return SlideBothTo(offscreenRight, offscreenRight2);

        if (characterRight != null) characterRight.gameObject.SetActive(false);
        if (characterRight2 != null) characterRight2.gameObject.SetActive(false);

        isSliding = false;
        AdvancePastTag();
    }

    IEnumerator SingleCharacterEnter(Sprite spriteToShow)
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

    // ============================================================
    // CHOICE BUTTON HANDLERS - HOOK THESE UP IN THE INSPECTOR
    // ============================================================
    public void OnChoice1Selected()
    {
        string tag = lines[currentLine];

        if (tag == "CHOICE_ROUTE")
        {
            if (StatsManager.Instance.academics >= deanThreshold)
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
            if (StatsManager.Instance.social >= gregThreshold)
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

    void FailedCheck(string characterName)
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        nameText.text = characterName;
        dialogueText.text = "Hey - Sarah, right? Glad you came. Catch you in class.";
        SetOtherLayout();
        nextArrow.SetActive(true);
        StartCoroutine(WaitForClickThenSolo());
    }

    IEnumerator WaitForClickThenSolo()
    {
        yield return null;

        while (!Mouse.current.leftButton.wasPressedThisFrame)
            yield return null;

        JumpTo(56);
    }

    void EndGame()
    {
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
}