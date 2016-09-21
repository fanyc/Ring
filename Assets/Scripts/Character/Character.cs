using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Spine.Unity;

public abstract class Character : ObjectBase
{
    //static
    protected static List<Character> m_listCharacter = new List<Character>();
    protected static Dictionary<Collider2D, Character> m_dictColliderMap = new Dictionary<Collider2D, Character>();

    public static Character GetCharacter(Collider2D col)
    {
        Character ret = null;
        m_dictColliderMap.TryGetValue(col, out ret);
        return ret;
    }

    public static List<Character> GetCharacters(int LayerFilter = int.MaxValue)
    {
        List<Character> ret = new List<Character>(m_listCharacter.Count);
        for(int i = 0, c = m_listCharacter.Count; i < c; ++i)
        {
            if((m_listCharacter[i].Layer & LayerFilter) != 0)
            {
                ret.Add(m_listCharacter[i]);
            }
        }
        ret.TrimExcess();
        return ret;
    }

    //end static
    protected SkeletonAnimation m_cachedAnimation; 
    protected Collider2D m_cachedCollider2D;
    
    public enum STATE
    {
        NULL = -1,
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_AFTER,
        CAST,
        BEATEN,
        DEAD,
    }

    public enum DAMAGE_TYPE
    {
        NULL = -1,
        WARRIOR,
        ELF,
        SORCERESS,
        ETC,
    }
    
    protected bool m_bPositionUpdate = false;
    protected Vector3 m_vecPosition;
    public Vector3 position
    {
        get
        {
            return m_vecPosition;
        }

        set
        {
            m_vecPosition = value;
            m_bPositionUpdate = true;
        }
    }

    protected STATE m_currentState = STATE.IDLE;
    protected Castable m_CurrentCast;
    protected Castable m_PrepareCast;
    

    public const int LEFT = -1;
    public const int RIGHT = 1;

    protected int m_nDirection = RIGHT;
    public int Direction
    {
        get
        {
            return m_nDirection;
        }
        set
        {
            m_nDirection = value;
            cachedTransform.localScale = new Vector3(m_nDirection, 1.0f, 1.0f);
        }
    }

    public abstract int Layer
    {
        get;
    }
    
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

    protected BigDecimal m_fHP = BigDecimal.Zero;
    public BigDecimal HP
    {
        get
        {
            return m_fHP;
        }
    }
    public BigDecimal MaxHP = BigDecimal.Zero;

    protected HPGauge m_HPGauge;
    public float HPGaugeHeight = 400.0f;

    protected float m_fStun = 0.0f;
    protected float m_fKnockBack = 0.0f;
    protected float m_fWeight = 0.0f;
    public float WeightBonus = 0.0f;
    
    
    protected virtual void Awake()
    {
        m_cachedAnimation = GetComponentInChildren<SkeletonAnimation>();
        m_cachedAnimation.Initialize(false);

        m_cachedCollider2D = GetComponent<Collider2D>();
    }

    protected virtual void LateUpdate()
    {
        if(m_bPositionUpdate)
        {
            cachedTransform.position = position;
            m_bPositionUpdate = false;
        }
    } 
    
    public virtual void Init()
    {
        position = cachedTransform.position;
        
        m_cachedCollider2D.enabled = true;
        m_dictColliderMap.Add(m_cachedCollider2D, this);
        m_listCharacter.Add(this);
        StopAllCoroutines();
        
        cachedTransform.localScale = new Vector3(m_nDirection, 1.0f, 1.0f);
        m_fKnockBack = 0.0f;

        if(m_HPGauge != null)
        {
            m_HPGauge.Recycle();
            m_HPGauge = null;
        }
        m_HPGauge = ObjectPool<HPGauge>.Spawn("@HPGauge");
        m_HPGauge.Init(this);

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
        if(State == STATE.BEATEN)
        {
            m_PrepareCast = cast;
            return;
        }

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

    IEnumerator BEATEN()
    {
        while(State != STATE.DEAD && (m_fStun > 0.0f /*|| m_fKnockBack > 0.0f || position.y > 0.0f*/)) yield return null;
        if(State != STATE.DEAD)
        {
            yield return new WaitForSeconds(0.2f);
            State = STATE.IDLE;
            PlayIdleAnimation();
            if(m_PrepareCast != null)
            {
                Cast(m_PrepareCast);
                m_PrepareCast = null;
            }
        }
        
        NextState();
    }
    public virtual void Beaten(BigDecimal damage, DAMAGE_TYPE type, bool isSmash = false)
    {
        if(State == STATE.DEAD || State == STATE.NULL) return;

        PlayBeatenAnimation();

        Color startColor = Color.white;
        Color endColor = Color.white;
        Color outlineColor = Color.black;
        Vector3 offset = Vector3.zero;

        switch (type)
        {
            case DAMAGE_TYPE.WARRIOR:
            startColor = new Color32(254, 216, 69, 255);
            endColor = new Color32(223, 121, 30, 255);
            outlineColor = new Color32(102, 35, 21, 192);
            offset = new Vector3(0.0f, 0.0f, 0.0f);
            break;

            case DAMAGE_TYPE.ELF:
            startColor = new Color32(210, 234, 74, 255);
            endColor = new Color32(116, 153, 39, 255);
            outlineColor = new Color32(59, 66, 15, 192);
            offset = new Vector3(-0.3f, 0.3f, 1.0f);
            break;

            case DAMAGE_TYPE.SORCERESS:
            startColor = new Color32(255, 235, 166, 255);
            endColor = new Color32(237, 65, 75, 255);
            outlineColor = new Color32(185, 15, 80, 192);
            offset = new Vector3(-0.6f, 0.6f, 2.0f);
            break;

            case DAMAGE_TYPE.ETC:
            startColor = new Color32(255, 81, 81, 255);
            endColor = new Color32(226, 14, 14, 255);
            outlineColor = new Color32(77, 1, 1, 192);
            offset = new Vector3(-0.9f, 0.9f, 3.0f);
            break;
        }

        ObjectPool<DamageText>.Spawn("@DamageText", new Vector3(position.x + 0.8f, 2.0f)).Init(damage.ToUnit(), offset, startColor, endColor, outlineColor);

        m_fHP -= damage;
        //if(Type != TYPE.StageBoss || isSmash == true)
            //PlayBeatenAnimation();
        //m_cachedAnimation.state.AddAnimation(1, "stand_01", true, 0.233f);
        if(m_fHP <= 0.0f)
        {
            m_fHP = 0.0f;
            m_HPGauge?.UpdateRatio();
            Dead();
        }
        else
        {
            m_HPGauge?.UpdateRatio();
        }
    }

    public void Stun(float duration)
    {
        m_fStun = Mathf.Max(m_fStun, duration);

        StopCoroutine("_stun");
        StartCoroutine("_stun");
    }

    IEnumerator _stun()
    {
        State = STATE.BEATEN;
        
        while(m_fStun > 0.0f)
        {
            yield return null;
            m_fStun -= Time.deltaTime;
        }

        m_fStun = 0.0f;
    }

    public void KnockBack(Vector2 power)
    {
        if(power.x * m_fKnockBack == 0.0f)
        {
            m_fKnockBack = power.x;
        }
        else if(power.x * m_fKnockBack > 0.0f)
        {
            m_fKnockBack = Mathf.Max(Mathf.Abs(power.x), Mathf.Abs(m_fKnockBack)) * Mathf.Sign(power.x);
        }
        else
        {
            m_fKnockBack = m_fKnockBack + power.x;
        }

        if(m_fWeight + WeightBonus > Mathf.Abs(m_fKnockBack))
        {
            m_fKnockBack = 0.0f;
            return;
        }

        //State = STATE.BEATEN;
        StopCoroutine("_knockBack");
        StartCoroutine("_knockBack");
        StartCoroutine(_airborne(power.y));
    }
    
    IEnumerator _knockBack()
    {
        while(Mathf.Abs(m_fKnockBack) > 0.0f)
        {
            yield return null;
            Vector3 center = GameManager.Instance.cachedTransform.position;
            Vector3 pos = position;
            pos += new Vector3(m_fKnockBack * Time.deltaTime, 0.0f, 0.0f);
            pos.x = Mathf.Clamp(pos.x, center.x - GameManager.Instance.LimitDistance, center.x + GameManager.Instance.LimitDistance);
            position = pos;
            if(100.0f * Time.deltaTime >= Mathf.Abs(m_fKnockBack))
            {
                break;
            }
            m_fKnockBack -= 100.0f * Time.deltaTime * Mathf.Sign(m_fKnockBack);
        }

        m_fKnockBack = 0.0f;
    }

    IEnumerator _airborne(float power)
    {
        while(power > 0.0f || position.y > 0.0f)
        {
            yield return null;
            Vector3 pos = position;
            pos += new Vector3(0.0f, power * Time.deltaTime);
            if(pos.y < 0.0f) pos.y = 0.0f;
            position = pos;
            power -= 9.8f * Time.deltaTime;
        }
    }

    public virtual void Dead()
    {
        m_dictColliderMap.Remove(m_cachedCollider2D);
        m_listCharacter.Remove(this);
        m_cachedCollider2D.enabled = false;
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

    public virtual void PlayBeatenAnimation()
    {
        if(State == STATE.IDLE || State == STATE.BEATEN)
        {
            PlayAnimation(GetBeatenAnimation(), true, false);
            //m_cachedAnimation.state.AddAnimation(0, "stand_01", true, 0.0f);
        }
    }

    public virtual void PlayIdleAnimation()
    {
        PlayAnimation("stand_01", false, true);
    }

    public virtual string GetRunAnimation()
    {
        return "run_01";
    }

    public virtual string GetAttackAnimation()
    {
        return "atk_01";
    }

    public virtual string GetBeatenAnimation()
    {
        return "stand_01";
    }
} 