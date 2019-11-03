using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBullet : MonoBehaviour
{

    public int shipType;

    GameObject scoreUI;

    Score score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (shipType == 0)
            {
                scoreUI = GameObject.FindGameObjectWithTag("Score");
                scoreUI.GetComponent<Score>().AddPoint(10);
            }
            if (shipType == 1)
            {
                scoreUI = GameObject.FindGameObjectWithTag("Score");
                scoreUI.GetComponent<Score>().AddPoint(20);
            }
            if (shipType == 2)
            {
                scoreUI = GameObject.FindGameObjectWithTag("Score");
                scoreUI.GetComponent<Score>().AddPoint(50);
            }
        }
    }
}
