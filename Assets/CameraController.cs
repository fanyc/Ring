using UnityEngine;
using System.Collections;

public class CameraController : MonoSingleton<CameraController> {

	public Transform Target;
    public Vector2 Offset;

    public float Speed = 2.6f;
    
    
    protected Camera m_cachedCamera;

    public const float Size = 3.636f;

    protected float m_fScale = 1.0f;
    protected Vector2 m_offset;
    
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

    protected Vector3 m_vecShake;
    
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
        m_vecShake = Vector3.zero;
        m_offset = Offset;
    }

    void LateUpdate()
    {
        m_fScale = Mathf.Lerp(m_fScale, TargetScale, Speed * Time.smoothDeltaTime);
        m_cachedCamera.orthographicSize = Size * Scale;

        m_offset = Vector2.Lerp(m_offset, Offset, Speed * Time.smoothDeltaTime);
        Vector3 pos = Target.position + (Vector3)m_offset * Scale + m_vecShake;
        m_vecShake = Vector3.zero;
        pos.z = -10.0f;
        cachedTransform.position = pos;

    }

    public void SetShake(float amplitude, float waveLength, float duration)
    {
        StartCoroutine(_shake(amplitude, waveLength, duration));
    }

    IEnumerator _shake(float amplitude, float waveLength, float duration)
    {
        float per = 0.0f;
        while(per < 1.0f)
        {
            m_vecShake += new Vector3(0.0f, (float)(Mathf.Sin(Mathf.PI * per * (1.0f / waveLength)) * amplitude * (1.0f - per)));
            per += Time.smoothDeltaTime / duration;
            yield return null;
        }

        m_vecShake = Vector3.zero;
    }
}
