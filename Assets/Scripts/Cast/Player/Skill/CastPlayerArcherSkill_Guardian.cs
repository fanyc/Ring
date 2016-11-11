using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerArcherSkill_Guardian : Castable
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

    private int m_nArrowCnt = 0;
    private EffectSpine m_Effect;
    private Effect[] m_EffectArrow = new Effect[3];
    
    

    public CastPlayerArcherSkill_Guardian(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        return true;
    }
    
    protected override void Prepare()
    {
        m_nArrowCnt = 0;
        CameraController.Instance.SetBackgroundFadeOut();
        m_caster.WeightBonus += 100.0f;
    }
    protected override IEnumerator Cast()
    {
        State = Character.STATE.CAST;
        SetCoolTime(CharacterPlayerArcher.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayIdleAnimation();

        yield return new WaitForSeconds(0.2f);

        m_Effect = (EffectSpine)ObjectPool<Effect>.Spawn("@Effect_Guardian");
        m_Effect.Init(m_caster.position);
        m_Effect.PlayAnimation("01");
        m_Effect.SpineAnimation.state.Event += Hit;
        ReleaseAction += ()=>
        {
            m_Effect.SpineAnimation.state.Event -= Hit;
        };
        yield return null;
        m_Effect.SpineAnimation.skeleton.a = 1.0f;
        OutlineSystem.Instance.cachedGameObject.SetActive(true);
        m_caster.PlayAnimation("skill_01", false, false, 0.3f);
        yield return new WaitForSeconds(1.333333f);
        m_caster.SetAnimationTimeScale(1.0f);
        while(!m_Effect.IsEndAnimation(0.8f)) yield return null;
        m_Effect.StartCoroutine(_release(m_Effect));
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.WeightBonus -= 100.0f;
        m_caster.RemoveAnimationEvent(Hit);
        base.Release();
    }

    IEnumerator _release(EffectSpine eff)
    {   
        Color c = new Color32(252, 255, 193, 255);

        float t = 0.0f;

        while(t < 1.0f)
        {
            t += Time.deltaTime * 1.25f;
            
            // Color a = Color.white - (Color.white - c) * t;
            // eff.SpineAnimation.Skeleton.r = a.r;
            // eff.SpineAnimation.Skeleton.g = a.g;
            // eff.SpineAnimation.Skeleton.b = a.b;
            eff.SpineAnimation.skeleton.a = Mathf.Max(1.0f - t, 0.0f);
            //eff.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - t);
            yield return null;
        }

        eff.SpineAnimation.skeleton.a = 0.0f;
        eff.Recycle();
        CameraController.Instance.SetBackgroundFadeIn();
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        switch(e.Data.name)
        {
            case "hit_01":
            {
                ObjectPool<Effect>.Spawn("@Effect_Guardian_Fire2").Init(position);
                for(int i = 0; i < m_EffectArrow.Length; ++i)
                {
                    Effect eff = ObjectPool<Effect>.Spawn("@Effect_Guardian_Arrow_Trace");
                    eff.Init(Vector3.zero);

                    float angle = m_EffectArrow[i].cachedTransform.eulerAngles.z;
                    eff.cachedTransform.eulerAngles = m_EffectArrow[i].cachedTransform.eulerAngles;
                    eff.cachedTransform.position = m_EffectArrow[i].cachedTransform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * 12.0f, Mathf.Sin(angle * Mathf.Deg2Rad) * 12.0f);

                    if(m_EffectArrow[i] != null)
                    {
                        m_EffectArrow[i].Recycle();
                        m_EffectArrow[i] = null;
                    }
                }

                Character[] targets = GetTargets();
            
                for(int i = 0; i < targets.Length; ++i)
                {
                    if(targets[i] == null) return;
                    ObjectPool<Effect>.Spawn("@Effect_MagnumShot").Init(targets[i].position + new Vector3(0.0f, 0.75f));
                    targets[i].KnockBack(new Vector2(35.0f, 0.0f));
                    targets[i].Stun(1.0f);
                }
            }
            break;

            case "arrow_01":
            {
                Effect eff = ObjectPool<Effect>.Spawn("@Effect_Guardian_Arrow");
                eff.cachedTransform.parent = m_Effect.cachedTransform.FindDeepChild("Muzzle");
                eff.Init(Vector3.zero);
                
                switch(m_nArrowCnt)
                {
                    case 0:
                    eff.cachedTransform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    eff.cachedTransform.localPosition += new Vector3(0.0f, 0.0f, -1.0f);
                    break;

                    case 1:
                    eff.cachedTransform.localEulerAngles = new Vector3(0.0f, 0.0f, 7.5f);
                    break;

                    case 2:
                    eff.cachedTransform.localEulerAngles = new Vector3(0.0f, 0.0f, -7.5f);
                    break;
                }
                m_EffectArrow[m_nArrowCnt] = eff;
                ++m_nArrowCnt;
            }
            break;
        }
    }
}