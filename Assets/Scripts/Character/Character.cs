using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Spine.Unity;

public class Character : ObjectBase
{
    protected SkeletonAnimation m_cachedAnimation; 
    
    public enum STATE
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_AFTER,
        CAST,
        
        DEAD,
    }
    
    protected STATE m_currentState = STATE.IDLE;
    
    public STATE State
    {
        get { return m_currentState; }
        set
        {
            m_currentState = value;
        }
    }
    
    protected Castable m_CurrentCast;
    
    protected virtual void Awake()
    {
        m_cachedAnimation = GetComponentInChildren<SkeletonAnimation>();
        m_cachedAnimation.Initialize(false);
    }
    
    
    public virtual void Init()
    {
        m_currentState = STATE.IDLE;
        NextState();
    }
    
    protected void NextState()
    {
        StartCoroutine(m_currentState.ToString());
    }
    
    IEnumerator IDLE()
    {
        PlayAnimation("stand_01", true, true);
        while(m_currentState == STATE.IDLE)
        {
            IdleThought();
            yield return null;
        }
        NextState();
    }
    
    protected virtual void IdleThought()
    {
        
    }
    
    public void Cast(Castable cast)
    {
        if(cast.Condition() == false)
            return;
            
        CastCancel();
        m_CurrentCast = cast;
        m_CurrentCast.StartCast();
    }
    public void CastCancel()
    {
        if(m_CurrentCast != null)
            m_CurrentCast.StopCast();
            
        m_CurrentCast = null;
    }
    
    protected virtual IEnumerator CAST()
    {
        while(State == STATE.CAST && m_CurrentCast != null && m_CurrentCast.IsCasting())
        {
            yield return null;
        }
        CastCancel();
        
        if(State == STATE.CAST)
            State = STATE.IDLE;

        NextState();
    }
    
    public virtual void PlayAnimation(string name, bool isReset = false, bool isLoop = false, float timeScale = 1.0f)
    {
        if(name.Equals("") == false)
        {
            m_cachedAnimation.loop = isLoop;
            SetAnimationTimeScale(timeScale);
            if(isReset)
            {
                m_cachedAnimation.state.ClearTrack(0);
            }
            m_cachedAnimation.AnimationName = name;
            
            
        }
    }

    public bool IsEndAnimation(float offset = 0.0f)
    {
        return m_cachedAnimation.state.GetCurrent(0) == null || m_cachedAnimation.state.GetCurrent(0).Time + offset >= m_cachedAnimation.state.GetCurrent(0).EndTime;
    }

    public float GetAnimationTimeScale()
    {
        return m_cachedAnimation.timeScale;
    }

    public void SetAnimationTimeScale(float timeScale)
    {
        m_cachedAnimation.timeScale = timeScale;
    }
    
    public void AddAnimationEvent(Spine.AnimationState.EventDelegate listener)
    {
        m_cachedAnimation.state.Event += listener;
    } 

    public void RemoveAnimationEvent(Spine.AnimationState.EventDelegate listener)
    {
        m_cachedAnimation.state.Event -= listener;
    } 

    public virtual void Beaten(BigDecimal damage)
    {
        
    }
}