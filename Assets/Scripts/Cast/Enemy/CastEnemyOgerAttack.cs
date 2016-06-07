using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastEnemyOgerAttack : Castable
{
    
    public CastEnemyOgerAttack(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        return true;
    }
    
    protected override void Prepare()
    {
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        m_caster.PlayAnimation("attack_01", true);
        yield return new WaitForSeconds(0.5f);
        if(UIRingButton.Instance.IsCharging)
        {
            List<CharacterPlayer> list = GameManager.Instance.GetPlayers();
            
            for(int i = 0; i < list.Count; ++i)
            {
                list[i].Beaten(0.0f);
            }
            
            //UIBoostGauge.Instance.LostGauge();
        }
        yield return new WaitForSeconds(0.5f);
        SetCoolTime(1.0f);
        State = Character.STATE.IDLE;
    }
}