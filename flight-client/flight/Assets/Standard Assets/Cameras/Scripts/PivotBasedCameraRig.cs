using System;
using UnityEngine;


namespace UnityStandardAssets.Cameras
{
    public abstract class PivotBasedCameraRig : AbstractTargetFollower
    {
        // ���̃X�N���v�g�́A�J�������O�̃��[�g�I�u�W�F�N�g�ɔz�u�����悤�ɐ݌v����Ă��܂�
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        protected Transform m_Cam; // �J�����̈ʒu
        protected Transform m_Pivot; // �J��������]����ʒu
        protected Vector3 m_LastTargetPosition;


        protected virtual void Awake()
        {
            // find the camera in the object hierarchy
            m_Cam = GetComponentInChildren<Camera>().transform;
            m_Pivot = m_Cam.parent;
        }
    }
}
