using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerArcherSkill_MagicArrow : Castable
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

    public CastPlayerArcherSkill_MagicArrow(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        return true;
    }
    
    protected override void Prepare()
    {
        m_caster.WeightBonus += 100.0f;
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayIdleAnimation();
        yield return new WaitForSeconds(0.2f);

        m_caster.PlayAnimation("skill_01", false, false, 0.75f);
        m_caster.StartCoroutine(_spawn());
        yield return new WaitForSeconds(0.533333f);
        m_caster.SetAnimationTimeScale(1.0f);
        yield return new WaitForSeconds(0.38f);
        m_caster.SetAnimationTimeScale(0.1f);
        yield return new WaitForSeconds(0.2f);
        m_caster.SetAnimationTimeScale(1.0f);
        
        while(!m_caster.IsEndAnimation()) yield return null;

        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.WeightBonus -= 100.0f;
        base.Release();
    }

    IEnumerator _spawn()
    {
        Character[] targets = GetTargets();
        if(targets.Length > 0)
        {
            for(int i = 0; i < 12; ++i)
            {
                new ProjMagicArrow().Init(m_caster, targets[i % targets.Length]);
                yield return new WaitForSeconds(0.08f);
            }
        }
    }


    protected class ProjMagicArrow
    {
        protected Character m_caster;
        protected Character m_target;
        protected ProjectileArrow m_proj;

        public void Init(Character caster, Character target)
        {
            m_caster = caster;
            m_target = target;

            Vector2 offset = new Vector2(Random.Range(0.0f, 2.0f) * -m_caster.Direction, Random.Range(1.5f, 4.0f));
            m_proj = (ProjectileArrow)ObjectPool<Projectile>.Spawn("@Proj_MagicArrow");
            m_proj.cachedTransform.position = m_caster.position + (Vector3)offset;
            Vector3 dist = (m_target.position + new Vector3(0.0f, 0.5f)) - m_proj.cachedTransform.position;
            float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
            m_proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
            m_proj.Sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, (20.0f - 40.0f * Random.Range(0, 2)));
            m_proj.StartCoroutine(_rotate());
        }

        IEnumerator _rotate()
        {
            uint targetUID = m_target.UID;
            float angle = m_proj.Sprite.transform.localEulerAngles.z;
            float duration = 0.5f;
            float per = 0.0f;
            
            Vector3 dist = (m_target.position + new Vector3(0.0f, 0.5f)) - m_proj.cachedTransform.position;
            
            while(per < 1.0f)
            {
                dist = (m_target.position + new Vector3(0.0f, 0.5f)) - m_proj.cachedTransform.position;
                m_proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg);
                per += Time.smoothDeltaTime / duration;
                m_proj.Sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.LerpAngle(angle, 0.0f, per));
                yield return null; 
            }

            m_proj.cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg);
            m_proj.Sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(0.08f);


            if(m_target.State == Character.STATE.DEAD || m_target.UID != targetUID)
            {
                m_proj.StartCoroutine(_release());
                yield break;
            }

            Coroutine stretch = m_proj.StartCoroutine(_stretch());
            m_proj.Init(m_proj.cachedTransform.position, m_target.position + new Vector3(0.0f, 0.5f), ()=>
            {
                if(stretch != null)
                    m_proj.StopCoroutine(stretch);
                m_proj.StartCoroutine(_unstretch());

                if(m_target.State != Character.STATE.DEAD && m_target.UID == targetUID)
                {
                    ObjectPool<Effect>.Spawn("@Effect_MagnumShot").Init(m_proj.cachedTransform.position);
                    m_target.KnockBack(new Vector2(10.0f * m_caster.Direction, 0.0f));
                    m_target.Stun(1.0f);
                    m_target.Beaten(UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.ELF);
                }
            });

        }

        IEnumerator _stretch()
        {
            float duration = 0.08f;
            float per = 0.0f;
            while(per < 1.0f)
            {
                per += Time.smoothDeltaTime / duration;
                m_proj.cachedTransform.localScale = new Vector3(1.0f + 1.0f * per * per, 1.0f, 1.0f);
                yield return null; 
            }
        }

        IEnumerator _unstretch()
        {
            float duration = 0.12f;
            float per = 0.0f;
            float scale = m_proj.cachedTransform.localScale.x - 1.0f;
            while(per < 1.0f)
            {
                per += Time.smoothDeltaTime / duration;
                m_proj.cachedTransform.localScale = new Vector3(1.0f + scale * (1.0f - per), 1.0f, 1.0f);
                yield return null; 
            }
            m_proj.cachedTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            m_proj.StartCoroutine(_release());
        }

        IEnumerator _release()
        {
            float duration = 0.08f;
            float per = 0.0f;
            while(per < 1.0f)
            {
                per += Time.smoothDeltaTime / duration;
                m_proj.Sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - per);
                yield return null; 
            }
        }
    }
}