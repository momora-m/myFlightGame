using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Fighter// 戦闘機周りはこの名前空間で統一
{
    public class FighterInputPS4 : MonoBehaviour//R2とL2の入力の関係上PS4コントローラーでしか動作しません！Fuck
    {

        private FighterController m_Fighter;


        private void Awake()
        {
            // 戦闘機モデルにアタッチされたFighterControllerを取得する
            m_Fighter = GetComponent<FighterController>();
        }


        private void FixedUpdate()
        {
            //ロールピッチヨーとエンジンスロットルの入力をとる
            float roll = CrossPlatformInputManager.GetAxis("Horizontal");
            float pitch = CrossPlatformInputManager.GetAxis("Vertical");
            bool yaw1 = CrossPlatformInputManager.GetButton("YawS1");
            bool yaw2 = CrossPlatformInputManager.GetButton("YawS2");
            //bool airBrakes = CrossPlatformInputManager.GetButton("Fire1");
            float airEngines = CrossPlatformInputManager.GetAxis("RightB");
            float airBrakes = CrossPlatformInputManager.GetAxis("LeftB");
            // auto throttle up, or down if braking.
            float throttle = 0;
            float rightYaw = 0;
            float leftYaw = 0;
            bool isAutoPilot = false; // ヨーの左右同時入力時、オートパイロットをオンにする
            bool isPitchup = false; //ピッチアップしているときは、空力とか無視したい
            bool isAirBrakes = false;
            if(pitch < 0) {
                isPitchup = true;
            }
            //float throttle = airBrakes ? -1 : 1;
            if(airEngines > 0) {
                throttle = 1;
                isAirBrakes = false;
            }
            else if(airBrakes > 0) {
                throttle = -1;
                isAirBrakes = true;
            }

            if(yaw1) {
                rightYaw = 1;
            }
            else {
                rightYaw = 0;
            }

            if(yaw2) {
                leftYaw = 1;
            }
            else {
                leftYaw = 0;
            }

            // Pass the input to the aeroplane
            //現状PS4コントローラにのみ対応!
            // TODO いつか改善したい
            if (rightYaw == 0 && leftYaw == 0)
            {
                m_Fighter.MoveFighter(roll, pitch, 0, throttle, isAirBrakes);
                m_Fighter.SetFighterStatus(isAutoPilot, isPitchup);
            }
            if (rightYaw == 1 && leftYaw == 0)
            {
                m_Fighter.MoveFighter(roll, pitch, rightYaw, throttle, isAirBrakes);
                m_Fighter.SetFighterStatus(isAutoPilot, isPitchup);
            }
            if (leftYaw == 1 && rightYaw == 0)
            {
                m_Fighter.MoveFighter(roll, pitch, -leftYaw, throttle, isAirBrakes);
                m_Fighter.SetFighterStatus(isAutoPilot, isPitchup);
            }
            if (rightYaw == 1 && leftYaw == 1)
            {
                isAutoPilot = true; 
                m_Fighter.MoveFighter(roll, pitch, 0, throttle, isAirBrakes);
                m_Fighter.SetFighterStatus(isAutoPilot, isPitchup);
            }


        }
    }
}
