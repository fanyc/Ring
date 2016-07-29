using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerArcherSkill : Castable
{
    public override float Cost
    {
        get { return 0.0f; }
    }
    public CastPlayerArcherSkill(Character caster) : base(caster)
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
        SetCoolTime(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_01", false, false);
        yield return new WaitForSeconds(1.133f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        // Effect eff = ObjectPool<Effect>.Spawn("@Effect_Sand", Vector3.zero,
        // m_caster.GetComponentInChildren<Spine.Unity.SkeletonUtility>().boneRoot.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
        // eff.Init(eff.cachedTransform.position);
        

        CharacterEnemy target = GameManager.Instance.CurrentEnemies[0];

        if(target == null) return;
        Spine.Bone bone = m_caster.GetAnimationBone("wp_elf_c01_c");
        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Arrow_MagnumShot");
        float angle = bone.AppliedRotation + Random.Range(-2.5f, 2.5f);
        float dist = (target.cachedTransform.position.x - m_caster.cachedTransform.position.x) + Random.Range(-0.25f, 0.25f);
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        Vector2 pos = (Vector2)m_caster.cachedTransform.position + new Vector2(bone.WorldX, bone.WorldY);
        Vector2 dest = pos + new Vector2(dist, dist * Mathf.Tan(angle * Mathf.Deg2Rad));
        proj.Init((Vector3)pos, dest, ()=>
        {
            target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("ArcherSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.ELF, true);
            ObjectPool<Effect>.Spawn("@Effect_MagnumShot").Init(dest);
            CameraController.Instance.SetShake(0.3f, 0.1f, 0.5f);
        });
    }
}