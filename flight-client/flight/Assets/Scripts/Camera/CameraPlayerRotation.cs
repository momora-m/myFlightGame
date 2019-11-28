using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class CameraPlayerRotation : MonoBehaviour
{
    [SerializeField] public float rotateSpeed = 50f;
    [SerializeField] public GameObject player;
    [SerializeField]public float timeOut;
    [SerializeField] private float minPolarAngle = 0;
    [SerializeField] private float maxPolarAngle = 90.0f;
    [SerializeField] private float horizontalSensitivity = 5.0f;
    [SerializeField] private float verticalSensitivity = 5.0f;
    [SerializeField] private float distance = 5.0f;
    private float timeElapsed;

    private Vector3 playerForward;
    private Vector3 prevPlayerForward;
    private Vector2 sphericalAngleCamera;


    private float hoge;
    // Start is called before the first frame update
    void Start()
    {
        prevPlayerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));
        sphericalAngleCamera = new Vector2(90,-90);
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
        transform.position = updatePosition(player.transform.position,sphericalAngleCamera);          
        transform.LookAt(player.transform.position);
        Debug.Log(CrossPlatformInputManager.GetAxis("HorizontalRight"));
        Debug.Log(CrossPlatformInputManager.GetAxis("VerticalRight"));
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

    private void calcuratePlayerRotation(){
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
