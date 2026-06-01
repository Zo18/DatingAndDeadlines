using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scene4Events : MonoBehaviour
{
    [Header("Dialogue UI")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Next Arrow")]
    public GameObject nextArrow;

    [Header("Characters")]
    public GameObject SarahCharacterSprite;
    public GameObject DeanCharacterSprite;
    public GameObject GregCharacterSprite;
    public GameObject LecturerSprite;

    [Header("Audio")]
    public AudioSource StudentsSpeaking;
    public AudioSource StudentsGroan;

    [Header("Choice Panel")]
    public GameObject choicePanel;
    public Button choice1Button;
    public Button choice2Button;

    [Header("Stats")]
    public TMP_Text academicText;
    public TMP_Text socialText;
    public TMP_Text loveText;

    [Header("Typing Settings")]
    public float typingSpeed = 0.025f;

    private bool isTyping = false;
    private bool sceneFinished = false;
    private string currentSentence;

    private int academics = 0;
    private int social = 0;
    private int love = 0;

    void Start()
    {
        // Text size
        nameText.fontSize = 18;
        dialogueText.fontSize = 18;

        academicText.fontSize = 18;
        socialText.fontSize = 18;
        loveText.fontSize = 18;
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);

        DeanCharacterSprite.SetActive(false);
        GregCharacterSprite.SetActive(false);
        LecturerSprite.SetActive(true);
        SarahCharacterSprite.SetActive(true);

        UpdateStats();

        StudentsSpeaking.Play();

        StartCoroutine(LecturerIntro());
    }

    void Update()
    {
        if (sceneFinished) return;
        if (choicePanel.activeSelf) return;

        bool clicked =
            Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (clicked)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentSentence;
                isTyping = false;
                nextArrow.SetActive(true);
            }
            else
            {
                NextStep();
            }
        }
    }

    IEnumerator LecturerIntro()
    {
        nameText.text = "Lecturer";

        yield return StartCoroutine(TypeSentence("Settle down… settle down students."));

        StudentsSpeaking.Stop();

        yield return StartCoroutine(TypeSentence("Welcome everyone. It's great to see so many new faces. I’ll be your lecturer for this semester."));

        LecturerSprite.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        nameText.text = "Sarah (Thinking)";
        yield return StartCoroutine(TypeSentence("He seems cool."));

        yield return new WaitForSeconds(0.5f);

        ShowChoice();
    }

    void ShowChoice()
    {
        choicePanel.SetActive(true);

        choice1Button.GetComponentInChildren<TMP_Text>().text = "Front row";
        choice2Button.GetComponentInChildren<TMP_Text>().text = "Back row";

        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();

        choice1Button.onClick.AddListener(FrontRow);
        choice2Button.onClick.AddListener(BackRow);
    }

    void FrontRow()
    {
        academics++;
        UpdateStats();
        choicePanel.SetActive(false);
        StartCoroutine(DeanPath());
    }

    void BackRow()
    {
        social++;
        UpdateStats();
        choicePanel.SetActive(false);
        StartCoroutine(GregPath());
    }

    IEnumerator DeanPath()
    {
        DeanCharacterSprite.SetActive(true);

        nameText.text = "Dean";
        yield return StartCoroutine(TypeSentence("You can share with me if you didn't get the reading list yet. It's… a lot. My name's Dean."));

        nameText.text = "Sarah";
        yield return StartCoroutine(TypeSentence("Thanks, I'm Sarah. You actually did the pre-reading?"));

        nameText.text = "Dean";
        yield return StartCoroutine(TypeSentence("Yeah.. I did."));

        ShowDeanChoice();
    }

    void ShowDeanChoice()
    {
        choicePanel.SetActive(true);

        choice1Button.GetComponentInChildren<TMP_Text>().text = "Honestly, that's kind of impressive. ";
        choice2Button.GetComponentInChildren<TMP_Text>().text = "Wow. You're THAT guy. ";

        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();

        choice1Button.onClick.AddListener(() =>
        {
            love++;
            UpdateStats();
            choicePanel.SetActive(false);
            StartCoroutine(FinalLine());
        });

        choice2Button.onClick.AddListener(() =>
        {
            social++;
            UpdateStats();
            choicePanel.SetActive(false);
            StartCoroutine(FinalLine());
        });
    }

    IEnumerator GregPath()
    {
        GregCharacterSprite.SetActive(true);

        nameText.text = "Greg";
        yield return StartCoroutine(TypeSentence("Smart move. Back here, the lecturer can't see your face when you fall asleep."));

        nameText.text = "Sarah";
        yield return StartCoroutine(TypeSentence("You've clearly thought about this."));

        nameText.text = "Greg";
        yield return StartCoroutine(TypeSentence("I'm a strategist. Greg by the way."));

        ShowGregChoice();
    }

    void ShowGregChoice()
    {
        choicePanel.SetActive(true);

        choice1Button.GetComponentInChildren<TMP_Text>().text = "I'm Sarah. And I actually want to pass. ";
        choice2Button.GetComponentInChildren<TMP_Text>().text = "Haha I'm Sarah, and that sounds like a good strategy. ";

        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();

        choice1Button.onClick.AddListener(() =>
        {
            academics++;
            UpdateStats();
            choicePanel.SetActive(false);
            StartCoroutine(FinalLine());
        });

        choice2Button.onClick.AddListener(() =>
        {
            love++;
            UpdateStats();
            choicePanel.SetActive(false);
            StartCoroutine(FinalLine());
        });
    }

    IEnumerator FinalLine()
    {
        nameText.text = "Lecturer";
        LecturerSprite.SetActive(true);

        yield return StartCoroutine(TypeSentence("Your first assignment is due Friday. Yes, this Friday."));

        StudentsGroan.Play();

        yield return new WaitForSeconds(1f);

        sceneFinished = true;
        nextArrow.SetActive(false);

        Debug.Log("Scene 4 Complete");
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        currentSentence = sentence;
        dialogueText.text = "";

        foreach (char c in sentence)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        nextArrow.SetActive(true);
    }

    void NextStep()
    {
        // Intentionally empty (flow is coroutine-driven)
    }

    void UpdateStats()
    {
        academicText.text = "Academics: " + academics;
        socialText.text = "Social: " + social;
        loveText.text = "Love: " + love;
    }
}