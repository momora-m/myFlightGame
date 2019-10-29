using UnityEngine;

public class playerCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform  m_target = null;
    [SerializeField]
    private float m_speed = 0.0f;

    public Transform Target
    {
        get { return m_target; }
    }

    private Transform  m_cameraTransform = null;
    private Transform  m_pivot = null;

    private void Awake()
    {
        Camera camera   = GetComponentInChildren<Camera>();
        if( camera == null )
        {
            return;
        }

        m_cameraTransform   = camera.transform;
        m_pivot             = m_cameraTransform.parent;
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if( Target == null )
        {
            return;
        }

        Vector3 targetPos   = Target.position;

        float deltaSpeed    = m_speed * Time.deltaTime;
        transform.position  = Vector3.MoveTowards( transform.position, targetPos, deltaSpeed );
    }  
} 