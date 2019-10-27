using System;
using UnityEngine;


namespace UnityStandardAssets.Cameras
{
    public abstract class PivotBasedCameraRig : AbstractTargetFollower
    {
        // このスクリプトは、カメラリグのルートオブジェクトに配置されるように設計されています
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        protected Transform m_Cam; // カメラの位置
        protected Transform m_Pivot; // カメラが回転する位置
        protected Vector3 m_LastTargetPosition;


        protected virtual void Awake()
        {
            // find the camera in the object hierarchy
            m_Cam = GetComponentInChildren<Camera>().transform;
            m_Pivot = m_Cam.parent;
        }
    }
}
