using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public SkeletonAnimation cachedAnimation
    {
        get
        {
            if(m_cachedAnimation == null)
            {
                m_cachedAnimation = GetComponentInChildren<SkeletonAnimation>();
                m_cachedAnimation.Initialize(false);
            } 

            return m_cachedAnimation;
        }
    } 

    public Color Color
    {
        get
        {
            return m_cachedAnimation.skeleton.GetColor();
        }
        set
        {
            m_cachedAnimation.skeleton.SetColor(value);
        }
    }
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

    protected Castable m_castAttack;
    protected Castable m_CurrentCast;
    protected Queue<Castable> m_PrepareCast = new Queue<Castable>();
    

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

    protected float m_fHP = 0.0f;
    public float HP
    {
        get
        {
            return m_fHP;
        }
    }
    public float MaxHP = 0.0f;

    protected UIHPGauge m_HPGauge;

    public Vector2 BasePosition = new Vector2(0.0f, 0.75f);
    public float HPGaugeHeight = 400.0f;

    protected float m_fStun = 0.0f;
    protected float m_fKnockBack = 0.0f;
    protected float m_fWeight = 0.0f;
    public float WeightBonus = 0.0f;
    
    
    protected virtual void Awake()
    {
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
        m_HPGauge = ObjectPool<UIHPGauge>.Spawn("@HPGauge");
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
            if(m_PrepareCast.Count > 0)
            {
                Cast(m_PrepareCast.Dequeue());
                break;
            }
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
        if(State == STATE.DEAD) return;
        if(cast != m_castAttack)
        {
            if(State == STATE.BEATEN ||
                (State == STATE.CAST && m_CurrentCast != m_castAttack))
            {
                m_PrepareCast.Enqueue(cast);
                return;
            }
        }

        if(cast.Condition() == false)
            return;

        if(cast.IsHighlight)
        {
            ObjectPool<Effect>.Spawn("@Effect_Skill", Vector3.zero, cachedTransform).Init((Vector3)BasePosition);
        }
        
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
        PlayBeatenAnimation();
        while(State != STATE.DEAD && (m_fStun > 0.0f /*|| m_fKnockBack > 0.0f || position.y > 0.0f*/)) yield return null;
        if(State != STATE.DEAD)
        {
            State = STATE.IDLE;
            PlayIdleAnimation();
        }
        
        NextState();
    }
    public virtual void Beaten(float damage, DAMAGE_TYPE type, bool isSmash = false)
    {
        if(State == STATE.DEAD || State == STATE.NULL) return;


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

        ObjectPool<DamageText>.Spawn("@DamageText", new Vector3(position.x + 0.8f, 2.0f)).Init(damage.ToString("F0"), offset, startColor, endColor, outlineColor);

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

        if(m_fWeight + WeightBonus > Mathf.Abs(m_fStun))
        {
            m_fStun = 0.0f;
            return;
        }

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

        //CastCancel();
        PlayBeatenAnimation();
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

    public virtual void Heal(float heal)
    {
        m_fHP += heal;
        if(m_fHP > MaxHP) m_fHP = MaxHP;
        m_HPGauge?.UpdateRatio();

        ObjectPool<DamageText>.Spawn("@DamageText", new Vector3(position.x + 0.8f, 2.0f)).Init(heal.ToString("F0"), new Vector3(0.0f, 0.0f, 4.0f), new Color32(210, 234, 74, 255), new Color32(116, 153, 39, 255), new Color32(59, 66, 15, 192));
    }

    public virtual void Dead()
    {
        m_dictColliderMap.Remove(m_cachedCollider2D);
        m_listCharacter.Remove(this);
        m_cachedCollider2D.enabled = false;

        State = STATE.DEAD;
    }
    
    public virtual void PlayAnimation(string name, bool isReset = false, bool isLoop = false, float timeScale = 1.0f)
    {
        if(name.Equals("") == false)
        {
            //m_cachedAnimation.loop = isLoop;
            SetAnimationTimeScale(timeScale);
            if(isReset)
            {
                cachedAnimation.state.ClearTrack(0);
            }
            //m_cachedAnimation.AnimationName = name;
            cachedAnimation.state.SetAnimation(0, name, isLoop);
            
            
        }
    }

    protected virtual IEnumerator DEAD()
    {
        PlayDeadAnimation();
        yield break;
    }

    public Spine.Bone GetAnimationBone(string boneName)
    {

        return cachedAnimation.Skeleton.FindBone(boneName);
    }

    public bool IsEndAnimation(float offset = 0.0f)
    {
        return cachedAnimation.state.GetCurrent(0) == null || cachedAnimation.state.GetCurrent(0).Time + offset >= cachedAnimation.state.GetCurrent(0).EndTime;
    }

    public float GetAnimationTimeScale()
    {
        return cachedAnimation.timeScale;
    }

    public void SetAnimationTimeScale(float timeScale)
    {
        cachedAnimation.timeScale = timeScale;
    }
    
    public string GetAnimationName()
    {
        return cachedAnimation.AnimationName;
    }

    


    public void AddAnimationEvent(Spine.AnimationState.EventDelegate listener)
    {
        cachedAnimation.state.Event += listener;
    } 

    public void RemoveAnimationEvent(Spine.AnimationState.EventDelegate listener)
    {
        cachedAnimation.state.Event -= listener;
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

    public virtual void PlayDeadAnimation()
    {
        PlayAnimation(GetDeadAnimation(), true, false);
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
        return "dmg_01";
    }

    public virtual string GetDeadAnimation()
    {
        return "dead_01";
    }

    Coroutine _doTint;
    public virtual void SetTint(Color destColor, float time = 0.0f, System.Action Callback = null)
    {
        if(_doTint != null)
            StopCoroutine(_doTint);
        if(time > 0.0f)
        {
            _doTint = StartCoroutine(_tint(time, destColor, Callback));
        }
        else
        {
            Color = destColor;
        }
    }

    IEnumerator _tint(float time, Color destColor, System.Action Callback)
    {
        Spine.Skeleton skeleton = m_cachedAnimation.skeleton;
        Color diff = destColor - this.Color;
        float timer = 0.0f;
        while(timer < time)
        {
            Color = (destColor - diff * (1.0f - timer / time));
            yield return null;
            timer += Time.deltaTime;
        }

        Color = destColor;
        
        if(Callback != null)
            Callback();
    }

    protected Dictionary<string, StatusEffect> m_dictStatusEffect = new Dictionary<string, StatusEffect>();

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if(m_dictStatusEffect.ContainsKey(statusEffect.Name) == false)
        {
            m_dictStatusEffect.Add(statusEffect.Name, statusEffect);
            statusEffect.Init();
        }

        m_dictStatusEffect[statusEffect.Name].Stack(statusEffect);
    }

    public class StatusEffect
    {
        public virtual string Name
        {
            get;
        }
        public Character Target
        {
            get;
            protected set;
        }
        public Character Caster
        {
            get;
            protected set;
        }

        public StatusEffect(Character caster, Character target)
        {
            Target = target;
            Caster = caster;
        }

        public virtual void Init()
        {

        }

        public virtual void Stack(StatusEffect statusEffect)
        {

        }

        public virtual void Pop()
        {

        }

        public virtual void Update()
        {
            
        }

        public virtual void Release()
        {
            Target.m_dictStatusEffect.Remove(Name);
        }
    }

    public class StatusEffectDuration : StatusEffect
    {
        public float Duration
        {
            get;
            protected set;
        }

        public StatusEffectDuration(Character caster, Character target, float duration) : base(caster, target)
        {
            Duration = duration;
        }

        public override void Stack(StatusEffect statusEffect)
        {
            this.Duration = ((StatusEffectDuration)statusEffect).Duration;
        }

        public override void Update()
        {
            Duration -= Time.deltaTime;
            if(Duration <= 0.0f)
            {
                Release();
            }
        }
    }

    public class StatusEffectStun : StatusEffectDuration
    {
        public override string Name
        {
            get
            {
                return "stun";
            }
        }

        public StatusEffectStun(Character caster, Character target, float duration) : base(caster, target, duration)
        {
        }

        public override void Init()
        {
            Target.State = STATE.BEATEN;
        }

        public override void Release()
        {
            Target.State = STATE.IDLE;
            base.Release();
        }
    }
    
    // public virtual void SetVibrate(float time)
    // {
    //     if(m_fVibrateTimer <= 0.0f)
    //     {
    //         m_fVibrateTimer = time;
    //         StartCoroutine(_vibrate());
    //     }
    //     else
    //     {
    //         m_fVibrateTimer = time;
    //     }        
    // }
    
    // protected virtual IEnumerator _vibrate()
    // {
    //     float old = GetAnimationTimeScale();
    //     SetAnimationTimeScale(0.0f);
    //     Vector3 pos = m_tfZOffset.localPosition;
    //     float amp = 0.03f;
    //     float speed = 1.5f;
        
    //     while(m_fVibrateTimer > 0.0f)
    //     {
    //         pos = m_tfZOffset.localPosition;
    //         if(Mathf.Abs(pos.x) >= amp)
    //         {
    //             speed = -speed;
    //         }
    //         pos.x = Mathf.Clamp(pos.x + speed * Time.deltaTime, -amp, amp);
    //         m_tfZOffset.localPosition = pos;
    //         m_fVibrateTimer -= Time.deltaTime;
    //         yield return null;
    //     }
        
    //     pos.x = 0.0f;
    //     m_tfZOffset.localPosition = pos;
    //     SetAnimationTimeScale(old);
    //     m_fVibrateTimer = 0.0f;
    // }
} 