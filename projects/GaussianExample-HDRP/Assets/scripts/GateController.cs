using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateController : MonoBehaviour
{
    // void OnTriggerEnter(Collider other)
    // {
    //     SceneManager.LoadScene("NU");
    // }

    // void OnCollisionEnter(Collision collisionInfo)
    // {
    //     SceneManager.LoadScene("NU_syokudomae");
    // }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene("NU_syokudomae");
        }
    }
}
