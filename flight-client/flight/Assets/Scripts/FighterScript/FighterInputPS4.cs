using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Fighter// 戦闘機周りはこの名前空間で統一
{
    public class FighterInputPS4 : MonoBehaviour//R2とL2の入力の関係上PS4コントローラーでしか動作しません！Fuck
    {

        private FighterController m_Aeroplane;


        private void Awake()
        {
            // 戦闘機モデルにアタッチされたFighterControllerを取得する
            m_Aeroplane = GetComponent<FighterController>();
        }


        private void FixedUpdate()
        {
            //ロールピッチヨーとエンジンスロットルの入力をとる
            float roll = CrossPlatformInputManager.GetAxis("Horizontal");
            float pitch = CrossPlatformInputManager.GetAxis("Vertical");
            float yaw1 = CrossPlatformInputManager.GetAxis("YawS1");
            float yaw2 = CrossPlatformInputManager.GetAxis("YawS2");
            float cameraHorizontal = CrossPlatformInputManager.GetAxis("HorizontalRight");
            float cameraVertical = CrossPlatformInputManager.GetAxis("VerticalRight");
            //bool airBrakes = CrossPlatformInputManager.GetButton("Fire1");
            bool airEngines = CrossPlatformInputManager.GetButton("RightB");
            bool airBrakes = CrossPlatformInputManager.GetButton("LeftB");
            // auto throttle up, or down if braking.
            float throttle = 0;

            bool isAutoPilot = false; // ヨーの左右同時入力時、オートパイロットをオンにする
            bool isPitchup = false; //ピッチアップしているときは、空力とか無視したい
            if(pitch < 0) {
                isPitchup = true;
            }
            //float throttle = airBrakes ? -1 : 1;
            if (airEngines == true)
            {
                throttle = 1;
            }
            if (airBrakes == true)
            {
                throttle = -1;
            }

            // Pass the input to the aeroplane
            //現状PS4コントローラにのみ対応!
            // TODO いつか改善したい
            if (yaw1 < 0 && yaw2 < 0)
            {
                m_Aeroplane.Move(roll, pitch, 0, throttle, airBrakes);
                m_Aeroplane.SetFighterStatus(isAutoPilot, isPitchup);
            }
            if (yaw1 >= 0 && yaw2 < 0)
            {
                m_Aeroplane.Move(roll, pitch, -yaw1, throttle, airBrakes);
                m_Aeroplane.SetFighterStatus(isAutoPilot, isPitchup);
            }
            if (yaw2 >= 0 && yaw1 < 0)
            {
                m_Aeroplane.Move(roll, pitch, yaw2, throttle, airBrakes);
                m_Aeroplane.SetFighterStatus(isAutoPilot, isPitchup);
            }
            if (yaw1 >= 0 && yaw2 >= 0)
            {
                isAutoPilot = true; 
                m_Aeroplane.Move(roll, pitch, 0, throttle, airBrakes);
                m_Aeroplane.SetFighterStatus(isAutoPilot, isPitchup);
            }


        }
    }
}
