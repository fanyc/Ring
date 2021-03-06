using System.Collections;
using UnityEngine;

public class CastPlayerArcherAttack : Castable
{
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
            return new Vector2((Distance + 0.0f) * GameManager.Instance.Direction, 10.0f);
        }
    }

    public override float Distance
    {
        get
        {
            return 3.0f;
        }
    }

    public override bool IsHighlight
    {
        get { return false; }
    }

    public CastPlayerArcherAttack(Character caster) : base(caster)
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
        m_caster.PlayAnimation("atk_" + Random.Range(1, 3).ToString("00"), true, false, GameManager.Instance.PlayerSpeed);
        m_cachedTarget = GetNearestTarget(GetTargets());
        if(m_cachedTarget != null)
            yield return new WaitForSeconds(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        //target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue + 2.0f);


        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
        base.Release();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        if(m_cachedTarget == null) return;
        if(m_cachedTarget.State == Character.STATE.DEAD || m_cachedTarget.State == Character.STATE.NULL)
        {
            m_cachedTarget = null;
            return;
        }
        Spine.Bone bone = m_caster.GetAnimationBone("wp_elf_c01_c");
        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Arrow_Normal");
        float angle = bone.AppliedRotation + Random.Range(-2.5f, 2.5f);
        float dist = (m_cachedTarget.position.x - m_caster.position.x) + Random.Range(-0.25f, 0.25f);
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        Vector2 pos = (Vector2)m_caster.position + new Vector2(bone.WorldX, bone.WorldY);
        Vector2 dest = new Vector2(dist, dist * Mathf.Tan(angle * Mathf.Deg2Rad));
        proj.Init((Vector3)pos, pos + dest, ()=>
        {
            m_cachedTarget.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.ELF);
            ObjectPool<Effect>.Spawn("@Effect_Arrow_Normal").Init(pos + dest);
        });
    } 
}