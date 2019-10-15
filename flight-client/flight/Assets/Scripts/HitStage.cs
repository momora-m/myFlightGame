using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HitStage : MonoBehaviour
{
    public GameObject explode;

    private int flg = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stage" || collision.gameObject.tag == "Enemy")
        {
            Instantiate(explode, this.transform.position, Quaternion.identity);
            Debug.Log("hit");
            StartCoroutine(DelayMethod());

        }
    }


    IEnumerator DelayMethod()
    {
        Debug.Log("coroutine");
        yield return new WaitForSeconds(1.0f);
        ChangeScene();
    }

    void ChangeScene()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene("gameover");

    }
}
