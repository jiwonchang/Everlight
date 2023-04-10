using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public Text dialogText;
    public Text nameText;
    public GameObject dialogBox;
    public GameObject nameBox;

    public string[] dialogLines;
    public int currentLine;
    private bool justStarted;

    private string questToMark;
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogBox.activeInHierarchy)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                if (!justStarted) {
                    currentLine++;
                    if (currentLine >= dialogLines.Length) {
                        // currentLine = 0;
                        dialogBox.SetActive(false);
                        GameManager.instance.dialogActive = false;
                        // at the end of the dialog, determine whether a certain quest should be marked complete
                        if (shouldMarkQuest)
                        {
                            shouldMarkQuest = false;
                            if (markQuestComplete)
                            {
                                QuestManager.instance.MarkQuestComplete(questToMark);
                            } else
                            {
                                QuestManager.instance.MarkQuestIncomplete(questToMark);
                            }
                        }
                    }
                    else {
                        dialogText.text = dialogLines[currentLine];
                    }
                } else
                {
                    justStarted = false;
                }
            }
        }
    }

    public void ShowDialog(string name, bool isPerson, string[] newLines)
    {
        dialogLines = newLines;
        currentLine = 0;
        dialogText.text = dialogLines[currentLine];
        dialogBox.SetActive(true);
        if (isPerson)
        {
            nameText.text = name;
            nameBox.SetActive(true);
        } else
        {
            nameBox.SetActive(false);
        }
        justStarted = true;
        GameManager.instance.dialogActive = true;
    }

    public void ShouldActivateQuestAtEnd(string questName, bool markComplete)
    {
        questToMark = questName;
        markQuestComplete = markComplete;

        shouldMarkQuest = true;
    }
}
