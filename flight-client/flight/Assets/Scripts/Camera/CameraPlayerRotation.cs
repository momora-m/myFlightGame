using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class CameraPlayerRotation : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private GameObject player;
    [SerializeField] private float horizontalSensitivity = 5.0f;
    [SerializeField] private float verticalSensitivity = 5.0f;
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private float scale = 3.0f;
    [SerializeField] private float cameraSpeed = 100f;
    private float timeElapsed;
    private float rollAngle;
    private float pitchAngle;

    private Vector3 playerForward;
    private Vector3 prevPlayerForward;
    private Vector2 sphericalAngleCamera;


    private float hoge;
    // Start is called before the first frame update
    void Start()
    {
        sphericalAngleCamera = new Vector2(90,-90);
        Debug.Log(transform.rotation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float roll = CrossPlatformInputManager.GetAxis("Horizontal");
        float pitch = CrossPlatformInputManager.GetAxis("Vertical"); 
        if(CrossPlatformInputManager.GetAxis("HorizontalRight") == 0 && CrossPlatformInputManager.GetAxis("VerticalRight") == 0) {
            sphericalAngleCamera = new Vector2(90,-90);
        }
        sphericalAngleCamera = updateAngle(CrossPlatformInputManager.GetAxis("HorizontalRight"),  
                                            CrossPlatformInputManager.GetAxis("VerticalRight"),
                                            sphericalAngleCamera);        
        Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        transform.localPosition = updatePosition(player.transform.localPosition,sphericalAngleCamera);
        transform.LookAt(target);
        //transform.rotation = player.transform.rotation;
    }

    private Vector2 updateAngle(float x, float y,Vector2 sphericalAngle)
    {
        Vector2 angle;
        x = sphericalAngle.x - x * horizontalSensitivity;
        angle.x = Mathf.Clamp(x, 0, 180);

        y = sphericalAngle.y + y * verticalSensitivity;
        angle.y = Mathf.Clamp(y, -180, 0);
        return angle;
    }

    private void localLookAtPos(Vector3 pos, Vector3 targetPos) {
        Vector3 z = (targetPos - transform.position).normalized;
        Vector3 x = Vector3.Cross(Vector3.up, z).normalized;
        Vector3 y = Vector3.Cross(z, x).normalized;
    }
    private void CalculateRollAndPitchAngles()//ロールとピッチの角度計算を行う
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0;
        //前方ベクトルゼロ以外のとき
        if (flatForward.sqrMagnitude > 0)
        {
            flatForward.Normalize();//ベクトルを正規化する
            // ピッチの角度を計算
            Vector3 localFlatForward = transform.InverseTransformDirection(flatForward);//ワールド座標からローカル座標への変換
            pitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);//ローカル座標を用いて迎え角を計算する
            // ロールの角度を計算
            Vector3 flatRight = Vector3.Cross(Vector3.up, flatForward);//外積を求めることで、右の角度か左の角度かを判断する
            Vector3 localFlatRight = transform.InverseTransformDirection(flatRight);//ワールド座標からローカル座標への変換
            rollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);//ローカル座標を用いて方位角を計算する
        }
    }

    private void calcuratePlayerRotation(){
    }
    private Vector3 updatePosition(Vector3 targetPos, Vector2 sphericalAngle)
    {
        float polarAngle = sphericalAngle.x * Mathf.Deg2Rad;
        float azimuthAngle = sphericalAngle.y * Mathf.Deg2Rad;
        Vector3 worldposition = new Vector3(
            targetPos.x + distance * Mathf.Sin(azimuthAngle) * Mathf.Cos(polarAngle),
            targetPos.y + distance * Mathf.Cos(azimuthAngle),
            targetPos.z + distance * Mathf.Sin(azimuthAngle) * Mathf.Sin(polarAngle)
        );
        return worldposition;
    }


}
