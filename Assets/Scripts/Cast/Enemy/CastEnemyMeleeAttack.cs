using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastEnemyMeleeAttack : Castable
{
    
    private int m_cachedMask = 0;
    public override int TargetMask
    {
        get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Ally"); return m_cachedMask; }
    }

    public override Vector2 Rect
    {
        get
        {
            return new Vector2(m_vecRect.x * m_caster.Direction, m_vecRect.y);
        }
    }

    private Vector2 m_vecRect;

    public CastEnemyMeleeAttack(Character caster, Vector2 rect) : base(caster)
    {
        m_vecRect = rect;

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
        m_caster.PlayAnimation(m_caster.GetAttackAnimation(), false, false, GameManager.Instance.PlayerSpeed);
        while(m_caster.IsEndAnimation() != true) yield return null;
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

        for(int i = 0; i < count; ++i)
        {
            Character target = targets[i];
            if(target == null) continue;
            //target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue, Character.DAMAGE_TYPE.WARRIOR);
            target.KnockBack(new Vector2(7.5f * m_caster.Direction, 0.0f));
            //ObjectPool<Effect>.Spawn("@Warrior_" + m_caster.GetAnimationName()).Init(m_caster.position);
        }
    }
}