using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerMageAttack : Castable
{
    
    public CastPlayerMageAttack(Character caster) : base(caster)
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
        SetCoolTime(5.0f / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("atk_" + Random.Range(1, 3).ToString("00"), true, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(1.867f / GameManager.Instance.PlayerSpeed);
        //target.Beaten(5.0f + UpgradeManager.Instance.GetUpgrade("SoceressAttackDamage").currentValue);
    
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
    }
}