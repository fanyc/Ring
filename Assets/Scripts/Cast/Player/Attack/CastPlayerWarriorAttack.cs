using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerWarriorAttack : Castable
{
    
    public CastPlayerWarriorAttack(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        if(GameManager.Instance.InGameState != GameManager.StateInGame.BATTLE) return false;
        if(GameManager.Instance.CurrentEnemy == null) return false;
        if(GameManager.Instance.CurrentEnemy.State == Character.STATE.DEAD ||
            GameManager.Instance.CurrentEnemy.State == Character.STATE.NULL) return false;

        return true;
    }
    
    protected override void Prepare()
    {
        m_caster.AddAnimationEvent(Hit);
    }
    protected override IEnumerator Cast()
    {
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;
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
        CharacterEnemy target = GameManager.Instance.CurrentEnemy;

        if(target == null) return;
        target.Beaten(UpgradeManager.Instance.GetUpgrade("WarriorAttackDamage").currentValue, CharacterEnemy.DAMAGE_TYPE.WARRIOR);
        //ObjectPool<Effect>.Spawn("@Warrior_" + m_caster.GetAnimationName()).Init(m_caster.cachedTransform.position);
        ObjectPool<Effect>.Spawn("@Effect_Sword_Atk").Init(m_caster.cachedTransform.position);
    }
}