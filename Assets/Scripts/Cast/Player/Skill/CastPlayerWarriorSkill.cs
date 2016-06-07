using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerWarriorSkill : Castable
{
    public override float Cost
    {
        get { return 30.0f; }
    }
    public CastPlayerWarriorSkill(Character caster) : base(caster)
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
        m_caster.PlayAnimation("skill_01", true, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(0.5f / GameManager.Instance.PlayerSpeed);
        target.Beaten(5.0f);
        yield return new WaitForSeconds(0.267f / GameManager.Instance.PlayerSpeed);
        
        SetCoolTime(1.0f / GameManager.Instance.PlayerSpeed);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
    }
}