using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("Characters")]
    public Image characterLeft;
    public Image characterRight;

    [Header("Name and Dialogue Box")]
    public RectTransform nameBox;
    public RectTransform dialogueBox;

    [Header("Settings")]
    public float textSpeed = 0.03f;

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

    private string[] lines = {
        "Okay... new city, new life, new me.",
        "No parents. No rules. Just... university.",
        "Have you unpacked? Don't forget why you're there. Focus on your studies!",
        "CHOICE_1",
        "CHLOE_ENTER",
        "You look new.",
        "Is it that obvious?",
        "Relax. Everyone looks confused on day one.",
        "I'm Chloe.",
        "Sarah.",
        "Cute. You seem... quiet.",
        "CHOICE_2"
    };

    private string[] speakers = {
        "Sarah (Thinking)",
        "Sarah (Thinking)",
        "Mom (Text)",
        "",
        "",
        "Chloe",
        "Sarah",
        "Chloe",
        "Chloe",
        "Sarah",
        "Chloe",
        ""
    };

    void Start()
    {
        Debug.Log("DialogueManager Started!");
        choicePanel.SetActive(false);
        nextArrow.SetActive(false);
        characterRight.gameObject.SetActive(false);
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
        if (lines[currentLine] == "CHOICE_1")
        {
            ShowChoice1();
            return;
        }
        if (lines[currentLine] == "CHOICE_2")
        {
            ShowChoice2();
            return;
        }
        if (lines[currentLine] == "CHLOE_ENTER")
        {
            characterRight.gameObject.SetActive(true);
            currentLine++;
            ShowLine();
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
        else
        {
            dialogueText.text = "--- End of Scene ---";
            nextArrow.SetActive(false);
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

    void ShowChoice1()
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

    void ShowChoice2()
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);
        choicePanel.SetActive(true);
        nameText.text = "";
        dialogueText.text = "How do you respond to Chloe?";
        choice1Text.text = "I'm just observing.";
        choice2Text.text = "I'm a little nervous.";
        choice3Text.text = "I don't like people.";
    }

    public void OnChoice1Selected()
    {
        if (currentLine == GetChoiceIndex("CHOICE_1"))
            StatsManager.Instance.ModifyAcademics(1);
        else if (currentLine == GetChoiceIndex("CHOICE_2"))
            StatsManager.Instance.ModifyAcademics(1);
        AfterChoice();
    }

    public void OnChoice2Selected()
    {
        if (currentLine == GetChoiceIndex("CHOICE_1"))
            StatsManager.Instance.ModifySocial(1);
        else if (currentLine == GetChoiceIndex("CHOICE_2"))
            StatsManager.Instance.ModifyLove(1);
        AfterChoice();
    }

    public void OnChoice3Selected()
    {
        if (currentLine == GetChoiceIndex("CHOICE_1"))
            StatsManager.Instance.ModifyAcademics(-1);
        else if (currentLine == GetChoiceIndex("CHOICE_2"))
            StatsManager.Instance.ModifySocial(-1);
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

    int GetChoiceIndex(string choiceTag)
    {
        for (int i = 0; i < lines.Length; i++)
            if (lines[i] == choiceTag) return i;
        return -1;
    }
}