using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerBack : MonoBehaviour
{

    [SerializeField] public GameObject player;
    [SerializeField] public float scale = 3.0f;
    [SerializeField] public float cameraSpeed = 100f;
    private Vector3 prevPlayerPos;
    private Vector3 posVector;
    private Vector3 currentPlayerPos;
    private Vector3 backVector;
    private Vector3 targetPos;
    private Vector3 hoge;

    void Start()
    {
        prevPlayerPos = player.transform.position;
        prevPlayerPos.z = player.transform.position.z - 5;
        currentPlayerPos = player.transform.position;
        posVector = (prevPlayerPos - currentPlayerPos).normalized*5;
        prevPlayerPos = player.transform.position;
        Debug.Log(posVector);
    }

    void FixedUpdate()
    {
        currentPlayerPos = player.transform.position;
        Debug.Log(currentPlayerPos);
        Debug.Log(prevPlayerPos);
        backVector = (prevPlayerPos - currentPlayerPos).normalized*5;
        posVector = (prevPlayerPos.Round() - currentPlayerPos.Round() == Vector3.zero) ? posVector : backVector;//モデルの都合上微小に動くことは考えられるので、少数値は切り捨てて判断する
        Debug.Log("backVector" + backVector);
        targetPos = currentPlayerPos + scale * posVector;
        targetPos.y = targetPos.y + 1f;
        this.transform.position = Vector3.Lerp(
            this.transform.position,
            targetPos,
            cameraSpeed * Time.deltaTime
        );
        this.transform.LookAt(player.transform.position);
        prevPlayerPos = player.transform.position;
    }
}
