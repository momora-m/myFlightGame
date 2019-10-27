using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class IconEvent2 : MonoBehaviour
{
    //　アイテム情報を出力するテキスト

    private GameObject parentUI;


    void Start()
    {
        parentUI = transform.root.gameObject;
    }

    //　マウスアイコンが自分のアイコン上に入った時
    void OnTriggerEnter2D(Collider2D col)
    {
        CheckEvent(col);
    }

    //　マウスアイコンが自分のアイコン上にいる間
    void OnTriggerStay2D(Collider2D col)
    {
        CheckEvent(col);
    }

    void CheckEvent(Collider2D col)
    {
        Debug.Log("check");
        if (col.tag == "Icon")
        {
           Debug.Log("restart");
           bool submit = CrossPlatformInputManager.GetButton("Decide");
           if (submit)
           {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                    Application.Quit();
#endif
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
