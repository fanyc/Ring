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
            return new Vector2(2.0f, 1.5f);
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
        m_caster.PlayIdleAnimation();

        m_caster.PlayAnimation("stand_01", false, false);
        
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
        m_caster.StartCoroutine(_spawn());
        CameraController.Instance.UnsetZoom();
    }

    IEnumerator _spawn()
    {
        SpawnMeteor(new Vector2(4.0f, 0.0f));
        yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
        SpawnMeteor(new Vector2(6.5f, 0.0f));
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        SpawnMeteor(new Vector2(7.5f, 0.0f));
        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        SpawnMeteor(new Vector2(9.5f, 0.0f));

        CameraController.Instance.SetBackgroundFadeIn();
    }

    void SpawnMeteor(Vector3 offset)
    {
        ProjectileArrow proj = (ProjectileArrow)ObjectPool<Projectile>.Spawn("@Proj_Meteor2");
        Vector3 dest = m_caster.position + offset;//target.position;
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, Random.Range(15.0f, 45.0f));
        proj.Init(dest + new Vector3(Mathf.Cos((90.0f + proj.cachedTransform.eulerAngles.z) * Mathf.Deg2Rad), Mathf.Sin((90.0f + proj.cachedTransform.eulerAngles.z) * Mathf.Deg2Rad)) * 7.0f, dest, ()=>
        {
            proj.Sprite.enabled = false;
            ObjectPool<Effect>.Spawn("@Effect_Meteor_Explosion").Init(dest);
            CameraController.Instance.SetShake(0.2f, 0.05f, 0.75f);

            Character[] targets = GetTargets(dest + new Vector3(Rect.x * -0.5f, 0.0f), Rect);

            for(int i = 0; i < targets.Length; ++i)
            {
                Character target = targets[i];
                if(target == null) continue;
                target.Beaten(1.0f, CharacterEnemy.DAMAGE_TYPE.SORCERESS, true);
                target.KnockBack(new Vector2(15.0f * m_caster.Direction, 1.0f));
            }

        });
    }
}