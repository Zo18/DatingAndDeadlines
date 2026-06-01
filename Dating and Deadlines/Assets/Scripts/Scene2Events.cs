using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scene2Events : MonoBehaviour
{
    [Header("Dialogue")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Next Arrow")]
    public GameObject nextArrow;

    [Header("Characters")]
    public GameObject sarahSprite;
    public GameObject chloeSprite;
    public GameObject mayaSprite;

    [Header("Choices")]
    public GameObject choicePanel;

    public Button choice1Button;
    public Button choice2Button;
    public Button choice3Button;

    [Header("Stats")]
    public TMP_Text academicText;
    public TMP_Text socialText;
    public TMP_Text loveText;

    [Header("Audio")]
    public AudioSource knockSound;

    [Header("Typing")]
    public float typingSpeed = 0.02f;

    private bool isTyping = false;
    private bool sceneFinished = false;

    private string currentSentence = "";

    private int academics = 0;
    private int social = 0;
    private int love = 0;

    private int dialogueIndex = 0;

    private string[] speakers =
    {
        "Chloe",
        "",
        "Maya",
        "Chloe",
        "Maya",
        "Maya"
    };

    private string[] lines =
    {
        "Come on, I'll show you around. The good vending machine's on floor two.",
        "",
        "Sorry, I heard someone moving in. I'm Maya, three doors down.",
        "Floor monitor energy.",
        "I heard that.",
        "If you need anything that isn't sugar and bad advice, knock."
    };

    void Start()
    {
        // Text size
        nameText.fontSize = 20;
        dialogueText.fontSize = 20;

        academicText.fontSize = 20;
        socialText.fontSize = 20;
        loveText.fontSize = 20;
        // Hide Maya initially
        sarahSprite.SetActive(true);
        chloeSprite.SetActive(true);
        mayaSprite.SetActive(false);

        // Hide choices
        choicePanel.SetActive(false);

        // Hide next arrow
        if (nextArrow != null)
        {
            nextArrow.SetActive(false);
        }

        // Set choice text size
        TMP_Text c1 = choice1Button.GetComponentInChildren<TMP_Text>();
        TMP_Text c2 = choice2Button.GetComponentInChildren<TMP_Text>();
        TMP_Text c3 = choice3Button.GetComponentInChildren<TMP_Text>();

        if (c1 != null) c1.fontSize = 20;
        if (c2 != null) c2.fontSize = 20;
        if (c3 != null) c3.fontSize = 20;

        // Button listeners
        choice1Button.onClick.AddListener(ChooseAcademics);
        choice2Button.onClick.AddListener(ChooseSocial);
        choice3Button.onClick.AddListener(ChooseLove);

        UpdateStats();

        ShowDialogue();
    }

    void Update()
    {
        if (sceneFinished)
            return;

        if (choicePanel.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();

                dialogueText.text = currentSentence;

                isTyping = false;

                if (nextArrow != null)
                {
                    nextArrow.SetActive(true);
                }
            }
            else
            {
                NextDialogue();
            }
        }
    }

    void ShowDialogue()
    {
        if (nextArrow != null)
        {
            nextArrow.SetActive(false);
        }

        nameText.text = speakers[dialogueIndex];

        if (dialogueIndex == 1)
        {
            StartCoroutine(MayaEntrance());
            return;
        }

        currentSentence = lines[dialogueIndex];

        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator MayaEntrance()
    {
        if (knockSound != null)
        {
            knockSound.Play();
        }

        nameText.text = "";
        currentSentence = "*Knock*";

        yield return StartCoroutine(TypeSentence(currentSentence));

        yield return new WaitForSeconds(0.5f);

        mayaSprite.SetActive(true);

        dialogueIndex++;

        ShowDialogue();
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (nextArrow != null)
        {
            nextArrow.SetActive(true);
        }
    }

    void NextDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex >= lines.Length)
        {
            ShowChoices();
            return;
        }

        ShowDialogue();
    }

    void ShowChoices()
    {
        choicePanel.SetActive(true);

        TMP_Text c1 = choice1Button.GetComponentInChildren<TMP_Text>();
        TMP_Text c2 = choice2Button.GetComponentInChildren<TMP_Text>();
        TMP_Text c3 = choice3Button.GetComponentInChildren<TMP_Text>();

        if (c1 != null)
        {
            c1.text = "Actually, could you show me where the library is later?";
        }

        if (c2 != null)
        {
            c2.text = "Bad advice sounds fun.";
        }

        if (c3 != null)
        {
            c3.text = "Thanks, Maya. That's really kind.";
        }
    }

    void ChooseAcademics()
    {
        academics++;
        UpdateStats();
        StartCoroutine(AfterChoiceSequence());
    }

    void ChooseSocial()
    {
        social++;
        UpdateStats();
        StartCoroutine(AfterChoiceSequence());
    }

    void ChooseLove()
    {
        love++;
        UpdateStats();
        StartCoroutine(AfterChoiceSequence());
    }

    IEnumerator AfterChoiceSequence()
    {
        choicePanel.SetActive(false);

        nameText.text = "Sarah (Thinking)";
        yield return StartCoroutine(
            TypeSentence("Two very different people. Two very different first years.")
        );

        yield return new WaitForSeconds(1f);

        nameText.text = "Maya";
        yield return StartCoroutine(
            TypeSentence("Oh! We're all in the same intro class, by the way. First lecture tomorrow, nine a.m.")
        );

        yield return new WaitForSeconds(1f);

        nameText.text = "Chloe";
        yield return StartCoroutine(
            TypeSentence("Nine A.M. should be illegal.")
        );

        sceneFinished = true;

        if (nextArrow != null)
        {
            nextArrow.SetActive(false);
        }

        Debug.Log("Scene Finished");
    }

    void UpdateStats()
    {
        academicText.text = "Academics: " + academics;
        socialText.text = "Social: " + social;
        loveText.text = "Love: " + love;
    }
}