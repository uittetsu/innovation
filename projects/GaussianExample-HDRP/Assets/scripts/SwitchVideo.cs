using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwitchVideo : MonoBehaviour
{
    private GameObject text;
    private VideoPlayer vp;
    private MediaPipe mp;
    private Text t;
    public string nextSceneName;
    private bool seekDone = true; // 初期値をtrueに設定

    // 再生速度ごとに対応するビデオクリップ
    //public VideoClip normalSpeedClip;
    //public VideoClip doubleSpeedClip;
    //public VideoClip halfSpeedClip;
    public VideoClip[] videoClips;

    //private int clipIndex = 2
    private int currentClipIndex = 0;
    private int currentSpeed = 1;
    private int prevSpeed = 1;

    void Start()
    {

        GameObject videoPlayerObject = GameObject.Find("RawImage");
        vp = videoPlayerObject.GetComponent<VideoPlayer>();
        text = GameObject.Find("Text");
        t = text.GetComponent<Text>();
        vp.seekCompleted += SeekCompletedHandler;
        vp.loopPointReached += LoopPointReached;

        GameObject pythonScriptObject = GameObject.Find("MediaPipe");
        mp = pythonScriptObject.GetComponent<MediaPipe>();

        vp.clip = videoClips[currentClipIndex];


    }

    void Update()
    {

        if (!vp.isPrepared)
        {
            //Debug.LogError("VideoPlayer is not prepared.");
            return;
        }
        
        // 動画クリップを切り替える
        if (currentSpeed > prevSpeed)
        {
            prevSpeed = currentSpeed;
            if (currentClipIndex + 1 < videoClips.Length)
            {
                SwitchVideoClip(1);
            }
        }
        else if (currentSpeed < prevSpeed)
        {
            prevSpeed = currentSpeed;
            if (currentClipIndex - 1 >= 0)
            {
                SwitchVideoClip(-1);
            }
            
        }

        // // ユーザーの入力に応じて動画クリップを切り替える
        // if (Input.GetKeyDown(KeyCode.UpArrow) || currentSpeed > prevSpeed)
        // {
        //     prevSpeed = currentSpeed;
        //     if (currentClipIndex + 1 < videoClips.Length)
        //     {
        //         SwitchVideoClip(1);
        //     }
        // }
        // else if (Input.GetKeyDown(KeyCode.DownArrow) || currentSpeed < prevSpeed)
        // {
        //     prevSpeed = currentSpeed;
        //     if (currentClipIndex - 1 >= 0)
        //     {
        //         SwitchVideoClip(-1);
        //     }
            
        // }

        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    SwitchVideoClip(halfSpeedClip);
        //}

        // escapeでスキップ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mp.OnApplicationQuit();
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void SeekCompletedHandler(VideoPlayer player)
    {
        //Debug.Log("Seek completed at frame: " + vp.frame);
        seekDone = true;
    }

    // 動画再生完了時の処理
    public void LoopPointReached(VideoPlayer vp)
    {
        mp.OnApplicationQuit();
        SceneManager.LoadScene(nextSceneName);
    }

    public void SwitchVideoClip(int mode)
    {

        
        long currentFrame = vp.frame;

        double ratio;
        
        
        //vp.Stop();
        int prevClipIndex = currentClipIndex;
        //int nextFrame;

        if (mode == 1)
        {
            ratio = (double)(currentClipIndex + 2) / (double)(currentClipIndex + 3);
            //ratio = 1/1.5;

            currentClipIndex++;
        }
        else
        {
            ratio = (double)(currentClipIndex + 2) / (double)(currentClipIndex + 1);
            //ratio = 1.25;
            currentClipIndex--;
        }
        int nextFrame = (int)(currentFrame * ratio);
        Debug.Log(ratio);
        Debug.Log("Switch clip" + prevClipIndex + "to" + currentClipIndex);
        Debug.Log("Frame" + currentFrame + "to" + nextFrame);

        vp.clip = videoClips[currentClipIndex];

        StartCoroutine(SeekToFrame(nextFrame));
       

            
   
        
    }

    IEnumerator SeekToFrame(int frame)
    {
        yield return new WaitForEndOfFrame();
        vp.frame = frame;
        vp.Play();
    }
    public void SetPlaybackSpeed(float speed)
    {
        //speed = speed;
        t.text = "Speed : " + speed;
        
        currentSpeed = (int)speed;
        //Debug.Log(currentSpeed);
        
    }
}
