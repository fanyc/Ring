using System.Numerics;
using UnityEngine;

public class CharacterEnemyNormal : CharacterEnemy
{
    public override bool IsBoss
    {
        get
        {
            return false;
        }
    }

    public override void Init()
    {
        base.Init();
        //m_castAttack = new CastEnemyOgerAttack(this);
    }
    
    public override void Dead()
    {
        base.Dead();
         
        BigDecimal reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue;
        for(int i = 0; i < 5; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", cachedTransform.position).Init(reward / 100.0f);
        }
        
    }
}