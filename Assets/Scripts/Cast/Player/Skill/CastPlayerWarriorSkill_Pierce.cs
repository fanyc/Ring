using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CastPlayerWarriorSkill_Pierce : Castable
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
            return new Vector2(6.0f * m_caster.Direction, 1.5f);
        }
    }

    public CastPlayerWarriorSkill_Pierce(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        return true;
    }

    protected override void Prepare()
    {
        base.Prepare();
        m_caster.AddAnimationEvent(Hit);
        m_caster.WeightBonus += 100.0f;

        CameraController.Instance.SetBackgroundFadeOut();
        //CameraController.Instance.SetZoom(1.15f, 0.1f);
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerWarrior.AttackPerSecond / GameManager.Instance.PlayerSpeed);

        yield return new WaitForSeconds(0.2f);
        
        m_caster.PlayAnimation("atk_01", false, false);
        Effect pierce = ObjectPool<Effect>.Spawn("@Effect_Pierce", position + new Vector3(1.75f, 0.75f));
        SkeletonAnimation sword = pierce.GetComponentInChildren<SkeletonAnimation>();
        sword.skeleton.A = 1.0f;
        ReleaseAction += ()=>
        {
            pierce.Resume();
            sword.StartCoroutine(_fadeout(sword));
            CameraController.Instance.SetBackgroundFadeIn();
            CameraController.Instance.UnsetZoom();
            //sword["PierceSword"].speed = 1.0f;
        };
        
        float t = 0.0f;
        float d = 0.05f;
        for(int i = 0; i < 10; ++i)
        {
            t = 0.0f;

            int count = Physics2D.OverlapAreaNonAlloc((Vector2)position, (Vector2)position + ((Vector2)pierce.cachedTransform.position - (Vector2)position) + new Vector2(0.5f * m_caster.Direction, 0.0f), m_Buffer, TargetMask);

            if(count > 0)
            {
                for(int j = 0; j < count; ++j)
                {
                    Character target = Character.GetCharacter(m_Buffer[j]);
                    if(target == null) continue;
                    target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("WarriorSkillDamage").currentValue * 0.166667f, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
                    target.KnockBack(new Vector2(5.0f, 0.0f));
                    ObjectPool<Effect>.Spawn("@Effect_Arrow_Normal").Init(target.position + new Vector3(0.0f, 0.75f));
                }
            
                CameraController.Instance.SetShake(0.10f, 0.05f, 0.1f);
                pierce.Pause();
                //sword["PierceSword"].speed = 0.0f;
                yield return new WaitForSeconds(0.025f);
                pierce.Resume();
                //sword["PierceSword"].speed = 1.0f;

                d = 0.025f;
            }
            else
            {
                d = 0.025f;
            }

            while(t < d)
            {
                pierce.cachedTransform.position += new Vector3(0.2f * m_caster.Direction / d * Time.smoothDeltaTime, 0.0f);
                position += new Vector3(0.1f * m_caster.Direction / d * Time.smoothDeltaTime, 0.0f);
                
                t += Time.smoothDeltaTime;
                yield return null;
            }
        }

        // t = 0.0f;
        // while(t < 0.025f)
        // {
        //     pierce.cachedTransform.position += new Vector3(1.0f * m_caster.Direction / 0.025f * Time.smoothDeltaTime, 0.0f);
        //     t += Time.smoothDeltaTime;
        //     yield return null;
        // }

        // Character[] targets = GetTargets();

        // for(int i = 0; i < targets.Length; ++i)
        // {
        //     Character target = targets[i];

        //     if(target == null) continue;
        //     target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("WarriorSkillDamage").currentValue * 0.5f, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
        //     target.KnockBack(new Vector2(10.0f, 0.0f));
        // }

        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.WeightBonus -= 100.0f;
        m_caster.RemoveAnimationEvent(Hit);
        base.Release();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        // Character[] targets = GetTargets();

        // for(int i = 0; i < targets.Length; ++i)
        // {
        //     Character target = targets[i];

        //     if(target == null) continue;
        //     target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue * UpgradeManager.Instance.GetUpgrade("WarriorSkillDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
        //     target.KnockBack(new Vector2(20.0f, 4.6f));
        // }

        // ObjectPool<Effect>.Spawn("@Effect_Lightning01").Init(m_caster.position + new Vector3(2.0f, 0.0f));
        // ObjectPool<Effect>.Spawn("@Effect_Lightning02").Init(m_caster.position + new Vector3(2.0f, 0.0f));
        // ObjectPool<Effect>.Spawn("@Effect_Flash01").Init(m_caster.position + new Vector3(2.0f, 0.0f));

        // CameraController.Instance.SetShake(0.35f, 0.075f, 0.3f);
    }

    IEnumerator _fadeout(SkeletonAnimation skeleton)
    {
        float f = 1.0f;
        while(f > 0.0f)
        {
            skeleton.skeleton.A = f;
            f -= Time.smoothDeltaTime * 2.0f;
            yield return null;
        }

        skeleton.skeleton.A = 0.0f;
    }
}