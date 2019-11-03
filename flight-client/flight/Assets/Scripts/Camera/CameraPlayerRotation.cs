using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerRotation : MonoBehaviour
{
    [SerializeField] public float rotateSpeed = 50f;
    [SerializeField] public GameObject player;
    [SerializeField]public float timeOut;
    private float timeElapsed;

    private Vector3 playerForward;
    private Vector3 prevPlayerForward;
    // Start is called before the first frame update
    void Start()
    {
        prevPlayerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));//プレイヤーが向いている向きをXYZ成分に分解して正規化
        Debug.Log(playerForward);
        //if(playerForward.Round() - prevPlayerForward.Round() != Vector3.zero)
        //{
            Quaternion rotationCamera = Quaternion.LookRotation(playerForward);//機体の向いている方向と同じ向きにカメラを動かす
            transform.rotation = Quaternion.Slerp(//カメラをキャラクターの向いている方向になめらかに動かす
                transform.rotation,
                rotationCamera,
                rotateSpeed * Time.deltaTime
            );
        //}
        prevPlayerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));
    }
}
