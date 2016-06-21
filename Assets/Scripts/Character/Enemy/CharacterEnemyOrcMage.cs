
using System.Numerics;
using UnityEngine;

public class CharacterEnemyOrcMage : CharacterEnemy
{
    public override TYPE Type
    {
        get { return TYPE.LevelBoss; }
    }
    public override float HPFactor
    {
        get { return 30.0f; }
    }

    public override void Init()
    {
        base.Init();
        m_castAttack = new CastEnemyOgerAttack(this);
    }
    
    public override void PlayBeatenAnimation()
    {

        m_cachedAnimation.state.AddAnimation(0, "hit_01", false, 0.0f);
    }

    public override void Dead()
    {
        base.Dead();
         
        BigDecimal reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue;
        for(int i = 0; i < 15; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", cachedTransform.position).Init(reward / 100.0f * 3.0f);
        }
        
    }
}