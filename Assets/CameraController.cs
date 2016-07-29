using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoSingleton<CameraController> {

    public Vector2 Offset;

    public float Speed = 2.6f;
    
    
    protected Camera m_cachedCamera;

    public const float Size = 3.636f;

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
    }

    void LateUpdate()
    {
        List<CharacterPlayer> players = GameManager.Instance.PlayerList;
        Transform target = players[0].cachedTransform;

        for(int i = 1, c = players.Count; i < c; ++i )
        {
            if(target.position.x * GameManager.Instance.Direction < players[i].cachedTransform.position.x * GameManager.Instance.Direction)
            {
                target = players[i].cachedTransform;
            }
        }

        m_fScale = Mathf.Lerp(m_fScale, TargetScale, Speed * Time.smoothDeltaTime);
        m_cachedCamera.orthographicSize = Size * Scale;

        Vector3 centor = GameManager.Instance.cachedTransform.position;
        Vector3 pos = new Vector3(target.position.x, 0.0f, 0.0f) + (Vector3)Offset * Scale + m_vecShake;
        m_vecShake = Vector3.zero;
        float margin = 0.5f;
        float limit = margin + 10.0f - Size * Scale * m_cachedCamera.aspect;
        pos.x = Mathf.Clamp(pos.x, centor.x - limit, centor.x + limit);
        pos = Vector2.Lerp(cachedTransform.position, pos, Speed * Time.smoothDeltaTime);
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
    }
}
