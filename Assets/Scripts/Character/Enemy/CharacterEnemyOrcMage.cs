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
        get { return 5.0f; }
    }

    public override void Init()
    {
        m_castAttack = new CastEnemyOgerAttack(this);
        m_castAttack.SetCoolTime(2.5f);
        base.Init();
    }

    public override void Dead()
    {
        BigDecimal reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue / 6.0f * 2.0f / 15.0f;
        for(int i = 0; i < 15; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", position + new Vector3(0.0f, 1.0f)).Init(reward);
        }

        base.Dead();
    }
}