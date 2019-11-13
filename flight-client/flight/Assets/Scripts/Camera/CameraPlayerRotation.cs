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

    private float cosPlayerForward;
    // Start is called before the first frame update
    void Start()
    {
        prevPlayerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));//プレイヤーが向いている向きをXYZ成分に分解して正規化
        cosPlayerForward = calcurateCosPlayer(prevPlayerForward,playerForward);
        Quaternion rotationCamera = Quaternion.LookRotation(playerForward);//機体の向いている方向と同じ向きにカメラを動かす
            transform.rotation = Quaternion.Slerp(//カメラをキャラクターの向いている方向になめらかに動かす
            transform.rotation,
            rotationCamera,
            rotateSpeed * Time.deltaTime
        );
        prevPlayerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));
    }

    private float calcurateCosPlayer(Vector3 prevPlayerForward, Vector3 playerForward) {
        float cosUp = playerForward.x * prevPlayerForward.x + playerForward.y * prevPlayerForward.y + playerForward.z *prevPlayerForward.z;
        float cosDownPrev =  Mathf.Sqrt(playerForward.x*playerForward.x + playerForward.y+playerForward.y + playerForward.z+playerForward.z);
        float cosDownNow = Mathf.Sqrt(prevPlayerForward.x*prevPlayerForward.x + prevPlayerForward.y+prevPlayerForward.y + prevPlayerForward.z+prevPlayerForward.z);
        return cosUp / (cosDownPrev * cosDownNow);
    }
}
