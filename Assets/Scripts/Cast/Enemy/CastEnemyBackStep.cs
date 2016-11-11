using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastEnemyBackStep : Castable
{
    public override bool IsHighlight
    {
        get { return false; }
    }
    
    public CastEnemyBackStep(Character caster) : base(caster)
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
        m_caster.PlayAnimation("back_dash", false, false, 1.5f);
        while(m_caster.IsEndAnimation() != true) yield return null;
        SetCoolTime(5.0f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        m_caster.KnockBack(new Vector2(m_caster.Direction * -20.0f, 0.0f));
    }
}