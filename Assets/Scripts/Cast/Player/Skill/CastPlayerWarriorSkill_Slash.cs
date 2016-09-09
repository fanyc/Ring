using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CastPlayerWarriorSkill_Slash : Castable
{
    public override float Cost
    {
        get { return 1.0f; }
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

    public CastPlayerWarriorSkill_Slash(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        if(State == Character.STATE.BEATEN) return false;
        return true;
    }

    protected override void Prepare()
    {
        base.Prepare();
        m_caster.WeightBonus += 100.0f;

        CameraController.Instance.SetBackgroundFadeOut();
        //CameraController.Instance.SetZoom(1.15f, 0.1f);  
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerWarrior.AttackPerSecond / GameManager.Instance.PlayerSpeed);

        yield return new WaitForSeconds(0.2f);
        
        m_caster.PlayAnimation("atk_01", false, false);
        Effect pierce = ObjectPool<Effect>.Spawn("@Effect_Slash01", position + new Vector3(2.0f, 1.5f));

        CameraController.Instance.SetShake(0.15f, 0.1f, 1.2f);
        
        yield return new WaitForSeconds(0.05f);

        for(int i = 0; i < 7; ++i)
        {
            Character[] targets = GetTargets();

            for(int j = 0; j < targets.Length; ++j)
            {
                Character target = targets[j];

                if(target == null) continue;
                target.Beaten(1.0f, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
                target.Stun(0.5f);
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.2f);
        m_caster.AddAnimationEvent(Hit);
        
        m_caster.PlayAnimation("atk_02", true, false);
        while(m_caster.IsEndAnimation() == false)
            yield return null;
        CameraController.Instance.SetBackgroundFadeIn();
        CameraController.Instance.UnsetZoom();
        yield return new WaitForSeconds(0.5f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.WeightBonus -= 100.0f;
        m_caster.RemoveAnimationEvent(Hit);


        base.Release();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        Effect pierce = ObjectPool<Effect>.Spawn("@Effect_Slash02", position + new Vector3(2.0f, 1.5f));

        CameraController.Instance.SetShake(0.35f, 0.25f, 0.5f);

        Character[] targets = GetTargets(position, new Vector2(10.0f, 10.0f));
        for(int j = 0; j < targets.Length; ++j)
        {
            Character target = targets[j];

            if(target == null) continue;
            target.Beaten(10.0f, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
            target.KnockBack(new Vector2(10.0f, 0.0f));
            target.Stun(1.0f);
        }
    }
}