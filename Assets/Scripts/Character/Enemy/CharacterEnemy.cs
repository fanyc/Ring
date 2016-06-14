using UnityEngine;
using System.Collections;
using System.Numerics;

public class CharacterEnemy : Character
{
    public BigDecimal MaxHP;
    public float Offset;
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
        MaxHP = m_fHP = UpgradeManager.Instance.GetUpgrade("EnemyHP").currentValue + new BigDecimal(8.0f);
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
        PlayAnimation("hit_01", true, false);
        //m_cachedAnimation.state.AddAnimation(1, "stand_01", true, 0.233f);
        ObjectPool<DamageText>.Spawn("@DamageText", new Vector3(cachedTransform.position.x + Random.Range(0.0f, 1.0f) - 1.5f, Random.Range(0.0f, 1.0f) + 1.5f)).Init(damage.ToUnit());
        if(m_fHP <= 0.0f)
        {
            m_fHP = 0.0f;
            Dead();
        }
        HPGauge.UpdateRatio();
    }
    
    protected IEnumerator DEAD()
    {
        GameManager.Instance.BossInfo.SetActive(false);
        GameManager.Instance.NormalInfo.SetActive(true);

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