using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Movie_Toyota2Stone : MonoBehaviour
{
    private GameObject movie;
    private VideoPlayer vp;

    // Start is called before the first frame update
    void Start()
    {
        movie = GameObject.Find("RawImage");
        vp = movie.GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            vp.playbackSpeed += 0.1f;
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {   
            vp.playbackSpeed -= 0.1f;
        }
    }
}
