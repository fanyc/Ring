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

    protected Character m_target;
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

        Character[] targets = GetTargets();
        if(targets.Length > 0)
        {
            Character farthest = targets[0];
            for(int i = 1; i < targets.Length; ++i)
            {
                if((targets[i].position.x - position.x) * m_caster.Direction > (farthest.position.x - position.x) * m_caster.Direction)
                    farthest = targets[i];
            }
            
            Vector2 pos = (Vector2)m_caster.position + new Vector2(bone.WorldX, bone.WorldY);
            Vector2 dest = (Vector2)farthest.position + new Vector2(bone.WorldX, bone.WorldY);

            ProjectileBoundingBox proj = (ProjectileBoundingBox)ObjectPool<Projectile>.Spawn("@Proj_SpiralShot");
            proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);

            proj.Init((Vector3)pos, dest, (Collider2D col)=>
            {
                Character target = Character.GetCharacter(col);
                if(target != null && (target.Layer & TargetMask) != 0)
                {
                    //ObjectPool<Effect>.Spawn("@Effect_MagnumShot").Init(target.position + new Vector3(0.0f, pos.y));
                    target.KnockBack(new Vector2(10.0f, 0.0f));
                    target.Stun(1.0f);
                }
                
                CameraController.Instance.SetShake(0.3f, 0.1f, 0.2f);
            },
            ()=>
            {
                m_caster.StartCoroutine(Haul(farthest));
            });
        }
        CameraController.Instance.UnsetZoom();
    }

    IEnumerator Haul(Character target)
    {
        if(target == null) yield break;
        

        ObjectPool<Effect>.Spawn("@Effect_SpiralShot").Init(target.position + new Vector3(0.0f, 1.0f));
        target.Color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        target.SetTint(new Color(0.5f, 0.5f, 1.0f, 1.0f), 0.12f);
        
        Vector2 pos = (Vector2)target.position + new Vector2(0.0f, 1.0f);
        Vector2 dest = (Vector2)m_caster.position + new Vector2(0.0f, 1.0f);
        Vector2 dist = dest - pos;

        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / 0.3f;
            target.position += (Vector3)(dist.normalized * -Time.smoothDeltaTime * 4.0f * (1.75f - t));
            yield return null;
        }
        pos = (Vector2)target.position + new Vector2(0.0f, 1.0f);
        dest = (Vector2)m_caster.position + new Vector2(0.0f, 1.0f);

        target.Stun(1.0f);

        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_SpiralShot_Back");
        proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        proj.Init((Vector3)pos, (Vector3)dest, ()=>
        {
            
        });

        dist = dest - pos;
        float p = 0.0f;
        float duration = dist.magnitude / 20.0f * 2.0f;
        while(p < 1.0f)
        {
            p += Time.deltaTime / duration;
            target.position += (Vector3)(dist.normalized * (Mathf.Pow(1.0f - p, 2.0f)) * (Time.smoothDeltaTime * 20.0f));
            yield return null;
        }
        
        target.SetTint(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.2f);
    }
    
}
