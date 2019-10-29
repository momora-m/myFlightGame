using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace PlayerCamera{
public class CameraController : MonoBehaviour
{
    private CameraPlayerInput moveCamera;
        // Start is called before the first frame update
        void Start()
        {
            moveCamera = GetComponent<CameraPlayerInput>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float cameraHorizontal = CrossPlatformInputManager.GetAxis("HorizontalRight");
            float cameraVertical = CrossPlatformInputManager.GetAxis("VerticalRight");
            bool cameraReset = CrossPlatformInputManager.GetButton("Reset");
            moveCamera.getControllerInput(cameraHorizontal,cameraVertical,cameraReset);

        }
    }
}
