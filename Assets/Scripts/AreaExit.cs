using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    public string areaToLoad;
    public string areaTransitionName;
    public AreaEntrance entrance;
    public float areaTransitionTime = 0.2f;
    public bool shouldLoadAfterFade;

    // Start is called before the first frame update
    void Start()
    {
        entrance.transitionName = areaTransitionName;
    }

    // Update is called once per frame
    void Update()
    {
        // when player exits the area, wait for a time to fade out to black. THEN load the next scene
        if (shouldLoadAfterFade)
        {
            areaTransitionTime -= Time.deltaTime;
            if (areaTransitionTime <= 0)
            {
                shouldLoadAfterFade = false;
                SceneManager.LoadScene(areaToLoad);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController.instance.areaTransitionName = areaTransitionName;
            // SceneManager.LoadScene(areaToLoad);
            shouldLoadAfterFade = true;
            GameManager.instance.fadingBetweenAreas = true;
            UIFade.instance.FadeToBlack();
        }
    }
}
