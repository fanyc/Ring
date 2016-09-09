using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerMageSkill_Ball : Castable
{
        protected Effect eff;

    public override float Cost
    {
        get { return 2.0f; }
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
            return new Vector2(2.0f, 1.5f);
        }
    }

    public CastPlayerMageSkill_Ball(Character caster) : base(caster)
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
        CameraController.Instance.SetBackgroundFadeOut();
        //CameraController.Instance.SetZoom(1.15f, 0.1f);
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;

        yield return new WaitForSeconds(0.2f);

        SetCoolTime(CharacterPlayerMage.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_01", false, false);
        eff = ObjectPool<Effect>.Spawn("@Effect_Fireball_Cast", Vector3.zero,
        m_caster.GetComponentInChildren<Spine.Unity.SkeletonUtility>().boneRoot.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));

        Spine.Bone bone = m_caster.GetAnimationBone("wp_sor_c01");
        float angle = bone.AppliedRotation;
        Vector2 pos = (Vector2)m_caster.position + new Vector2(bone.WorldX, bone.WorldY);
        eff.Init(pos);
        eff.cachedTransform.localPosition = new Vector3(0.845f, 0.0f, -0.01f);
        eff.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);

        yield return new WaitForSeconds(1.3f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        eff?.Recycle();
        m_caster.RemoveAnimationEvent(Hit);
        CameraController.Instance.SetBackgroundFadeIn();

        base.Release();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {        
        eff?.GetComponent<ParticleSystem>().Stop();
        m_caster.StartCoroutine(_move());
        CameraController.Instance.UnsetZoom();
    }

    IEnumerator _move()
    {
        Effect ball = ObjectPool<Effect>.Spawn("@Effect_Ball");
        ball.Init(position + new Vector3(5.0f * m_caster.Direction, 1.0f));
        ball.StartCoroutine(_hit(ball));
        while(ball.IsPlaying)
        {
            ball.cachedTransform.position += new Vector3(2.5f * m_caster.Direction * Time.smoothDeltaTime, 0.0f);
            yield return null;
        }
    }

    IEnumerator _hit(Effect eff)
    {
        float timer = 0.0f;
        while(eff.IsPlaying)
        {
            timer += Time.deltaTime;
            if(timer > 0.2f)
            {
                timer -= 0.2f;

                Character[] targets = GetTargets(eff.cachedTransform.position + new Vector3(Rect.x * -0.5f, 0.0f), new Vector2(Rect.x * 0.5f, Rect.y));

                for(int i = 0; i < targets.Length; ++i)
                {
                    Character target = targets[i];
                    if(target == null) continue;
                    target.Beaten(1.0f, CharacterEnemy.DAMAGE_TYPE.SORCERESS, true);
                    target.KnockBack(new Vector2(5.0f, 0.0f));
                    target.Stun(0.5f);
                }

                if(targets.Length > 0)
                    CameraController.Instance.SetShake(0.1f, 0.03f, 0.2f);
            }
            yield return null;
        }
    }
}