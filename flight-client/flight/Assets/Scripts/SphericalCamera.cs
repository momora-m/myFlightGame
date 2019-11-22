using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class SphericalCamera : MonoBehaviour {

    [SerializeField] private GameObject player;
    [SerializeField] private float cameraSpeed = 100f;
    [SerializeField] private float spinSpeed = 1.0f;

    private Vector3 preCameraPos;
    private Vector3 cameraPos;
    private Vector3 cameraTargetPos;
    private Vector2 inputController = Vector2.zero;
    private Vector2 preInputController = Vector2.zero;
    private float radiusCamera = 5f;
    void Start(){
        preCameraPos = transform.position;
        cameraTargetPos = player.transform.position;
    }
    void FixedUpdate() {
        
        inputController += new Vector2(CrossPlatformInputManager.GetAxis("HorizontalRight"), 
                            CrossPlatformInputManager.GetAxis("VerticalRight")) * Time.deltaTime * spinSpeed;
        //inputController.x %= 360f;
        //inputController.y %= 360f;
        //inputController.y = Mathf.Clamp(inputController.y, -0.3f + 0.5f, 0.3f + 0.5f);
        cameraPos = calcurateSphericalPosition(inputController,radiusCamera);
        // r and upper
        cameraPos.z += preCameraPos.z;

        cameraPos.y += preCameraPos.y;
        //cameraPos.x += preCameraPos.x; // if u need a formula,pls remove comment tag.

        transform.position = cameraPos + cameraTargetPos;
        transform.LookAt(cameraTargetPos);
        //transform.eulerAngles = new Vector3(inputController.y, inputController.x-180f, 0);
        cameraTargetPos = player.transform.position;
    }

    private float calcurateRadius(Vector3 pos) {
        return Mathf.Sqrt(pos.x*pos.x+pos.y*pos.y+pos.z*pos.z);
    }

    private Vector3 calcurateSphericalPosition(Vector2 input, float radius) {
        Vector3 pos;
        pos.x = radius * Mathf.Sin(input.y * Mathf.PI) * Mathf.Cos(input.x * Mathf.PI);
        pos.y = radius * Mathf.Cos(input.y * Mathf.PI);
        pos.z = radius * Mathf.Sin(input.y * Mathf.PI) * Mathf.Sin(input.x * Mathf.PI) ;
        return pos;
    }

    
}