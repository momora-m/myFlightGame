using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
namespace PlayerCamera {
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] public GameObject target; // an object to follow
        [SerializeField] public float cameraSpeed = 100f;
        [SerializeField] public float rotateSpeed = 1000f;

        [SerializeField] private float distance = 4.0f; // distance from following object
        private float cameraVertical = 0; // angle with verticalRotate-axis
        private float cameraHorizontal = 0; // angle with horizontalRotate-axis

        private Vector3 prevPlayerPos;
        private Vector3 posVector;
        private Vector3 currentPlayerPos;
        private Vector3 backVector;
        private Vector3 targetPos;

        void start()
        {
            prevPlayerPos = target.transform.position;
            prevPlayerPos.z = target.transform.position.z - 5;
            currentPlayerPos = target.transform.position;
            posVector = (prevPlayerPos - currentPlayerPos).normalized*5;
            prevPlayerPos = target.transform.position;
        }
        void FixedUpdate()
        {
            float cameraHorizontalInput = CrossPlatformInputManager.GetAxis("HorizontalRight");
            float cameraVerticalInput = CrossPlatformInputManager.GetAxis("VerticalRight");
            updateAngle(cameraHorizontalInput*5 , cameraVerticalInput*5 );
            currentPlayerPos = target.transform.position;
            backVector = (prevPlayerPos - currentPlayerPos).normalized*5;
            posVector = (prevPlayerPos.Round() - currentPlayerPos.Round() == Vector3.zero) ? posVector : backVector;//モデルの都合上微小に動くことは考えられるので、少数値は切り捨てて判断する
            targetPos = currentPlayerPos + posVector;
            targetPos.y = targetPos.y + 1f;
            Vector3 Temp;
            var da = cameraHorizontal * Mathf.Deg2Rad;
            var dp = cameraVertical * Mathf.Deg2Rad;
            Temp = new Vector3(
                targetPos.x + distance * Mathf.Sin(dp) * Mathf.Cos(da),
                targetPos.y + distance * Mathf.Cos(dp),
                targetPos.z + distance * Mathf.Sin(dp) * Mathf.Sin(da));
            transform.position = Vector3.Lerp(//線形補間を行うことでプレイヤーが酔わないようなカメラ移動にする
                transform.position,
                targetPos,
                cameraSpeed * Time.deltaTime
            );
            transform.LookAt(targetPos);
            updatePosition(targetPos);
            prevPlayerPos = target.transform.position;
            // targetの位置のY軸を中心に、回転（公転）する
            transform.RotateAround(currentPlayerPos, Vector3.up, cameraHorizontalInput * Time.deltaTime *200f);
            // カメラの垂直移動（※角度制限なし、必要が無ければコメントアウト）
            transform.RotateAround(targetPos, Vector3.right, cameraVerticalInput * Time.deltaTime * 200f);
            Debug.Log(cameraHorizontal);
        }

        void updateAngle(float horizontalRotate, float verticalRotate)
        {
                if(cameraHorizontal >= 3600) {
                    cameraHorizontal = cameraHorizontal - 360;
                }
                if(cameraHorizontal < 0) {
                    cameraHorizontal = 360 - cameraHorizontal;
                }
                cameraHorizontal = cameraHorizontal + horizontalRotate;

                if(cameraVertical >= 80) {
                    if(verticalRotate < 0) {
                        cameraVertical = cameraVertical + verticalRotate;
                    } 
                }
                else if(cameraVertical < -80) {
                    if(verticalRotate > 0) {
                        cameraVertical = cameraVertical + verticalRotate;
                    } 
                }
                else {
                    cameraVertical = cameraVertical + verticalRotate;
                }
        }
        void updatePosition(Vector3 lookAtPos)
        {

        }
    }
}