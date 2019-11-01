using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCamera {
    public class CameraPlayerInput : MonoBehaviour
    {
        private float cameraHorizontal = 0;
        private float cameraVertical = 0;
        public void getControllerInput(float Horizontal, float Vertical, bool isReset) {
            rotateCamera(Horizontal*5,Vertical*5, Horizontal, Vertical, isReset);
        }


        private void rotateCamera(float horizontalRotate, float verticalRotate, float horizontalTransform, float verticalTransform, bool isReset) {
            if(cameraHorizontal >= 360) {
                cameraHorizontal = cameraHorizontal - 360;
            }
            if(cameraHorizontal < 0) {
                cameraHorizontal = 360 - cameraHorizontal;
            }
            cameraHorizontal = cameraHorizontal + horizontalRotate;

            if(cameraVertical >= 180) {
                if(verticalRotate < 0) {
                    cameraVertical = cameraVertical + verticalRotate;
                } 
            }
            else if(cameraVertical < -180) {
                if(verticalRotate > 0) {
                    cameraVertical = cameraVertical + verticalRotate;
                } 
            }
            else {
                cameraVertical = cameraVertical + verticalRotate;
            }
            transform.localRotation = Quaternion.Euler(cameraVertical, cameraHorizontal, 0);

            if(isReset) {
                cameraHorizontal = 0;
                cameraVertical = 0;
            }

        }
    }
}
