using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 
public class Movie_ : MonoBehaviour
{
    private GameObject movie;
    private GameObject text;
    private VideoPlayer vp;
    private Text t;
    public string nextSceneName;

    public int skipFrames = 1; // 1フレームおきにスキップ（倍速再生）
    private int currentFrame = 0;
 
    void Start()
    {
        movie = GameObject.Find("RawImage");
        vp = movie.GetComponent<VideoPlayer>();

        vp.started += OnVideoStarted;
        vp.Play();

        text = GameObject.Find("Text");
        t = text.GetComponent<Text>();

        vp.loopPointReached += LoopPointReached;
    }
 
    void OnVideoStarted(VideoPlayer vp)
    {
        vp.started -= OnVideoStarted;
        currentFrame = (int)vp.frame;
        SkipFrames();
    }
 
    void SkipFrames()
    {
        if (vp.isPlaying)
        {
            int targetFrame = currentFrame + skipFrames;
            if (targetFrame < (int)vp.frameCount)
            {
                vp.frame = targetFrame;
                currentFrame = targetFrame;
            }
            else
            {
                vp.Pause();
            }
        }
    }
 
    void OnDestroy()
    {
        if (vp != null)
        {
            vp.started -= OnVideoStarted;
        }
    }

    // 動画再生完了時の処理
    public void LoopPointReached(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }

}
