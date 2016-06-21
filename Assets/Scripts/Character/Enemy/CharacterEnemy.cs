using UnityEngine;
using System.Collections;
using System.Numerics;

public abstract class CharacterEnemy : Character
{
    public enum TYPE
    {
        Null = -1,
        Normal = 0,
        LevelBoss,
        StageBoss,
    }
    public virtual bool IsBoss
    {
        get
        {
            return Type == TYPE.LevelBoss || Type == TYPE.StageBoss;
        }
    }

    
    public virtual float HPFactor
    {
        get { return 1.0f; }
    }

    public BigDecimal MaxHP;
    public float Offset;
    public abstract TYPE Type
    {
        get;
    }
    protected Castable m_castAttack;
    protected BigDecimal m_fHP;
    public BigDecimal HP
    {
        get
        {
            return m_fHP;
        }
    }
    
    public override void Init()
    {
        base.Init();
        MaxHP = m_fHP = (UpgradeManager.Instance.GetUpgrade("EnemyHP").currentValue + new BigDecimal(8.0f)) * HPFactor;
        m_cachedAnimation.skeleton.a = 1.0f;
    }
    protected override void IdleThought()
    {
        if(GameManager.Instance.InGameState == GameManager.StateInGame.BATTLE)
        {
            Attack();
        }
    }
    
    protected virtual void Attack()
    {
        if(m_castAttack != null)
            Cast(m_castAttack);
    }
    
    public override void Beaten(BigDecimal damage)
    {
        if(m_currentState == STATE.DEAD) return;
        base.Beaten(damage);
        m_fHP -= damage;
        PlayBeatenAnimation();
        //m_cachedAnimation.state.AddAnimation(1, "stand_01", true, 0.233f);
        ObjectPool<DamageText>.Spawn("@DamageText", new Vector3(cachedTransform.position.x + Random.Range(0.0f, 1.0f) - 1.5f, Random.Range(0.0f, 1.0f) + 1.5f)).Init(damage.ToUnit());
        if(m_fHP <= 0.0f)
        {
            m_fHP = 0.0f;
            Dead();
        }
        HPGauge.UpdateRatio();
    }

    public virtual void PlayBeatenAnimation()
    {
        PlayAnimation("hit_01", true, false);
    }
    
    protected IEnumerator DEAD()
    {
        GameManager.Instance.BossInfo.SetActive(false);
        GameManager.Instance.NormalInfo.SetActive(true);

        if(IsBoss == true)
        {
            GameManager.Instance.StopCoroutine("_timer");
        }

        PlayAnimation("stand_01");

        float t = 0.0f;
        
        while(t < 1.0f)
        {
            yield return null;
            t += Time.smoothDeltaTime * 2.0f;
            m_cachedAnimation.skeleton.a = 1.0f - t;
        }
        m_cachedAnimation.skeleton.a = 0.0f;
        Recycle();
        GameManager.Instance.NextWave();
    }
    
    public virtual void Dead()
    {
        m_currentState = STATE.DEAD;
    }
}