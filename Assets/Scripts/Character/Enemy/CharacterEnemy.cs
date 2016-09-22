using UnityEngine;
using System.Collections;

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
        get { return 1.0f; }
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

    protected override void LateUpdate()
    {
        if(State != STATE.DEAD && (position.x - (CharacterPlayer.PlayerList[0].position.x + 1.0f * GameManager.Instance.Direction)) * GameManager.Instance.Direction < 0.0f)
        {
            Vector3 pos = position;
            pos.x = CharacterPlayer.PlayerList[0].position.x + 1.0f * GameManager.Instance.Direction;
            position = pos;
        }

        base.LateUpdate();
    }

    public override void Init()
    {
        Direction = LEFT;
        MaxHP = m_fHP = (UpgradeManager.Instance.GetUpgrade("EnemyHP").currentValue + 8.0f) * HPFactor;
        cachedAnimation.skeleton.a = 1.0f;

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
    
    protected override IEnumerator DEAD()
    {
        PlayDeadAnimation();

        float t = 0.0f;
        
        while(t < 1.0f)
        {
            yield return null;
            t += Time.smoothDeltaTime * 2.0f;
            cachedAnimation.skeleton.a = 1.0f - t;
        }
        cachedAnimation.skeleton.a = 0.0f;
        Recycle();
    }
    
    public override void Dead()
    {
        base.Dead();

        GameManager.Instance.RemoveEnemy(this);
    }

    public virtual void SetCamera()
    {
        CameraController.Instance.TargetScale = 1.0f;
    }

    public override string GetDeadAnimation()
    {
        return "dmg_01";
    }
}