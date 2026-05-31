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
 
    [Header("Name and Dialogue Box")]
    public RectTransform nameBox;
    public RectTransform dialogueBox;
 
    [Header("Settings")]
    public float textSpeed = 0.03f;
 
    [Header("Stat Thresholds")]
    public int deanThreshold = 3;   // Academics required for Dean route
    public int gregThreshold = 3;   // Social required for Greg route
 
    [Header("End Of Game")]
    [Tooltip("Name of the scene to load when the game ends (e.g. MainMenu).")]
    public string endSceneName = "MainMenu";
 
    // === Layout values (copied straight from Scene 1 so positioning matches) ===
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
 
    // Which route the player is on. Set by the main CHOICE_ROUTE.
    // Possible values: "Dean", "Greg", "Solo"
    private string currentRoute = "";
 
    // === Dialogue lines ===
    // Special tags:
    //   CHOICE_ROUTE     -> the main stat-gated route choice
    //   CHOICE_DEAN      -> the in-route choice during Dean's ending
    //   CHOICE_GREG      -> the in-route choice during Greg's ending
    //   GOTO_DEAN        -> jump to start of Dean ending
    //   GOTO_GREG        -> jump to start of Greg ending
    //   GOTO_SOLO        -> jump to start of Solo ending
    //   END_GAME         -> load end scene
    private string[] lines = {
        // ---- OPENING (index 0-5) ----
        "Music, lights, so many people, this is going to be a good night!",
        "You made it. I was about to send a search party.",
        "I was the search party.",
        "Okay, big night, our first first-year party!",
        "Yay!",
        "I'm so excited!",
 
        // ---- THE ROUTE CHOICE (index 6) ----
        "CHOICE_ROUTE",
 
        // ==== DEAN ENDING (index 7-22) ====
        "Ah, I see you came.",                                              // 7
        "Yes, yes I did indeed.",                                            // 8
        "So how have your first few days been?",                             // 9
        "They've been good, I'm excited to see what this year holds for me.",// 10
        "Yeah, me too.",                                                     // 11
        "I didn't really expect to see you here, honestly.",                 // 12
        "Why's that?",                                                       // 13
        "I dunno. You seem like the type who'd be home with a textbook.",    // 14
        "CHOICE_DEAN",                                                       // 15
        "Okay, fair.",                                                       // 16
        "This is gonna sound weird, but I was kind of hoping you'd show up.",// 17
        "Yeah?",                                                             // 18
        "Yeah.",                                                             // 19
        "Okay. That's... a lot for a Tuesday.",                              // 20
        "Anyway. We should swap numbers. For class stuff. Or... not just class stuff.", // 21
        "Smooth.",                                                           // 22
        "I've been practising.",                                             // 23
        "See you in lecture tomorrow?",                                      // 24
        "See you tomorrow, Dean.",                                           // 25
        "Day two of uni. So far, so good.",                                  // 26
        "END_GAME",                                                          // 27
 
        // ==== GREG ENDING (index 28+) ====
        "Well, well. Look who showed up.",                                   // 28
        "Were you waiting for me?",                                          // 29
        "Maybe. Don't let it go to your head.",                              // 30
        "So, surviving first year so far?",                                  // 31
        "Two days in, I'm undefeated.",                                      // 32
        "Bold.",                                                             // 33
        "I'm just figuring it out as I go.",                                 // 34
        "Honestly? Same. I just hide it better.",                            // 35
        "Ohhh, you're so mysterious.",                                       // 36
        "Haha, very funny. But honestly I don't usually do the whole... get-to-know-you thing at parties.", // 37
        "What do you usually do?",                                           // 38
        "Avoid it. You're easy to talk to, though.",                         // 39
        "CHOICE_GREG",                                                       // 40
        "Don't get used to it.",                                             // 41
        "Put your number in my phone. In case you wanna keep talking when there's no bass involved.", // 42
        "Smooth.",                                                           // 43
        "I have my moments.",                                                // 44
        "Find me in class on Monday?",                                       // 45
        "Back row?",                                                         // 46
        "Obviously.",                                                        // 47
        "Deal.",                                                             // 48
        "That wasn't what I expected. In a good way.",                       // 49
        "END_GAME",                                                          // 50
 
        // ==== SOLO ENDING (index 51+) ====
        "Honestly? I just wanna hang out with you two tonight.",             // 51
        "Aww, babe.",                                                        // 52
        "That's the best answer.",                                           // 53
        "Okay but we ARE dancing. Non-negotiable.",                          // 54
        "I have so many regrettable moves prepared.",                        // 55
        "What ARE you doing?",                                               // 56
        "It's called expression, Sarah.",                                    // 57
        "It's called being unwell.",                                         // 58
        "Two days ago I didn't know either of them. Now I'm yelling a chorus at the ceiling with them.", // 59
        "Okay. Promise me. Every party. The three of us.",                   // 60
        "Pinky promise.",                                                    // 61
        "Pinky promise.",                                                    // 62
        "There's a whole year for everything else. Tonight is just this.",   // 63
        "Another song! Go go go!",                                           // 64
        "END_GAME"                                                           // 65
    };
 
    private string[] speakers = {
        // Opening
        "Sarah (Thinking)",
        "Chloe",
        "Maya",
        "Chloe",
        "Maya",
        "Sarah",
 
        // Route choice
        "",
 
        // Dean ending
        "Dean",                 // 7
        "Sarah",                // 8
        "Dean",                 // 9
        "Sarah",                // 10
        "Dean",                 // 11
        "Dean",                 // 12
        "Sarah",                // 13
        "Dean",                 // 14
        "",                     // 15 CHOICE_DEAN
        "Dean",                 // 16
        "Dean",                 // 17
        "Sarah",                // 18
        "Dean",                 // 19
        "Sarah (Thinking)",     // 20
        "Dean",                 // 21
        "Sarah",                // 22
        "Dean",                 // 23
        "Dean",                 // 24
        "Sarah",                // 25
        "Sarah (Thinking)",     // 26
        "",                     // 27 END_GAME
 
        // Greg ending
        "Greg",                 // 28
        "Sarah",                // 29
        "Greg",                 // 30
        "Greg",                 // 31
        "Sarah",                // 32
        "Greg",                 // 33
        "Sarah",                // 34
        "Greg",                 // 35
        "Sarah",                // 36
        "Greg",                 // 37
        "Sarah",                // 38
        "Greg",                 // 39
        "",                     // 40 CHOICE_GREG
        "Greg",                 // 41
        "Greg",                 // 42
        "Sarah",                // 43
        "Greg",                 // 44
        "Greg",                 // 45
        "Sarah",                // 46
        "Greg",                 // 47
        "Sarah",                // 48
        "Sarah (Thinking)",     // 49
        "",                     // 50 END_GAME
 
        // Solo ending
        "Sarah",                // 51
        "Chloe",                // 52
        "Maya",                 // 53
        "Chloe",                // 54
        "Maya",                 // 55
        "Sarah",                // 56
        "Maya",                 // 57
        "Chloe",                // 58
        "Sarah (Thinking)",     // 59
        "Chloe",                // 60
        "Maya",                 // 61
        "Sarah",                // 62
        "Sarah (Thinking)",     // 63
        "Chloe",                // 64
        ""                      // 65 END_GAME
    };
 
    void Start()
    {
        Debug.Log("DialogueManager_Scene6 Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);
        ShowLine();
    }
 
    void Update()
    {
        if (waitingForChoice) return;
 
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
 
        // Handle special tags
        if (line == "CHOICE_ROUTE")
        {
            ShowRouteChoice();
            return;
        }
        if (line == "CHOICE_DEAN")
        {
            ShowDeanChoice();
            return;
        }
        if (line == "CHOICE_GREG")
        {
            ShowGregChoice();
            return;
        }
        if (line == "END_GAME")
        {
            EndGame();
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
 
        // Same layout logic as Scene 1: MC on left, everyone else on right
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
    // CHOICE BUTTON HANDLERS — hook these up in the Inspector's OnClick events
    // ============================================================
    public void OnChoice1Selected()
    {
        string tag = lines[currentLine];
 
        if (tag == "CHOICE_ROUTE")
        {
            // Try Dean route (requires Academics >= deanThreshold)
            if (StatsManager.Instance.Academics >= deanThreshold)
            {
                currentRoute = "Dean";
                JumpTo(7); // start of Dean ending
            }
            else
            {
                FailedCheck("Dean");
            }
        }
        else if (tag == "CHOICE_DEAN" || tag == "CHOICE_GREG")
        {
            // In-route choices: all options lead to the same next line
            AfterChoice();
        }
    }
 
    public void OnChoice2Selected()
    {
        string tag = lines[currentLine];
 
        if (tag == "CHOICE_ROUTE")
        {
            // Try Greg route (requires Social >= gregThreshold)
            if (StatsManager.Instance.Social >= gregThreshold)
            {
                currentRoute = "Greg";
                JumpTo(28); // start of Greg ending
            }
            else
            {
                FailedCheck("Greg");
            }
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
            // Solo route - no stat check needed
            currentRoute = "Solo";
            JumpTo(51); // start of Solo ending
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
 
    /// <summary>
    /// Jump straight to a specific line index (used to skip from the route
    /// choice to the start of the chosen ending).
    /// </summary>
    void JumpTo(int index)
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        currentLine = index;
        ShowLine();
    }
 
    /// <summary>
    /// Player picked Dean or Greg but didn't meet the stat threshold.
    /// Show the brush-off line, then roll into the Solo ending.
    /// </summary>
    void FailedCheck(string characterName)
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        nameText.text = characterName;
        dialogueText.text = "Hey - Sarah, right? Glad you came. Catch you in class.";
 
        // Use the "other" layout since it's Dean or Greg speaking
        SetOtherLayout();
        nextArrow.SetActive(true);
 
        // Override what happens on the next click: jump straight to Solo
        StartCoroutine(WaitForClickThenSolo());
    }
 
    IEnumerator WaitForClickThenSolo()
    {
        // Wait one frame so the click that picked the choice doesn't immediately register
        yield return null;
 
        while (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            yield return null;
        }
 
        currentRoute = "Solo";
        JumpTo(51);
    }
 
    void EndGame()
    {
        dialogueText.text = "--- End of Game ---";
        nameText.text = "";
        nextArrow.SetActive(false);
 
        // Load main menu after a short delay
        StartCoroutine(LoadEndSceneAfterDelay(2f));
    }
 
    IEnumerator LoadEndSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!string.IsNullOrEmpty(endSceneName))
            SceneManager.LoadScene(endSceneName);
    }
}
 