using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoClipTextureActions : MonoBehaviour
{
    public VideoPlayer playNow;
    bool playerWatch = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            if (!playNow.isPlaying || playNow.isPaused)
            {
                playerWatch = true;
                playNow.Play();
            }
                
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            if (playNow.isPlaying)
            {
                playerWatch = false;
                playNow.Pause();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerWatch)
        {
            playNow.Pause();
        }
    }
}
