using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ChangeSpeed : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            videoPlayer.playbackSpeed += 0.1f;
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {   
            videoPlayer.playbackSpeed -= 0.1f;
        }
    }
}
