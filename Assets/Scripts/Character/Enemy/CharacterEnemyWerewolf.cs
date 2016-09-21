using System.Numerics;
using UnityEngine;

public class CharacterEnemyWerewolf : CharacterEnemy
{
    public override TYPE Type
    {
        get { return TYPE.StageBoss; }
    }
    public override float HPFactor
    {
        get { return 10.0f; }
    }

    public override void Init()
    {
        m_castAttack = new CastEnemyWerewolfAttack(this);
        m_castAttack.SetCoolTime(2.5f);
        base.Init();
    }

    public override void Dead()
    {
        BigDecimal reward = UpgradeManager.Instance.GetUpgrade("Reward").currentValue / 6.0f * 2.0f / 15.0f;
        for(int i = 0; i < 15; ++i)
        {
            ObjectPool<ItemGold>.Spawn("@ItemGold", cachedTransform.position + new Vector3(0.0f, 1.0f)).Init(reward);
        }

        base.Dead();
    }

    public override void PlayBeatenAnimation()
    {
        if(State == STATE.IDLE)
        {
            PlayAnimation("hit_01", true, false);
            cachedAnimation.state.AddAnimation(0, "stand", true, 0.0f);
        }
    }

    public override void PlayIdleAnimation()
    {
        PlayAnimation("stand", false, true);
    }

    public override void SetCamera()
    {
        CameraController.Instance.TargetScale = 1.0f;
    }
}