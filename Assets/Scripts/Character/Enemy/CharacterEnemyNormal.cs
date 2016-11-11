using UnityEngine;
using System.Collections;

public class CharacterEnemyNormal : CharacterEnemy
{
    public override TYPE Type
    {
        get { return TYPE.Normal; }
    }
    
    public override float HPFactor
    {
        get { return 5.0f; }
    }

    public float Range = 1.0f;
    
    public override void Init()
    {
        m_castAttack = new CastEnemyMeleeAttack(this, new Vector2(Range, 1.5f));
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
            
            Vector3 pos = position + new Vector3(11.25f * Time.smoothDeltaTime * 0.25f * -GameManager.Instance.Direction, 0.0f);
            
            position = pos;
            
            yield return null;
        }
        
        NextState();
    }

    public override void Dead()
    {
        float reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue / (6.0f * 8.0f);
        for(int i = 0; i < 8; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", position + new Vector3(0.0f, 1.0f)).Init(reward);
        }

        base.Dead();
    }
}