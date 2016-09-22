using System.Collections;
using UnityEngine;

public class CastPlayerMageAttack : Castable
{
    protected Effect eff;

    private int m_cachedMask = 0;
    private Character m_cachedTarget;
    public override int TargetMask
    {
        get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Enemy"); return m_cachedMask; }
    }


    public override Vector2 Rect
    {
        get
        {
            return new Vector2((MinDistance + 0.0f) * GameManager.Instance.Direction, 10.0f);
        }
    }

    public override float MinDistance
    {
        get
        {
            return 5.0f;
        }
    }
    public CastPlayerMageAttack(Character caster) : base(caster)
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
        SetCoolTime(CharacterPlayerMage.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("atk_" + Random.Range(1, 3).ToString("00"), false, false, GameManager.Instance.PlayerSpeed);

        eff = ObjectPool<Effect>.Spawn("@Effect_Fireball_Cast", Vector3.zero,
        m_caster.GetComponentInChildren<Spine.Unity.SkeletonUtility>().boneRoot.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));

        Spine.Bone bone = m_caster.GetAnimationBone("wp_sor_c01");
        float angle = bone.AppliedRotation;
        Vector2 pos = (Vector2)m_caster.position + new Vector2(bone.WorldX, bone.WorldY);
        eff.Init(pos);
        eff.cachedTransform.localPosition = new Vector3(0.845f, 0.0f, -0.01f);
        eff.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);


        yield return new WaitForSeconds(CharacterPlayerMage.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        //target.Beaten(UpgradeManager.Instance.GetUpgrade("SoceressAttackDamage").currentValue + 2.0f);
    
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
        Character target = GetNearestTarget(GetTargets());
        if(target == null) return;

        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Fireball");
        Vector2 pos = (Vector2)eff.cachedTransform.position;// m_caster.position + new Vector2(bone.WorldX, bone.WorldY)
        //+ new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 0.845f;

        eff?.Recycle();
        
        proj.Init((Vector3)pos, target.position + new Vector3(0.0f, pos.y), ()=>
        {
            int count = Physics2D.OverlapCircleNonAlloc((Vector2)proj.cachedTransform.position, 2.0f, m_Buffer, TargetMask);

            float damage = UpgradeManager.Instance.GetUpgrade("SoceressAttackDamage").currentValue;
            for(int i = 0; i < count; ++i)
            {
                Character t = Character.GetCharacter(m_Buffer[i]);
                t.Beaten(damage, CharacterEnemy.DAMAGE_TYPE.SORCERESS);
                t.KnockBack(new Vector2(7.5f, 0.0f));
            }

            ObjectPool<Effect>.Spawn("@Effect_Fireball_Explosion").Init(proj.cachedTransform.position);
        });
        
    }
} 