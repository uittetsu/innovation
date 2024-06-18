using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    public string nextSceneName;
    void OnCollisionStay(Collision collisionInfo)
    {
        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
