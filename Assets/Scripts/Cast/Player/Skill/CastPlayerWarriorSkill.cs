using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerWarriorSkill : Castable
{
    public override float Cost
    {
        get { return 4.0f; }
    }

    private int m_cachedMask = 0;
    public override int TargetMask
    {
        get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Enemy"); return m_cachedMask; }
    }

    public override Vector2 Rect
    {
        get
        {
            return new Vector2(4.0f * m_caster.Direction, 1.5f);
        }
    }

    public CastPlayerWarriorSkill(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        return true;
    }

    protected override void Prepare()
    {
        m_caster.AddAnimationEvent(Hit);
        m_caster.WeightBonus += 100.0f;
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerWarrior.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_a01", false, false);
        yield return new WaitForSeconds(0.767f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.WeightBonus -= 100.0f;
        m_caster.RemoveAnimationEvent(Hit);
        base.Release();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        Character[] targets = GetTargets();

        for(int i = 0; i < targets.Length; ++i)
        {
            Character target = targets[i];

            if(target == null) continue;
            target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("WarriorSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
            target.KnockBack(new Vector2(20.0f, 4.6f));
        }

        ObjectPool<Effect>.Spawn("@Effect_Lightning01").Init(m_caster.position + new Vector3(2.0f, 0.0f));
        ObjectPool<Effect>.Spawn("@Effect_Lightning02").Init(m_caster.position + new Vector3(2.0f, 0.0f));
        ObjectPool<Effect>.Spawn("@Effect_Flash01").Init(m_caster.position + new Vector3(2.0f, 0.0f));

        CameraController.Instance.SetShake(0.35f, 0.075f, 0.3f);
    }
}