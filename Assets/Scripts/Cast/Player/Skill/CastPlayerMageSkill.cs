using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerMageSkill : Castable
{
        protected Effect eff;

    public override float Cost
    {
        get { return 0.0f; }
    }
    public CastPlayerMageSkill(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        if(GameManager.Instance.InGameState != GameManager.StateInGame.BATTLE) return false;
        if(GameManager.Instance.CurrentEnemy == null) return false;
        if(GameManager.Instance.CurrentEnemy.State == Character.STATE.DEAD ||
            GameManager.Instance.CurrentEnemy.State == Character.STATE.NULL) return false;
        return true;
    }
    
    protected override void Prepare()
    {
        m_caster.AddAnimationEvent(Hit);
    }
    protected override IEnumerator Cast()
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerMage.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_01", false, false);

        eff = ObjectPool<Effect>.Spawn("@Effect_Fireball_Cast", Vector3.zero,
        m_caster.GetComponentInChildren<Spine.Unity.SkeletonUtility>().boneRoot.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));

        Spine.Bone bone = m_caster.GetAnimationBone("wp_sor_c01");
        float angle = bone.AppliedRotation;
        Vector2 pos = (Vector2)m_caster.cachedTransform.position + new Vector2(bone.WorldX, bone.WorldY);
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
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {        
        eff?.GetComponent<ParticleSystem>().Stop();
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
        
        if(target == null) return;
        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Meteor");
        Vector3 dest = target.cachedTransform.position;
        Vector3 pos = m_caster.cachedTransform.position;
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, 45.0f);
        proj.Init(dest + new Vector3(-7.0f, 7.0f * Mathf.Tan((90.0f - proj.cachedTransform.eulerAngles.z) * Mathf.Deg2Rad)), dest, ()=>
        {
            target.Beaten(UpgradeManager.Instance.GetUpgrade("SoceressAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("SoceressSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.SORCERESS, true);
            ObjectPool<Effect>.Spawn("@Effect_Meteor").Init(target.cachedTransform.position);
            CameraController.Instance.SetShake(0.3f, 0.075f, 0.75f);
        });
    }
}