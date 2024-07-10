using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class  MediaPipe : MonoBehaviour
{
    private Process pythonProcess;
    private Thread outputThread;
    private bool isRunning;
    private float speed;
    private readonly Regex brightnessRegex = new Regex(@"^\d+(\.\d+)?$"); // 数値のみをマッチ
    private SwitchVideo videoScript;
    private float prev_speed;

    void Start()
    {
        GameObject videoObject = GameObject.Find("RawImage");
        if (videoObject != null)
        {
            videoScript = videoObject.GetComponent<SwitchVideo>();
        }

        //RunPythonScript("python2unity/test.py");
        RunPythonScript("PythonScript/running_detection.py");
    }

    void RunPythonScript(string scriptPath)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "/opt/anaconda3/envs/opencv/bin/python"; // Pythonの実行ファイルのパス
        start.Arguments = string.Format("\"{0}\"", scriptPath);
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        pythonProcess = new Process();
        pythonProcess.StartInfo = start;

        isRunning = true;
        pythonProcess.Start();

        outputThread = new Thread(ReadOutput);
        outputThread.Start();
    }

    void ReadOutput()
    {
        using (StreamReader reader = pythonProcess.StandardOutput)
        {
            while (isRunning && !reader.EndOfStream)
            {
                string result = reader.ReadLine();
                //UnityEngine.Debug.Log("Raw Output: " + result); // 追加: 生の出力を表示
                if (!string.IsNullOrEmpty(result) && brightnessRegex.IsMatch(result))
                {
                    if (float.TryParse(result, out float parsedSpeed))
                    {
                        lock (this)
                        {
                            speed = parsedSpeed;

                        }
                    }
                    //UnityEngine.Debug.Log("Parsed Brightness: " + parsedBrightness); // 追加: 解析した輝度を表示
                    
                    
                }
            }
        }

        using (StreamReader reader = pythonProcess.StandardError)
        {
            while (isRunning && !reader.EndOfStream)
            {
                string error = reader.ReadLine();
                if (!string.IsNullOrEmpty(error))
                {
                    UnityEngine.Debug.LogError("Python Error: " + error);
                }
            }
        }
    }

    void Update()
    {
        //UnityEngine.Debug.Log("Playback speed set to: " + prev_speed +"to"+ speed);
        if (videoScript != null && prev_speed != speed)
        {
            videoScript.SetPlaybackSpeed(speed);
            //UnityEngine.Debug.Log("Update: " + speed);
            prev_speed = speed;
        }
    }
     

    public void OnApplicationQuit()
    {
        isRunning = false;

        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }

        if (outputThread != null && outputThread.IsAlive)
        {
            outputThread.Join();
        }
    }
}
