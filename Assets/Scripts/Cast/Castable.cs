using System.Collections;
using UnityEngine;
using System;

public class Castable
{
    public static Character GetNearestTarget(Character caster, Character[] targets)
    {
        int count = targets.Length;

        if(count <= 0) return null;
        Character target = targets[0];
        float distX = Mathf.Abs(target.cachedTransform.position.x - caster.cachedTransform.position.x);
        for(int i = 1; i < count; ++i)
        {
            if(targets[i] == null) continue;
            float dist = Mathf.Abs(targets[i].cachedTransform.position.x - caster.cachedTransform.position.x);
            if(dist < distX)
            {
                target = targets[i];
                distX = dist;
            }
        }

        return target;
    }

    protected static Collider2D[] m_Buffer = new Collider2D[100];

    public Action OnCoolTime;
    protected Coroutine CachedCoroutine;
    protected IEnumerator m_enumCast;
    
    protected float m_fMaxCoolTime = 0.0f;
    protected float m_fCoolTime = 0.0f;

    public float CurCoolTime
    {
        get
        {
            return m_fCoolTime;
        }
    }

    public float CoolTimePer
    {
        get
        {
            return m_fCoolTime / m_fMaxCoolTime;
        }
    }
    
    public bool IsCoolTime()
    {
        return m_fCoolTime > 0.0f;
    }
    
    public virtual float Cost
    {
        get{ return 0.0f; }
    }
    
    public virtual Vector2 Rect
    {
        get
        {
            return Vector2.zero;
        }
    }

    public virtual float MinDistance
    {
        get
        {
            return 0.0f;
        }
    }

    public virtual int TargetMask
    {
        get { return 0; }
    }
    
    protected Character m_caster;
    
    public Character.STATE State
    {
        get
        {
            return m_caster.State;
        }
        
        set
        {
            m_caster.State = value;
        }
    }
    
    public Vector3 position
    {
        get
        {
            return m_caster.cachedTransform.position;
        }
        
        set
        {
            m_caster.cachedTransform.position = value;
        }
    }

    public virtual Character[] GetTargets()
    {
        return null;
    }
    
    public Character GetNearestTarget(Character[] targets)
    {
        return GetNearestTarget(m_caster, targets);
    }
    public Castable(Character caster)
    {
        m_fCoolTime = 0.0f;
        m_caster = caster;
    }
    
    public void StartCast()
    {
        CachedCoroutine = m_caster.StartCoroutine(_coroutine());
    }
    public void StopCast()
    {
        if(CachedCoroutine != null)
        {
            m_caster.StopCoroutine(CachedCoroutine);
            CachedCoroutine = null;
        }
        m_enumCast = null;
        Release();
    }
    
    public bool IsCasting()
    {
        return m_enumCast != null;
    }
    
    IEnumerator _coroutine()
    {
        Prepare();
        
        m_enumCast = Cast();
        while(m_enumCast != null && m_enumCast.MoveNext())
            yield return m_enumCast.Current;
        m_enumCast = null;
            
        Release();
    }

    public virtual bool Condition()
    {
        return false;
    }

    protected virtual void Prepare()
    {
    }

    protected virtual IEnumerator Cast()
    {
        yield break;
    }

    protected virtual void Release()
    {
    }

    public void SetCoolTime(float coolTime)
    {
        float oldCoolTIme = m_fCoolTime;
        m_fMaxCoolTime = m_fCoolTime = coolTime;
        
        if(oldCoolTIme <= 0.0f)
            m_caster.StartCoroutine(_cooltime());
        
        if(OnCoolTime != null)
            OnCoolTime();
        OnCoolTime = null;
    }

    protected virtual IEnumerator _cooltime()
    {
        while(m_fCoolTime > 0.0f)
        {
            yield return null;
            m_fCoolTime -= Time.deltaTime;
        }

        m_fCoolTime = 0.0f;
    }

    public static Castable CreateCast(string strType, Character caster)
    {
        Type type = Type.GetType(strType);
        if(type != null)
        {
            object instance = Activator.CreateInstance(type, (object)caster);

            if(instance != null)
            {
                return (Castable)instance;
            }
        }
        return null;
    }
}