using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : MonoBehaviour
{
    public string[] lines;
    public bool canActivate;
    public string speakerName;
    public bool isPerson;

    public bool shouldActivateQuest;
    public string questToMark;
    public bool markComplete;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canActivate && Input.GetButtonDown("Fire1") && !DialogueManager.instance.dialogBox.activeInHierarchy) {
            DialogueManager.instance.ShowDialog(speakerName, isPerson, lines);
            // just after showing the dialog, pass in quest related information
            DialogueManager.instance.ShouldActivateQuestAtEnd(questToMark, markComplete);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canActivate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canActivate = false;
        }
    }
}
