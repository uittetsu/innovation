using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate_Toyota2Stone : MonoBehaviour
{
    void OnCollisionStay(Collision collisionInfo)
    {
        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene("Stone");
        }
    }
}
