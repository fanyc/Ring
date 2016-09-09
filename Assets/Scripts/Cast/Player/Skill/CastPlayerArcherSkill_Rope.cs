using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerArcherSkill_Rope : Castable
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
            return new Vector2(15.0f * m_caster.Direction, 1.5f);
        }
    }

    public CastPlayerArcherSkill_Rope(Character caster) : base(caster)
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
        m_caster.WeightBonus += 100.0f;

        CameraController.Instance.SetBackgroundFadeOut();
        //CameraController.Instance.SetZoom(1.15f, 0.1f);
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        
        yield return new WaitForSeconds(0.2f);

        SetCoolTime(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("skill_01", false, false);
        yield return new WaitForSeconds(1.133f);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.WeightBonus -= 100.0f;
        m_caster.RemoveAnimationEvent(Hit);

        CameraController.Instance.SetBackgroundFadeIn();
        base.Release();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        // Effect eff = ObjectPool<Effect>.Spawn("@Effect_Sand", Vector3.zero,
        // m_caster.GetComponentInChildren<Spine.Unity.SkeletonUtility>().boneRoot.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
        // eff.Init(eff.cachedTransform.position);

        Spine.Bone bone = m_caster.GetAnimationBone("wp_elf_c01_c");
        float angle = Random.Range(0.0f, 1.5f);//bone.AppliedRotation + Random.Range(-2.5f, 2.5f);
        
        float dist = 15.0f * m_caster.Direction;//(target.position.x - m_caster.position.x);// + Random.Range(-0.25f, 0.25f);

        Vector2 pos = (Vector2)m_caster.position + new Vector2(bone.WorldX, bone.WorldY);
        Vector2 dest = pos + new Vector2(dist, dist * Mathf.Tan(angle * Mathf.Deg2Rad));

        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Arrow_MagnumShot");
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        proj.Init((Vector3)pos, dest, ()=>
        {
            Character[] targets = GetTargets();
            
            List<Character> sortedList = new List<Character>(targets.Length);
            for(int i = 0; i < targets.Length; ++i)
            {
                if(targets[i] == null) return;
                sortedList.Add(targets[i]);
                //target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("ArcherSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.ELF, true);
            }

            sortedList.Sort((Character a, Character b)=>
            {
                return ((a.position.x - b.position.x) * GameManager.Instance.Direction < 0.0f ? 1 : -1);  
            });

            for(int i = 0; i < sortedList.Count; ++i)
            {
                ObjectPool<Effect>.Spawn("@Effect_MagnumShot").Init(sortedList[i].position + new Vector3(0.0f, pos.y));
                sortedList[i].KnockBack(new Vector2(5.0f + 5.0f - Mathf.Max(sortedList.Count - i * 2.5f, 0.0f), 0.0f));
                sortedList[i].Stun(1.0f);
            }

            m_caster.StartCoroutine(Haul());
            
            CameraController.Instance.SetShake(0.3f, 0.1f, 0.5f);
        });

        CameraController.Instance.UnsetZoom();
    }

    IEnumerator Haul()
    {
        yield return new WaitForSeconds(0.5f);
        Vector2 pos = (Vector2)m_caster.position + new Vector2(15.0f, 1.0f);
        Vector2 dest = (Vector2)m_caster.position + new Vector2(-15.0f, 1.0f);

        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Arrow_MagnumShot");
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        proj.Init((Vector3)pos, (Vector3)dest, ()=>
        {
            Character[] targets = GetTargets();
            
            List<Character> sortedList = new List<Character>(targets.Length);
            for(int i = 0; i < targets.Length; ++i)
            {
                if(targets[i] == null) return;
                sortedList.Add(targets[i]);
                //target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("ArcherSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.ELF, true);
            }

            sortedList.Sort((Character a, Character b)=>
            {
                return ((a.position.x - b.position.x) * GameManager.Instance.Direction < 0.0f ? -1 : 1);  
            });

            for(int i = 0; i < sortedList.Count; ++i)
            {
                ObjectPool<Effect>.Spawn("@Effect_MagnumShot").Init(sortedList[i].position + new Vector3(0.0f, pos.y));
                sortedList[i].KnockBack(new Vector2(-(10.0f + 5.0f - Mathf.Max(sortedList.Count - i * 2.5f, 0.0f)), 0.0f));
                sortedList[i].Stun(1.0f);
            }
        });
    }
}