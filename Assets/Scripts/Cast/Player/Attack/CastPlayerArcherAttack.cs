using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CastPlayerArcherAttack : Castable
{
    
    public CastPlayerArcherAttack(Character caster) : base(caster)
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
        SetCoolTime(2.0f / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("attack_01", true, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(0.4f / GameManager.Instance.PlayerSpeed);
        target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue + 2.0f);
        yield return new WaitForSeconds(0.6f / GameManager.Instance.PlayerSpeed);
    
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
    }
}