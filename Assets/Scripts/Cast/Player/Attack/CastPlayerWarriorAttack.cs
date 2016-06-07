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
    }
    protected override IEnumerator Cast()
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
        State = Character.STATE.CAST;
        SetCoolTime(1.0f / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("attack_01", true, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(0.35f / GameManager.Instance.PlayerSpeed);
        target.Beaten(1.0f + UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue);
        yield return new WaitForSeconds(0.55f / GameManager.Instance.PlayerSpeed);
        
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
    }
}