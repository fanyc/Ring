using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerMageSkill : Castable
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
            return new Vector2(5.0f, 1.5f);
        }
    }

    public CastPlayerMageSkill(Character caster) : base(caster)
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
        SetCoolTime(CharacterPlayerMage.AttackPerSecond / GameManager.Instance.PlayerSpeed);

        yield return new WaitForSeconds(0.2f);

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
        base.Release();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {        
        eff?.GetComponent<ParticleSystem>().Stop();
        
        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Meteor");
        Vector3 dest = m_caster.position + new Vector3(7.5f, 0.0f);//target.position;
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, 45.0f);
        proj.Init(dest + new Vector3(-7.0f, 7.0f * Mathf.Tan((90.0f - proj.cachedTransform.eulerAngles.z) * Mathf.Deg2Rad)), dest, ()=>
        {
            ObjectPool<Effect>.Spawn("@Effect_Meteor").Init(dest);
            CameraController.Instance.SetShake(0.3f, 0.075f, 0.75f);

            Character[] targets = GetTargets(dest + new Vector3(Rect.x * -0.5f, 0.0f), new Vector2(Rect.x * 0.5f, Rect.y));

            for(int i = 0; i < targets.Length; ++i)
            {
                Character target = targets[i];
                if(target == null) continue;
                //target.Beaten(UpgradeManager.Instance.GetUpgrade("SoceressAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("SoceressSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.SORCERESS, true);

                float distFactor = ((target.position.x - dest.x) / (Rect.x * 0.5f));
                distFactor = (1.0f - Mathf.Abs(distFactor)) * Mathf.Sign(distFactor);
                float knockback = 20.0f * distFactor;
                float airborne = 0.0f;//2.3f + 4.6f * Mathf.Abs(distFactor);
                target.KnockBack(new Vector2(knockback, airborne));
            }

            CameraController.Instance.SetBackgroundFadeIn();
        });

        CameraController.Instance.UnsetZoom();
    }
}