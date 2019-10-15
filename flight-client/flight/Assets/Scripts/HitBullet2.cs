using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBullet2 : MonoBehaviour
{
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
            Debug.Log("Hit2");
            FindObjectOfType<Score>().AddPoint(50);
        }
    }
}
