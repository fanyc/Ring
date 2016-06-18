using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerArcherSkill : Castable
{
    public override float Cost
    {
        get { return 45.0f; }
    }
    public CastPlayerArcherSkill(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        if(GameManager.Instance.InGameState != GameManager.StateInGame.BATTLE) return false;
        return true;
    }
    
    protected override void Prepare()
    {
        m_caster.AddAnimationEvent(Hit);
    }
    protected override IEnumerator Cast()
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_01", false, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(1.133f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
        target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("ArcherSkillDamage").currentValue);
    }
}