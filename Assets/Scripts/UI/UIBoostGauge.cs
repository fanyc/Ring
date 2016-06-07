using UnityEngine;
using System.Collections;

public class UIBoostGauge : MonoSingleton<UIBoostGauge> {
    
    protected RectTransform m_cachedRectTransform;
    protected float m_fWidth;
    protected float m_fGauge;
    protected bool IsEnter = false;
    protected bool IsPressed = false;
    public bool IsCharging
    {
        get { return IsPressed && IsEnter; }
    }
    
    public bool IsBoost
    {
        get;
        protected set;
    }
    
    void Awake()
    {
        m_cachedRectTransform = GetComponent<RectTransform>();
        m_fWidth = m_cachedRectTransform.sizeDelta.x;
        
        SetPercent(0.0f);
    }
    
    public void Press()
    {
        IsPressed = true;
    }
    
    public void Release()
    {
        IsPressed = false;
    }
    
	public void Enter()
    {
        IsEnter = true;
    }
    
    public void Exit()
    {
        IsEnter = false;
    }
    
    void Update()
    {
        if(IsCharging)
        {
            if(m_fGauge < 1.0f)
            {
                m_fGauge = Mathf.Clamp01(m_fGauge + Time.deltaTime);
                
                if(m_fGauge >= 1.0f)
                {
                    IsBoost = true;
                }
                
                SetPercent(m_fGauge);
            }
        }
        else if(m_fGauge > 0.0f)
        {
            m_fGauge = Mathf.Clamp01(m_fGauge - Time.deltaTime);
            
            if(m_fGauge <= 0.0f)
            {
                IsBoost = false;
            }
            
            SetPercent(m_fGauge);
        }
    }
    
    void SetPercent(float per)
    {
        Vector2 size = m_cachedRectTransform.sizeDelta;
        size.x = m_fWidth * per;
        m_cachedRectTransform.sizeDelta = size;
    }
    
    public void LostGauge()
    {
        m_fGauge = 0.0f;
        IsBoost = false;
        SetPercent(m_fGauge);
    }
}
