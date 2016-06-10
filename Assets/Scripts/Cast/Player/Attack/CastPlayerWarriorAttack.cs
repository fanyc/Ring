using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerWarriorAttack : Castable
{
    
    public CastPlayerWarriorAttack(Character caster) : base(caster)
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
        SetCoolTime(0.467f / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("atk_" + Random.Range(1,4).ToString("00"), true, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(0.467f / GameManager.Instance.PlayerSpeed);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
        target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue * 0.467f);
    }
}