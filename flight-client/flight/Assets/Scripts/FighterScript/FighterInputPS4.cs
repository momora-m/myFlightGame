﻿using System.Collections;
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
            //bool airBrakes = CrossPlatformInputManager.GetButton("Fire1");
            bool airEngines = CrossPlatformInputManager.GetButton("RightB");
            bool airBrakes = CrossPlatformInputManager.GetButton("LeftB");
            // auto throttle up, or down if braking.
            float throttle = 0;
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
            if (yaw1 < 0 && yaw2 < 0)
            {
                m_Aeroplane.Move(roll, pitch, 0, throttle, airBrakes);
            }
            if (yaw1 >= 0 && yaw2 < 0)
            {
                m_Aeroplane.Move(roll, pitch, -yaw1, throttle, airBrakes);
            }
            if (yaw2 >= 0 && yaw1 < 0)
            {
                m_Aeroplane.Move(roll, pitch, yaw2, throttle, airBrakes);
            }
            if (yaw1 >= 0 && yaw2 >= 0)
            {
                m_Aeroplane.Move(roll, pitch, 0, throttle, airBrakes);
            }


        }
    }
}
