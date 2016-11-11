using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastEnemyWerewolfAttack : Castable
{
    protected enum TYPE
    {
        ATK_01,
        ATK_02,
        ATK_03,
    }

    protected TYPE m_Type;

    public override bool IsHighlight
    {
        get { return false; }
    }
    
    public CastEnemyWerewolfAttack(Character caster) : base(caster)
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
        m_Type = (TYPE)Random.Range(0, 3);

        m_caster.PlayAnimation(m_Type.ToString().ToLower(), true);
        while(m_caster.IsEndAnimation() != true) yield return null;
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        SetCoolTime(2.5f);
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
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
    } 
}