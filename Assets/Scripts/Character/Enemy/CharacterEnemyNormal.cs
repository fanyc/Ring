using System.Numerics;
using UnityEngine;

public class CharacterEnemyNormal : CharacterEnemy
{
    public override TYPE Type
    {
        get { return TYPE.Normal; }
    }


    public override void Init()
    {
        base.Init();
        m_castAttack = new CastEnemyOgerAttack(this);
        m_castAttack.SetCoolTime(2.5f);
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