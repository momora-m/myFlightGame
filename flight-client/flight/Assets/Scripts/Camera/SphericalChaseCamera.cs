using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SphericalChaseCamera : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset; // offset form the target object

    [SerializeField] private float distance = 4.0f; // distance from following object
    [SerializeField] private float minDistance = 1.0f;
    [SerializeField] private float maxDistance = 7.0f;
    [SerializeField] private float minPolarAngle = 5.0f;
    [SerializeField] private float maxPolarAngle = 75.0f;
    [SerializeField] private Vector3 playerForward;
    [SerializeField] private Vector3 playerOffset; 
    private Vector2 sphericalAnglePlayer;

    void Start() {
        sphericalAnglePlayer.x = 45.0f;
        sphericalAnglePlayer.y = 45.0f;
        playerOffset = new Vector3(0,0,-10);
    }
    void FixedUpdate() {
        playerForward = Vector3.Scale(player.transform.forward, new Vector3(10, 10, 10));//プレイヤーが向いている向きをXYZ成分に分解して正規化
        sphericalAnglePlayer = SphericalSystem.calcurateSphericalAngle(player.transform.rotation, playerForward);
        Vector3 lookAtPos = player.transform.forward + offset;
        transform.position = playerOffset + updatePosition(lookAtPos, sphericalAnglePlayer);
        transform.LookAt(lookAtPos + playerOffset);
    }

    private Vector3 updatePosition(Vector3 lookAtPos, Vector2 sphericalAngle)
    {
        float polarAngle = sphericalAngle.x * Mathf.PI;
        float azimuthAngle = sphericalAngle.y * Mathf.PI;
        return new Vector3(
            lookAtPos.x + distance * Mathf.Sin(azimuthAngle) * Mathf.Cos(polarAngle),
            lookAtPos.z + distance * Mathf.Cos(azimuthAngle),
            lookAtPos.y + distance * Mathf.Sin(azimuthAngle) * Mathf.Sin(polarAngle));
    }
}
