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

    [Header("Settings")]
    public float textSpeed = 0.03f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool waitingForChoice = false;

    private string[] lines = {
        // SCENE 1 - Dorm Room
        "Okay... new city, new life, new me.",
        "No parents. No rules. Just... university.",
        "Have you unpacked? Don't forget why you're there. Focus on your studies!",
        "CHOICE_1",
        // SCENE 2 - Chloe enters
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
        // SCENE 1
        "Sarah (Thinking)",
        "Sarah (Thinking)",
        "Mom (text)",
        "",
        // SCENE 2
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
            // Show Chloe sliding in from right and stopping in the middle
            characterRight.gameObject.SetActive(true);
            StartCoroutine(SlideInCharacter(
                characterRight.rectTransform,
                1500f,  // starts off screen to the right
                0f      // stops in the middle of the screen
            ));
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

    IEnumerator SlideInCharacter(RectTransform character, float startX, float endX)
    {
        Vector2 startPos = new Vector2(startX, character.anchoredPosition.y);
        Vector2 endPos = new Vector2(endX, character.anchoredPosition.y);
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            character.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        character.anchoredPosition = endPos;
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