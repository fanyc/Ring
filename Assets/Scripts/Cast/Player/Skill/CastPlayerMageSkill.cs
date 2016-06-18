using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerMageSkill : Castable
{
    public override float Cost
    {
        get { return 60.0f; }
    }
    public CastPlayerMageSkill(Character caster) : base(caster)
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
        SetCoolTime(CharacterPlayerMage.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_01", false, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(1.867f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
        target.Beaten(UpgradeManager.Instance.GetUpgrade("SoceressAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("SoceressSkillDamage").currentValue);
    }
}