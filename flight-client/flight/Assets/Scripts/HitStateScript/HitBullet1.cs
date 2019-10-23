using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBullet1 : MonoBehaviour
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
            Debug.Log("Hit");
            FindObjectOfType<Score>().AddPoint(30);
        }
    }
}
