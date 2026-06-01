using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager_Scene2 : MonoBehaviour
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
    private Vector2 characterRight2Home;

    private string[] lines = {
        "CHLOE_ENTER",
        "Come on, I'll show you around. The good vending machine's on floor two.",
        "MAYA_ENTER",
        "Sorry, I heard someone moving in. I'm Maya, three doors down.",
        "Floor monitor energy.",
        "I heard that.",
        "If you need anything that isn't sugar and bad advice, knock.",
        "CHOICE_MAYA",
        "Two very different people. Two very different first years.",
        "Oh! We're all in the same intro class, by the way. First lecture tomorrow, nine a.m.",
        "Nine A.M. should be illegal.",
        "FADE_OUT"
    };

    private string[] speakers = {
        "",
        "Chloe",
        "",
        "Maya",
        "Chloe",
        "Maya",
        "Maya",
        "",
        "Sarah (Thinking)",
        "Maya",
        "Chloe",
        ""
    };

    void Start()
    {
        Debug.Log("DialogueManager_Scene2 Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);

        if (characterRight != null)
            characterRightHome = characterRight.rectTransform.anchoredPosition;
        if (characterRight2 != null)
            characterRight2Home = characterRight2.rectTransform.anchoredPosition;

        if (characterRight != null) characterRight.gameObject.SetActive(false);
        if (characterRight2 != null) characterRight2.gameObject.SetActive(false);

        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(true);
        }

        LinkStatsDisplay();
        ShowLine();
    }

    void LinkStatsDisplay()
    {
        if (StatsManager.Instance == null) return;

        var academicsObj = GameObject.Find("AcademicsText");
        var socialObj    = GameObject.Find("SocialText");
        var loveObj      = GameObject.Find("LoveText");

        if (academicsObj != null) StatsManager.Instance.academicsText = academicsObj.GetComponent<TextMeshProUGUI>();
        if (socialObj != null)    StatsManager.Instance.socialText    = socialObj.GetComponent<TextMeshProUGUI>();
        if (loveObj != null)      StatsManager.Instance.loveText      = loveObj.GetComponent<TextMeshProUGUI>();

        StatsManager.Instance.ModifyAcademics(0);
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

        if (line == "CHOICE_MAYA")   { ShowMayaChoice(); return; }
        if (line == "FADE_OUT")      { StartCoroutine(FadeOutAndLoadNext()); return; }
        if (line == "CHLOE_ENTER")   { StartCoroutine(CharacterEnter(characterRight, chloeSprite, characterRightHome)); return; }
        if (line == "MAYA_ENTER")    { StartCoroutine(CharacterEnter(characterRight2, mayaSprite, characterRight2Home)); return; }

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

    IEnumerator CharacterEnter(Image character, Sprite sprite, Vector2 home)
    {
        isSliding = true;

        if (character != null)
        {
            character.sprite = sprite;
            character.gameObject.SetActive(true);
            character.rectTransform.anchoredPosition = home + new Vector2(slideStartOffset, 0);

            Vector2 start = character.rectTransform.anchoredPosition;
            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / slideDuration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                character.rectTransform.anchoredPosition = Vector2.Lerp(start, home, eased);
                yield return null;
            }
            character.rectTransform.anchoredPosition = home;
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

    void ShowMayaChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you respond?";
        choice1Text.text = "Actually, could you show me where the library is later?";
        choice2Text.text = "Bad advice sounds fun.";
        choice3Text.text = "Thanks, Maya. That's really kind.";
    }

    public void OnChoice1Selected()
    {
   if (StatsManager.Instance == null)
    {
        Debug.Log("StatsManager.Instance is NULL - no singleton exists");
    }
    else
    {
        Debug.Log("BEFORE: academics=" + StatsManager.Instance.academics);
        StatsManager.Instance.ModifyAcademics(1);
        Debug.Log("AFTER: academics=" + StatsManager.Instance.academics);
    }
    AfterChoice();
}

    public void OnChoice2Selected()
    {
        if (StatsManager.Instance != null) StatsManager.Instance.ModifySocial(1);
        AfterChoice();
    }

    public void OnChoice3Selected()
    {
        if (StatsManager.Instance != null) StatsManager.Instance.ModifyLove(1);
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