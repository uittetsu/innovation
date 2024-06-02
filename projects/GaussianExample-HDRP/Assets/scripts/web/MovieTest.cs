using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MovieTest : MonoBehaviour
{
    [SerializeField]MovieImage movieImage = null;
    [SerializeField]VideoClip videoClip = null;

    public void Start()
    {   // 動画を再生する
        movieImage.Play(videoClip);
    }
}
