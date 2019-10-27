using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class LaserSystem : MonoBehaviour
{
    Rigidbody rigid;

    Vector3 velocity;

    Vector3 position;

    public Vector3 acceleration;

    public Transform target;

    float period = 2f;

    private void Start()
    {
        // 初期位置をposionに格納
        position = transform.position;
        // rigidbody取得
        rigid = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {

        var diff = transform.position;

        acceleration = Vector3.zero;
        acceleration += (diff * period) * 2f / (period * period);

        if (acceleration.magnitude > 100f)
        {
            acceleration = acceleration.normalized * 100f;
        }

        period -= Time.deltaTime;

        velocity += acceleration * Time.deltaTime;
    }

    void FixedUpdate()
    {
        bool fireLaser = CrossPlatformInputManager.GetButtonDown("Fire2");
        // 移動処理
        if(fireLaser) {
            rigid.MovePosition(transform.position + velocity * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
