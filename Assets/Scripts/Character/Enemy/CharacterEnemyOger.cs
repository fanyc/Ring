using System.Numerics;
using UnityEngine;

public class CharacterEnemyOger : CharacterEnemy
{
    public override void Init()
    {
        base.Init();
        //m_castAttack = new CastEnemyOgerAttack(this);
    }
    
    public override void Dead()
    {
        base.Dead();
         
        BigDecimal reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue;
        for(int i = 0; i < 10; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", cachedTransform.position).Init(reward / 80.0f);
        }
        
    }
}