using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string[] questMarkerNames;
    public bool[] questMarkersComplete;

    public static QuestManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        questMarkersComplete = new bool[questMarkerNames.Length];
    }

    // Update is called once per frame
    void Update()
    {
        // code for testing (START)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(CheckIfComplete("quest test"));
            MarkQuestComplete("quest test");
            MarkQuestIncomplete("fight the demon");
        }
        // code for testing (END)

        // code for testing QUEST SAVE / LOAD (START)
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveQuestData();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadQuestData();
        }
        // code for testing QUEST SAVE / LOAD (END)
    }

    public int GetQuestNumber(string questToFind)
    {
        for (int i = 0; i < questMarkerNames.Length; i++)
        {
            if (questMarkerNames[i] == questToFind)
            {
                return i;
            }
        }
        // the quest does not exist
        // return 0;
        Debug.LogError("Quest " + questToFind + " does not exist");
        return -1;
    }

    public bool CheckIfComplete(string questToCheck)
    {
        // if the quest exists, then check if it is complete
        int questNumber = GetQuestNumber(questToCheck);
        if (questNumber != -1)
        {
            return questMarkersComplete[questNumber];
        }
        // otherwise, return "false" by default
        Debug.Log("Quest not found: " + questToCheck);
        return false;
    }

    public void MarkQuestComplete(string questToMark)
    {
        int questNumber = GetQuestNumber(questToMark);
        if (questNumber != -1)
        {
            questMarkersComplete[questNumber] = true;
        }

        UpdateLocalQuestObjects();
    }

    public void MarkQuestIncomplete(string questToMark)
    {
        int questNumber = GetQuestNumber(questToMark);
        if (questNumber != -1)
        {
            questMarkersComplete[questNumber] = false;
        }

        UpdateLocalQuestObjects();
    }

    public void UpdateLocalQuestObjects()
    {
        // NOTE: this will only find the quest objects LOCAL in the scene.
        // IF the quest object is another scene, this will not work for that object
        QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>();
        if (questObjects.Length > 0)
        {
            for (int i = 0; i < questObjects.Length; i++)
            {
                questObjects[i].CheckCompletion();
            }
        }
    }

    public void SaveQuestData()
    {
        for (int i = 0; i < questMarkerNames.Length; i++)
        {
            if (questMarkersComplete[i])
            {
                // PlayerPrefs is used to store data in the system
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 1);
            } else
            {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 0);
            }
        }
    }

    public void LoadQuestData()
    {
        for (int i = 0; i < questMarkerNames.Length; i++)
        {
            // initialize the bool value to false, in case we have new quests in the array than we did when we last saved
            int valueToSet = 0;
            if (PlayerPrefs.HasKey("QuestMarker_" + questMarkerNames[i]))
            {
                valueToSet = PlayerPrefs.GetInt("QuestMarker_" + questMarkerNames[i]);
            }

            if (valueToSet == 0)
            {
                questMarkersComplete[i] = false;
            } else
            {
                questMarkersComplete[i] = true;
            }
        }
    }
}
