using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPlayerWarrior : CharacterPlayer
{
    protected static SkillDataList m_skillDataList = new SkillDataList();

    public override SkillDataList ListSkillData
    {
        get {return m_skillDataList;}
    }

    static CharacterPlayerWarrior()
    {
        m_skillDataList.AddSkillData("파 크라이", "WarriorSkill", "skill_a01", "Icons/btle_icskill_wri_01b", "CastPlayerWarriorSkill");
        //m_skillDataList.AddSkillData("파 크라이", "WarriorSkill", "skill_a01", "Icons/btle_icskill_wri_01b", "CastPlayerWarriorSkill_Slash");
        m_skillDataList.AddSkillData("파 크라이", "WarriorSkill", "skill_a01", "Icons/btle_icskill_wri_01b", "CastPlayerWarriorSkill_Pierce");
        
        
    }


    public new static float AttackPerSecond
    {
        get
        {
            return 0.467f;
        }
    }
    public override void Init()
    {
        m_castAttack = new CastPlayerWarriorAttack(this);
        m_castSkill = Castable.CreateCast(m_skillDataList[0].castableName, this);

        for(int i = 0; i < m_skillDataList.Count; ++i)
        {
            UIAbilityIconSkill orig = Resources.Load<UIAbilityIconSkill>("Abilities/@AbilityIconSkill_Warrior");
            ObjectPool<UIAbilityIcon>.CreatePool("Ability" + m_skillDataList[i].castableName, orig.cachedGameObject, 10, (UIAbilityIcon icon)=>
            {
                ((UIAbilityIconSkill)icon).Init(this, m_skillDataList[i]);
            });
            UIAbilitySlot.Instance.Add("Ability" + m_skillDataList[i].castableName);
        }

        m_fHP = MaxHP = 10.0f;

        base.Init();
    }

    protected override void IdleThought()
    {
        if(m_castAttack?.GetTargets()?.Length > 0)
        {
            Attack();
        }
        else if(m_fKnockBack == 0.0f)
        {
            State = STATE.MOVE;
        }
    }

    protected override IEnumerator MOVE()
    {
        PlayAnimation(GetRunAnimation(), false, true);
        while(State == STATE.MOVE)
        {
            
            if(m_castAttack.GetTargets().Length > 0)
            {
                Attack();
                yield return null;
                break;
            }
            
            Vector3 pos = position + new Vector3(11.25f * Time.smoothDeltaTime * 0.4f * GameManager.Instance.Direction, 0.0f);
            
            position = pos;
            
            yield return null;
        }
        
        NextState();
    }
}