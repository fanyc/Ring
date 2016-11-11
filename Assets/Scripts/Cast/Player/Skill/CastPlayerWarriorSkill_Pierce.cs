using System.Collections;
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
        m_caster.PlayIdleAnimation();
        
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerWarrior.AttackPerSecond / GameManager.Instance.PlayerSpeed);

        m_caster.PlayAnimation("skill_01", false, false);
        EffectSpine pierce = (EffectSpine)ObjectPool<Effect>.Spawn("@Effect_Pierce", new Vector3(-10000.0f, -10000.0f));
        SkeletonAnimation sword = pierce.SpineAnimation;
        pierce.PlayAnimation("on_02");
        yield return null;
        pierce.Init(position + new Vector3(-0.15f, 0.75f));
        
        while(pierce.IsEndAnimation() == false) yield return null;
        ReleaseAction += ()=>
        {
            
            pierce.Resume();
            sword.StartCoroutine(_fadeout(pierce));
            CameraController.Instance.SetBackgroundFadeIn();
            CameraController.Instance.UnsetZoom();
            //sword["PierceSword"].speed = 1.0f;
        };
        
        int cycle = 10;
        float dist = 4.0f;
        float duration = 0.25f;
        float damage = 1.0f;

        float p = 0.0f;
        for(int i = 0; i < cycle; ++i)
        {
            int count = Physics2D.OverlapAreaNonAlloc((Vector2)position, (Vector2)position + ((Vector2)pierce.cachedTransform.position - (Vector2)position) + new Vector2(0.5f * m_caster.Direction, 0.0f), m_Buffer, TargetMask);

            if(count > 0)
            {
                for(int j = 0; j < count; ++j)
                {
                    Character target = Character.GetCharacter(m_Buffer[j]);
                    if(target == null) continue;
                    target.Beaten(damage, CharacterEnemy.DAMAGE_TYPE.WARRIOR, true);
                    target.KnockBack(new Vector2(5.0f, 0.0f));
                    target.Stun(0.2f);
                    ObjectPool<Effect>.Spawn("@Effect_Arrow_Normal").Init(target.position + new Vector3(0.0f, 0.75f));
                }
            
                CameraController.Instance.SetShake(0.10f, 0.05f, 0.1f);
                pierce.Pause();
                //sword["PierceSword"].speed = 0.0f;
                yield return new WaitForSeconds(duration / cycle);
                pierce.Resume();
                //sword["PierceSword"].speed = 1.0f;
            }

            while(p < (1.0f / cycle) * (i + 1))
            {
                float step = Time.smoothDeltaTime / duration;
                p += step;
                
                pierce.cachedTransform.position += new Vector3(dist * m_caster.Direction * step, 0.0f);
                position += new Vector3(dist * m_caster.Direction * step * 0.5f, 0.0f);
                
                yield return null;
            }
        }
        ObjectPool<Effect>.Spawn("@Effect_Boost").Init(pierce.cachedTransform.position);
        
        
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

    IEnumerator _fadeout(EffectSpine pierce)
    {
        pierce.PlayAnimation("off_02");
        while(pierce.IsEndAnimation() == false) yield return null;
        pierce.Recycle();
    }
}