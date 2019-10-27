using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class IconEvent : MonoBehaviour
{
    //　アイテム情報を出力するテキスト
    
    private GameObject parentUI;


    void Start()
    {
        parentUI = transform.root.gameObject;
        Debug.Log(parentUI);
    }

    //　マウスアイコンが自分のアイコン上に入った時
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("end");
        CheckEvent(col);
    }

    //　マウスアイコンが自分のアイコン上にいる間
    void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log("end");
        CheckEvent(col);
    }

    void CheckEvent(Collider2D col)
    {
        if (col.tag == "Icon")
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {

                //　このUIをフォーカスする
                EventSystem.current.SetSelectedGameObject(gameObject);
                Debug.Log("check");
            }
        }

        //　アイコンを検知する
        if (col.tag == "Icon")
        {
           Debug.Log("end");
           bool decide = CrossPlatformInputManager.GetButton("Decide");
           if (decide)
           {
              Destroy(parentUI);
              Time.timeScale = 1f;
           }
        }
    }

    //　マウスアイコンが自分のアイコン上から出て行った時
    void OnTriggerExit2D(Collider2D col)
    {

        if (col.tag == "Icon")
        {
            //　フォーカスを解除する
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
