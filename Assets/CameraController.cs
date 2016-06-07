using UnityEngine;
using System.Collections;

public class CameraController : ObjectBase {

	public Transform Target;
    public Vector2 Offset;
    
    protected Camera m_cachedCamera;
    public Camera cachedCamera
    {
        get
        {
            if(m_cachedCamera == null)
                m_cachedCamera = GetComponent<Camera>();
            return m_cachedCamera;
        }
    }
    
    void LateUpdate()
    {
        Vector3 pos = Target.position + (Vector3)Offset;
        pos.z = -10.0f;
        cachedTransform.position = pos;
    }
}
