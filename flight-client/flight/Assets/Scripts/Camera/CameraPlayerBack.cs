using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerCamera {
    public class CameraPlayerBack : MonoBehaviour
    {

        [SerializeField] public GameObject player;
        [SerializeField] public float scale = 3.0f;
        [SerializeField] public float cameraSpeed = 100f;
        [SerializeField] public float rotateSpeed = 1000f;
        private Vector3 prevPlayerPos;
        private Vector3 posVector;
        private Vector3 currentPlayerPos;
        private Vector3 backVector;
        private Vector3 targetPos;
        private Quaternion rotCam;

        void Start()
        {
            prevPlayerPos = player.transform.position;
            prevPlayerPos.z = player.transform.position.z - 5;
            currentPlayerPos = player.transform.position;
            posVector = (prevPlayerPos - currentPlayerPos).normalized*5;
            prevPlayerPos = player.transform.position;
        }

        void FixedUpdate()
        {
            currentPlayerPos = player.transform.position;
            backVector = (prevPlayerPos - currentPlayerPos).normalized*5;
            posVector = (prevPlayerPos.Round() - currentPlayerPos.Round() == Vector3.zero) ? posVector : backVector;//モデルの都合上微小に動くことは考えられるので、少数値は切り捨てて判断する
            targetPos = currentPlayerPos + scale * posVector;
            targetPos.y = targetPos.y + 1f;
            transform.position = Vector3.Lerp(//線形補間を行うことでプレイヤーが酔わないようなカメラ移動にする
                transform.position,
                targetPos,
                cameraSpeed * Time.deltaTime
            );
            transform.LookAt(player.transform.position);
            prevPlayerPos = player.transform.position;
            //transform.rotation = player.transform.rotation;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                player.transform.rotation,
                rotateSpeed * Time.deltaTime
            );
        }
    }
}
