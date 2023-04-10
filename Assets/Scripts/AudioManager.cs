using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public AudioSource[] SFX;
    public AudioSource[] BGM;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // testing audio play sfx (START)
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     PlaySFX(0);
        //     PlayBGM(3);
        // }
        // testing audio play sfx (END)
    }

    public void PlaySFX(int soundToPlay)
    {
        if (soundToPlay < SFX.Length)
        {
            SFX[soundToPlay].Play();
        } else
        {
            Debug.LogError("SFX at index " + soundToPlay + " does not exist!");
        }
    }

    public void PlayBGM(int musicToPlay)
    {
        if (!BGM[musicToPlay].isPlaying) {
            StopMusic();

            if (musicToPlay < BGM.Length)
            {
                // stop any other currently playing BGM
                BGM[musicToPlay].Play();
            }
        }
    }

    public void StopMusic()
    {
        for (int i = 0; i < BGM.Length; i++)
        {
            BGM[i].Stop();
        }
    }
}
