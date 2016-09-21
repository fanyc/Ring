using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerWarriorSkill : Castable
{
    public override float Cost
    {
        get { return 4.0f; }
    }

    private int m_cachedMask = 0;
    public override int TargetMask
    {
        get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Enemy"); return m_cachedMask; }
    }

    public override Vector2 Rect
    {
        get
        {
            return new Vector2(4.0f * m_caster.Direction, 1.5f);
        }
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
        base.Prepare();
        m_caster.WeightBonus += 100.0f;

        CameraController.Instance.SetBackgroundFadeOut();
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerWarrior.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_02", false, false);
        EffectSpine eff = (EffectSpine)ObjectPool<Effect>.Spawn("@Effect_Judgement", position + new Vector3(0.0f, 6.7f));
        eff.SpineAnimation.state.Event += _event;
        // ReleaseAction += ()=>
        // {
        //     eff.Skeleton.state.Event -= _event;
        // };
        while(m_caster.IsEndAnimation() != true) yield return null;
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        CameraController.Instance.SetBackgroundFadeIn();
        m_caster.WeightBonus -= 100.0f;
        base.Release();
    }

    void _event(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        switch(e.Data.name)
        {
            case "hit_01":
            {
                break;
            }
            case "light_01":
            {
                ObjectPool<Effect>.Spawn("@Effect_Flash01").Init(m_caster.position + new Vector3(2.0f, 0.0f));
                break;
            }
            case "shake_01":
            {
                CameraController.Instance.SetShake(0.3f, 0.075f, 0.75f);
                break;
            }
        }
    }
}