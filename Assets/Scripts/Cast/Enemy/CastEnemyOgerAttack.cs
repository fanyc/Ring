using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastEnemyOgerAttack : Castable
{
    Effect eff;

    public override bool IsHighlight
    {
        get { return false; }
    }
    
    public CastEnemyOgerAttack(Character caster) : base(caster)
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
        m_caster.PlayAnimation("atk_01", true);

        eff = ObjectPool<Effect>.Spawn("@Effect_Fireball_OrcMage", Vector3.zero, m_caster.cachedTransform.FindDeepChild("Trail"));
        while(m_caster.IsEndAnimation() != true) yield return null;
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        eff?.Recycle();
        SetCoolTime(2.5f);
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        eff?.Recycle();

        Projectile proj = ObjectPool<Projectile>.Spawn("@Proj_Fireball_OrcMage");
        proj.Init(m_caster.cachedTransform.FindDeepChild("Trail").position, GameManager.Instance.cachedTransform.position + new Vector3(-1.75f, 0.0f), ()=>
        {
            if(!UIRingButton.Instance.IsCharging)
            {
                //UIDamage.Instance.BoostBreak();
                //GameManager.Instance.PlayerHP -= 1.0f;
                //UIBoostGauge.Instance.LostGauge();
            }
            else
            {
                //UIDamage.Instance.Positive();

                List<CharacterPlayer> list = GameManager.Instance.GetPlayers();
                
                for(int i = 0; i < list.Count; ++i)
                {
                    list[i].MP += 3.0f;
                    //ObjectPool<Effect>.Spawn("@Effect_Twinkle_Small").Init(list[i].SkillButton.cachedTransform.position);
                }
            }
            ObjectPool<Effect>.Spawn("@Effect_Fireball_OrcMage_Explosion").Init(proj.cachedTransform.position);
        });

        
    } 
}