using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CastPlayerArcherAttack : Castable
{
    
    public CastPlayerArcherAttack(Character caster) : base(caster)
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
        SetCoolTime(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("atk_" + Random.Range(1, 3).ToString("00"), true, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        //target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue + 2.0f);
    
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {        
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;

        if(target == null) return;
        Spine.Bone bone = m_caster.GetAnimationBone("wp_elf_c01_c");
        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Arrow_Normal");
        float angle = bone.AppliedRotation + Random.Range(-2.5f, 2.5f);
        float dist = (target.cachedTransform.position.x - m_caster.cachedTransform.position.x) + Random.Range(-0.25f, 0.25f);
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        Vector2 pos = (Vector2)m_caster.cachedTransform.position + new Vector2(bone.WorldX, bone.WorldY);
        Vector2 dest = new Vector2(dist, dist * Mathf.Tan(angle * Mathf.Deg2Rad));
        proj.Init((Vector3)pos, pos + dest, ()=>
        {
            target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.ELF);
            ObjectPool<Effect>.Spawn("@Effect_Arrow_Normal").Init(pos + dest);
        });
    } 
}