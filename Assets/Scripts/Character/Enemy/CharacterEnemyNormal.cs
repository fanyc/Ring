using System.Numerics;
using UnityEngine;
using System.Collections;

public class CharacterEnemyNormal : CharacterEnemy
{
    public override TYPE Type
    {
        get { return TYPE.Normal; }
    }


    public override void Init()
    {
        m_castAttack = new CastEnemyMeleeAttack(this);
        base.Init();
    }
    
    protected override void IdleThought()
    {
        if(m_castAttack?.GetTargets()?.Length > 0)
        {
            Attack();
        }
        else
        {
            State = STATE.MOVE;
        }
    }

    protected IEnumerator MOVE()
    {
        PlayAnimation(GetRunAnimation(), false, true);
        while(State == STATE.MOVE)
        {
            
            if(m_castAttack.GetTargets().Length > 0)
            {
                Attack();
                yield return null;
                break;
            }
            
            Vector3 pos = cachedTransform.position + new Vector3(11.25f * Time.smoothDeltaTime * 0.25f * -GameManager.Instance.Direction, 0.0f);
            
            cachedTransform.position = pos;
            
            yield return null;
        }
        
        NextState();
    }

    public override void Dead()
    {
        BigDecimal reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue / (6.0f * 8.0f);
        for(int i = 0; i < 8; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", cachedTransform.position + new Vector3(0.0f, 1.0f)).Init(reward);
        }

        base.Dead();
    }
}