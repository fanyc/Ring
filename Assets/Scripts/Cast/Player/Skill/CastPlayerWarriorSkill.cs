using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerWarriorSkill : Castable
{
    public override float Cost
    {
        get { return 0.0f; }
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
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemies[0];

        if(target == null) return;
        target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("WarriorSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
        ObjectPool<Effect>.Spawn("@Effect_Lightning01").Init(m_caster.cachedTransform.position + new Vector3(2.0f, 0.0f));
        ObjectPool<Effect>.Spawn("@Effect_Lightning02").Init(m_caster.cachedTransform.position + new Vector3(2.0f, 0.0f));
        ObjectPool<Effect>.Spawn("@Effect_Flash01").Init(m_caster.cachedTransform.position + new Vector3(2.0f, 0.0f));
        CameraController.Instance.SetShake(0.35f, 0.075f, 0.3f);
    }
}