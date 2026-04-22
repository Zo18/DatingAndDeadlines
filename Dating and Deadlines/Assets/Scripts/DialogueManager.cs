using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
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

    [Header("Settings")]
    public float textSpeed = 0.03f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool waitingForChoice = false;

    private string[] lines = {
        "Okay... new city, new life, new me.",
        "No parents. No rules. Just... university.",
        "Have you unpacked? Don't forget why you're there. Focus on your studies!",
        "CHOICE_1"
    };

    private string[] speakers = {
        "Sarah (Thinking)",
        "Sarah (Thinking)",
        "Mom (text)",
        ""
    };

    void Start()
    {
        Debug.Log("DialogueManager Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);
        ShowLine();
    }

    void Update()
    {
        if (waitingForChoice) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Click detected!");
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
        if (lines[currentLine] == "CHOICE_1")
        {
            ShowChoice();
            return;
        }
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        nextArrow.SetActive(false);
        nameText.text = speakers[currentLine];
        dialogueText.text = "";

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
            dialogueText.text = "--- End of Scene ---";
            nextArrow.SetActive(false);
        }
    }

    void ShowChoice()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "What do you reply to Mom?";
        choice1Text.text = "I know, Mom.";
        choice2Text.text = "I'll be fine.";
        choice3Text.text = "Ignore message";
    }

    public void OnChoice1Selected()
    {
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
        StatsManager.Instance.ModifyAcademics(-1);
        AfterChoice();
    }

    void AfterChoice()
    {
        choicePanel.SetActive(false);
        waitingForChoice = false;
        currentLine++;
        if (currentLine < lines.Length)
            ShowLine();
        else
        {
            dialogueText.text = "--- End of Scene ---";
            nextArrow.SetActive(false);
        }
    }
}