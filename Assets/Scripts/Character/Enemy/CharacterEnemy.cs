using UnityEngine;
using System.Collections;
using System.Numerics;

public abstract class CharacterEnemy : Character
{
    public enum TYPE
    {
        NULL = -1,
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
        get { return 1000.0f; }
    }

    private int m_nCachedLayer = 0;
    public override int Layer
    {
        get
        {
            if(m_nCachedLayer == 0)
            {
                m_nCachedLayer = 1 << LayerMask.NameToLayer("Enemy");
            }
            return m_nCachedLayer;
        }
    }

    public float Offset;
    public abstract TYPE Type
    {
        get;
    }
    protected Castable m_castAttack;

    public override void Init()
    {
        Direction = LEFT;
        MaxHP = m_fHP = (UpgradeManager.Instance.GetUpgrade("EnemyHP").currentValue + new BigDecimal(8.0f)) * HPFactor;
        m_cachedAnimation.skeleton.a = 1.0f;

        SetCamera();
        base.Init();
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
    
    

    protected IEnumerator DEAD()
    {
        PlayIdleAnimation();

        float t = 0.0f;
        
        while(t < 1.0f)
        {
            yield return null;
            t += Time.smoothDeltaTime * 2.0f;
            m_cachedAnimation.skeleton.a = 1.0f - t;
        }
        m_cachedAnimation.skeleton.a = 0.0f;
        Recycle();
    }
    
    public override void Dead()
    {
        base.Dead();
        State = STATE.DEAD;

        //UILevelInfo.Instance.SetBoss(false);
        //UILevelInfo.Instance.SetLeaveBoss(false);

        GameManager.Instance.NextWave();
        
        if(IsBoss == true)
        {
            GameManager.Instance.StopCoroutine("_timer");
        }
    }

    public virtual void SetCamera()
    {
        CameraController.Instance.TargetScale = 1.0f;
        CameraController.Instance.Offset = new Vector2(-0.2f, 1.65f);
    }
}