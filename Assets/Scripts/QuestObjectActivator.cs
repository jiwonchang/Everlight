using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectActivator : MonoBehaviour
{
    public GameObject objectToActivate;
    public string questToCheck;
    public bool activeIfComplete;

    private bool initialCheckDone;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // run the check once ALL other objects in the scene have been loaded in
        // (For example, our code could break if the GameManager hasn't loaded but the Quest Object has)
        if (!initialCheckDone)
        {
            initialCheckDone = true;
            CheckCompletion();
        }
    }

    // checks to see whether a quest is complete
    public void CheckCompletion()
    {
        if (QuestManager.instance.CheckIfComplete(questToCheck))
        {
            Debug.Log("active if complete!! " + activeIfComplete);
            objectToActivate.SetActive(activeIfComplete);
        }
    }
}
