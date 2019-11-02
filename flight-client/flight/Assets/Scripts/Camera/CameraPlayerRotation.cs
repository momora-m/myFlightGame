using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerRotation : MonoBehaviour
{
    [SerializeField] public float rotateSpeed = 50f;
    [SerializeField] public GameObject player;

    private Vector3 playerForward;
    // Start is called before the first frame update
    void Start()
    {
        playerForward = Vector3.Scale(player.transform.forward, new Vector3(1, 1, 1));//プレイヤーが向いている向きの正規化
    }

    // Update is called once per frame
    void LateUpdate()
    {
        playerForward = Vector3.Scale(player.transform.forward, new Vector3(1, 1, 1));//プレイヤーが向いている向きをXYZ成分に分解して正規化
        Quaternion rotationCamera = Quaternion.LookRotation(playerForward);//機体の向いている方向と同じ向きにカメラを動かす
        transform.rotation = Quaternion.Slerp(//カメラをキャラクターの向いている方向になめらかに動かす
            transform.rotation,
            rotationCamera,
            rotateSpeed * Time.deltaTime
        );
    }
}
