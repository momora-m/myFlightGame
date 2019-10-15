using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameSystem : MonoBehaviour
{

    public GameObject prefabMenuUI;

    private GameObject instanceMenuUI;

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
            if (instanceMenuUI == null)
            {
                instanceMenuUI = GameObject.Instantiate(prefabMenuUI) as GameObject;
                Time.timeScale = 0f;
            }
            else
            {
                Destroy(instanceMenuUI);
                Time.timeScale = 1f;
            }
        }
    }
}
