using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Movie_Toyota2Stone : MonoBehaviour
{
    private GameObject movie;
    private GameObject text;
    private VideoPlayer vp;
    private Text t;

    // Start is called before the first frame update
    void Start()
    {
        movie = GameObject.Find("RawImage");
        vp = movie.GetComponent<VideoPlayer>();

        text = GameObject.Find("Text");
        t = text.GetComponent<Text>();
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

        t.text = "Speed : " + vp.playbackSpeed;

        vp.loopPointReached += LoopPointReached;
    }

    // 動画再生完了時の処理
    public void LoopPointReached(VideoPlayer vp)
    {
        SceneManager.LoadScene("Stone");
    }
}
