using UnityEngine;
using System.Collections;

public class CameraController : ObjectBase {

	public Transform Target;
    public Vector2 Offset;
    
    protected Camera m_cachedCamera;

    public const float Size = 6.4f;

    protected float m_fScale = 1.0f;
    public float Scale
    {
        get { return m_fScale; }
    }

    public float TargetScale;

    public Camera cachedCamera
    {
        get
        {
            return m_cachedCamera;
        }
    }
    
    void Awake()
    {
        m_cachedCamera = GetComponent<Camera>();

        Init();
    }

    void Init()
    {
        m_fScale = 1.0f;
        TargetScale = 1.0f;
        m_cachedCamera.orthographicSize = Size * Scale;
    }

    void LateUpdate()
    {
        m_fScale = Mathf.Lerp(m_fScale, TargetScale, 0.995f * Time.smoothDeltaTime);
        m_cachedCamera.orthographicSize = Size * Scale;

        Vector3 pos = Target.position + (Vector3)Offset * Scale;
        pos.z = -10.0f;
        cachedTransform.position = pos;

    }
}
