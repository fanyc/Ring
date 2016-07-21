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
        NULL = -1,
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
            if(m_currentState == STATE.DEAD && value != STATE.NULL) return;
            if(m_currentState == STATE.NULL) return;
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
        StopAllCoroutines();
        
        m_currentState = STATE.IDLE;
        NextState();
    }
    
    protected void NextState()
    {
        StartCoroutine(State.ToString());
    }
    
    IEnumerator IDLE()
    {
        PlayIdleAnimation();
        while(State == STATE.IDLE)
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
            //m_cachedAnimation.loop = isLoop;
            SetAnimationTimeScale(timeScale);
            if(isReset)
            {
                m_cachedAnimation.state.ClearTrack(0);
            }
            //m_cachedAnimation.AnimationName = name;
            m_cachedAnimation.state.SetAnimation(0, name, isLoop);
            
            
        }
    }

    public Spine.Bone GetAnimationBone(string boneName)
    {
        return m_cachedAnimation.Skeleton.FindBone(boneName);
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
    
    public string GetAnimationName()
    {
        return m_cachedAnimation.AnimationName;
    }

    


    public void AddAnimationEvent(Spine.AnimationState.EventDelegate listener)
    {
        m_cachedAnimation.state.Event += listener;
    } 

    public void RemoveAnimationEvent(Spine.AnimationState.EventDelegate listener)
    {
        m_cachedAnimation.state.Event -= listener;
    } 

    public virtual void Beaten(BigDecimal damage, bool isSmash = false)
    {
        
    }

    public virtual void PlayBeatenAnimation()
    {
        if(State == STATE.IDLE)
        {
            PlayAnimation("hit_01", true, false);
            m_cachedAnimation.state.AddAnimation(0, "stand_01", true, 0.0f);
        }
    }

    public virtual void PlayIdleAnimation()
    {
        PlayAnimation("stand_01", false, true);
    }
}