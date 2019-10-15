using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MoveTitle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool start = CrossPlatformInputManager.GetButtonDown("Submit");
        if (start)
        {
            Debug.Log("start");
            SceneManager.LoadScene("Title");
        }
    }
}
