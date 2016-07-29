using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerWarriorAttack : Castable
{
    
    private int m_cachedMask = 0;
    public override int TargetMask
    {
        get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Enemy"); return m_cachedMask; }
    }

    public override Vector2 Rect
    {
        get
        {
            return new Vector2(1.0f * m_caster.Direction, 1.5f);
        }
    }

    public override Character[] GetTargets()
    {
        int count = Physics2D.OverlapAreaNonAlloc((Vector2)position, (Vector2)position + Rect, m_Buffer, TargetMask);

        Character[] ret = new Character[count];
        for(int i = 0; i < count; ++i)
        {
            ret[i] = Character.GetCharacter(m_Buffer[i]);
        }

        return ret;
    }

    public CastPlayerWarriorAttack(Character caster) : base(caster)
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
        SetCoolTime(CharacterPlayerWarrior.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        m_caster.PlayAnimation("atk_" + Random.Range(1,4).ToString("00"), false, false, GameManager.Instance.PlayerSpeed);
        yield return new WaitForSeconds(CharacterPlayerWarrior.AttackPerSecond / GameManager.Instance.PlayerSpeed);
        State = Character.STATE.IDLE;
    }
    
    protected override void Release()
    {
        m_caster.RemoveAnimationEvent(Hit);
    }

    void Hit(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        Character[] targets = GetTargets();
        int count = targets.Length;

        //for(int i = 0; i < count; ++i)
        {
            Character target = GetNearestTarget(targets);
            if(target == null) return;
            target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue, Character.DAMAGE_TYPE.WARRIOR);
            target.KnockBack(new Vector2(15.0f, 2.3f));
            //ObjectPool<Effect>.Spawn("@Warrior_" + m_caster.GetAnimationName()).Init(m_caster.cachedTransform.position);
            ObjectPool<Effect>.Spawn("@Effect_Sword_Atk").Init(m_caster.cachedTransform.position);
        }
    }
}