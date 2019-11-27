using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class CameraPlayerRotation : MonoBehaviour
{
    [SerializeField] public float rotateSpeed = 50f;
    [SerializeField] public GameObject player;
    [SerializeField]public float timeOut;
    [SerializeField] private float minPolarAngle = -90.0f;
    [SerializeField] private float maxPolarAngle = 90.0f;
    [SerializeField] private float horizontalSensitivity = 5.0f;
    [SerializeField] private float verticalSensitivity = 5.0f;
    [SerializeField] private float distance = 5.0f;
    private float timeElapsed;

    private Vector3 playerForward;
    private Vector3 prevPlayerForward;
    private Vector2 sphericalAngleCamera;
    private Vector3 playerOffset;


    private float hoge;
    // Start is called before the first frame update
    void Start()
    {
        prevPlayerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));
        playerOffset = new Vector3(0, 0, -5);
        sphericalAngleCamera = new Vector2(0,0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(CrossPlatformInputManager.GetButton("Cancel") == false) {
            //playerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));//プレイヤーが向いている向きをXYZ成分に分解して正規化
            //Quaternion rotationCamera = Quaternion.LookRotation(playerForward);//機体の向いている方向と同じ向きにカメラを動かす
            //transform.rotation = Quaternion.Slerp(//カメラをキャラクターの向いている方向になめらかに動かす
                //transform.rotation,
                //rotationCamera,
                //rotateSpeed * Time.deltaTime
            //);
            sphericalAngleCamera = new Vector2(0,0);
            transform.position = updatePosition(player.transform.position,sphericalAngleCamera);               
        }
        prevPlayerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));
        if(CrossPlatformInputManager.GetButton("Cancel")) {
            sphericalAngleCamera = updateAngle(CrossPlatformInputManager.GetAxis("HorizontalRight"),  
                                                CrossPlatformInputManager.GetAxis("VerticalRight"),
                                                sphericalAngleCamera);
            transform.position = updatePosition(player.transform.position,sphericalAngleCamera);          
        }
        Debug.Log(sphericalAngleCamera);
        Debug.Log(transform.position);
    }

    private Vector2 updateAngle(float x, float y,Vector2 sphericalAngle)
    {
        Vector2 angle;
        x = sphericalAngle.x - x * horizontalSensitivity;
        angle.x = Mathf.Repeat(x, 360);

        y = sphericalAngle.y + y * verticalSensitivity;
        angle.y = Mathf.Clamp(y, -90, 90);
        return angle;
    }
    private Vector3 updatePosition(Vector3 lookAtPos ,Vector2 sphericalAngle)
    {
        float polarAngle = sphericalAngle.x * Mathf.Deg2Rad;
        float azimuthAngle = sphericalAngle.y * Mathf.Deg2Rad;
        return new Vector3(
            lookAtPos.x + distance * Mathf.Sin(azimuthAngle) * Mathf.Cos(polarAngle),
            lookAtPos.y + distance * Mathf.Cos(azimuthAngle),
            lookAtPos.z + distance * Mathf.Sin(azimuthAngle) * Mathf.Sin(polarAngle));
    }


}
