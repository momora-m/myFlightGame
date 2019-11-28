using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerMove : MonoBehaviour
{
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = player.transform.rotation;
        Vector3 position = player.transform.position;
        position.z  = player.transform.position.z - 10;
        transform.position = position;
    }
}
