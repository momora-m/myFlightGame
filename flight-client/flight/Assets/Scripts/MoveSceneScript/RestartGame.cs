using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void pushButton()
    {
        GameObject parentUI;
        parentUI = transform.root.gameObject;
        Destroy(parentUI);
        Time.timeScale = 1f;
    }
}
