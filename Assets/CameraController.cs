using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : MonoSingleton<CameraController> {
    public static List<Background> ListBackground = new List<Background>();
    protected static float m_fBackgroundFade = 1.0f;
    protected static float BackgroundFade
    {
        get
        {
            return m_fBackgroundFade;
        }

        set
        {
            m_fBackgroundFade = value;
            Color c = new Color(m_fBackgroundFade, m_fBackgroundFade, m_fBackgroundFade, 1.0f);
            for(int i = 0; i < ListBackground.Count; ++i)
            {
                ListBackground[i].SetColor(c);
            }
        }
    }
    public Vector2 Offset;

    public float Speed = 5.2f;
    
    
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

    protected Transform m_tfTarget;
    protected Vector3 m_position;
    protected Vector3 m_vecShake;

    protected delegate void ZoomProcess(ref float scale);
    protected ZoomProcess m_zoomProc;

    protected delegate void PositionProcess(ref Vector3 position);
    protected PositionProcess m_followProc;
    protected PositionProcess m_SetPositionProc;
    protected PositionProcess m_PostPositionProc;

    protected int m_nFadeEffectCount = 0;
    
    
    
    protected float m_fZoom = 1.0f;

    void Awake()
    {
        m_cachedCamera = GetComponent<Camera>();

        Init();
    }

    void Init()
    {
        m_fScale = 1.0f;
        TargetScale = 1.0f;
        m_fZoom = 1.0f;
        m_tfTarget = null;
        m_cachedCamera.orthographicSize = Size * Scale;
        m_vecShake = Vector3.zero;
        m_position = cachedTransform.position;

        m_SetPositionProc = SetPositionLerp;

        m_fBackgroundFade = 1.0f;

        m_nFadeEffectCount = 0;
    }

    void LateUpdate()
    {
        float scale = m_fScale;
        if(m_zoomProc != null)
        {
            m_zoomProc(ref scale);
            
        }
        else
        {
            DefaultZoom(ref scale);
        }

        if(m_fScale != scale)
        {
            m_fScale = scale;
            m_cachedCamera.orthographicSize = Size * Scale;
        }

        Vector3 pos = m_position;
        
        if(m_followProc != null)
        {
            m_followProc(ref pos);
        }
        else
        {
            DefaultFollow(ref pos);
        }

        if(m_SetPositionProc != null)
        {
            m_SetPositionProc(ref pos);
        }

        Vector3 centor = GameManager.Instance.cachedTransform.position;
        float margin = 0.5f;
        float limit = margin + GameManager.Instance.LimitDistance - Size * Scale * m_cachedCamera.aspect;
        pos.x = Mathf.Clamp(pos.x, centor.x - limit, centor.x + limit);
        pos.z = -10.0f;

        bool isPositionUpdate = false;
        if(m_position.Equals(pos) == false)
        {
            m_position = pos;
            isPositionUpdate = true;
        }

        if(m_PostPositionProc != null)
        {
            m_PostPositionProc(ref pos);
            isPositionUpdate = true;
        }

        if(isPositionUpdate)
        {
            cachedTransform.position = pos;
        }
    }

    void DefaultZoom(ref float scale)
    {
        scale = Mathf.Lerp(scale, TargetScale, Speed * Time.smoothDeltaTime);
    }

    void DefaultFollow(ref Vector3 position)
    {
        Transform target = m_tfTarget;

        if(target == null)
        {
            List<CharacterPlayer> players = CharacterPlayer.PlayerList;
            target = players[0].cachedTransform;

            for(int i = 1, c = players.Count; i < c; ++i )
            {
                if(target.position.x * GameManager.Instance.Direction < players[i].cachedTransform.position.x * GameManager.Instance.Direction)
                {
                    target = players[i].cachedTransform;
                }
            }
        }

        position = new Vector3(target.position.x, 0.0f, 0.0f) + (Vector3)Offset * Scale;
    }

    void SetPositionLerp(ref Vector3 position)
    {
        position = Vector2.Lerp(m_position, position, Speed * Time.smoothDeltaTime);
    }

    public void SetShake(float amplitude, float waveLength, float duration)
    {
        float per = 0.0f;
        PositionProcess proc = null;
        proc = (ref Vector3 position)=>
        {
            position += new Vector3(0.0f, (float)(Mathf.Sin(Mathf.PI * per * (1.0f / waveLength)) * amplitude * (1.0f - per)));
            per += Time.smoothDeltaTime / duration;
            if(per >= 1.0f)
            {
                m_PostPositionProc -= proc;
            }
        };

        m_PostPositionProc += proc;
    }

    public void SetZoom(float zoom)
    {
        m_fZoom = 1.0f / zoom;
    }
    public void SetZoom(float zoom, float duration)
    {
        SetZoom(zoom);
        float per = 0.0f;
        ZoomProcess proc = null;
        proc = (ref float scale)=>
        {
            per += Time.smoothDeltaTime / duration;
            
            if(per <= 1.0f)
            {
                scale = TargetScale * (1.0f + (m_fZoom - 1.0f) * per);
            }
            else if(per <= 2.0f)
            {
                scale = TargetScale * (1.0f + (m_fZoom - 1.0f) * (2.0f - per));
            }
            else
            {
                m_fZoom = 1.0f;
                m_zoomProc -= proc;
            }
        };

        m_zoomProc += proc;
    }

    public void UnsetZoom()
    {
        m_fZoom = 1.0f;
        m_tfTarget = null;
    }

    public void SetBackgroundFadeOut()
    {
        if(m_nFadeEffectCount == 0)
        {
            StopCoroutine("_fadeIn");
            StopCoroutine("_fadeOut");
            StartCoroutine("_fadeOut");
        }

        ++m_nFadeEffectCount;
    }

    public void SetBackgroundFadeIn()
    {
        --m_nFadeEffectCount;

        if(m_nFadeEffectCount == 0)
        {
            StopCoroutine("_fadeOut");
            StopCoroutine("_fadeIn");
            StartCoroutine("_fadeIn");
        }
    }

    IEnumerator _fadeOut()
    {
        float per = BackgroundFade;
        while(per > 0.25f)
        {
            per -= Time.smoothDeltaTime * 4.5f;
            if(per < 0.25f) per = 0.25f;
            BackgroundFade = per;
            yield return null;
        }
    }

    IEnumerator _fadeIn()
    {
        float per = BackgroundFade;
        while(per < 1.0f)
        {
            per += Time.smoothDeltaTime * 4.5f;
            if(per > 1.0f) per = 1.0f;
            BackgroundFade = per;
            yield return null;
        }
    }
}
