using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPlayerFairy : CharacterPlayer
{
    protected static SkillDataList m_skillDataList = new SkillDataList();

    public override SkillDataList ListSkillData
    {
        get {return m_skillDataList;}
    }

    static CharacterPlayerFairy()
    {
        //m_skillDataList.AddSkillData("파 크라이", "WarriorSkill", "skill_a01", "SkillIcon/btle_icskill_wri_01b", "CastPlayerWarriorSkill");
        m_skillDataList.AddSkillData("회복", "", "", "Icons/btle_icskill_wri_01b", "CastPlayerHeal");
    }


    public new static float AttackPerSecond
    {
        get
        {
            return 0.0f;
        }
    }

    protected Character m_following;
    protected Vector3 m_vecOffset;
    public override void Init()
    {
        m_castAttack = new CastPlayerHeal(this);
        m_castSkill = Castable.CreateCast(m_skillDataList[0].castableName, this);
        UIAbilityIconSkill orig = Resources.Load<UIAbilityIconSkill>("Abilities/@AbilityIconSkill_Supporter");
        ObjectPool<UIAbilityIcon>.CreatePool("Ability" + m_skillDataList[0].castableName, orig.cachedGameObject, 10, (UIAbilityIcon icon)=>
        {
            ((UIAbilityIconSkill)icon).Init(this, m_skillDataList[0]);
        });
        //UIAbilitySlot.Instance.Add("Ability" + m_skillDataList[0].castableName);

        m_fHP = MaxHP = 10.0f;

        base.Init();

        m_HPGauge.Hide();

        m_following = CharacterPlayer.PlayerList[0];
        m_vecOffset = new Vector3(-0.75f * GameManager.Instance.Direction, 1.0f);
    }

    protected override void IdleThought()
    {
        // if(m_castAttack?.GetTargets()?.Length > 0)
        // {
        //     Attack();
        // }
        // else if(m_fKnockBack == 0.0f)
        {
            State = STATE.MOVE;
        }
    }

    protected override IEnumerator MOVE()
    {
        //PlayAnimation(GetRunAnimation(), false, true);
        while(State == STATE.MOVE)
        {
            
            // if(m_castAttack.GetTargets().Length > 0)
            // {
            //     Attack();
            //     yield return null;
            //     break;
            // }
            
            Vector3 pos = Vector3.Lerp(position, CharacterPlayer.PlayerList[0].position + m_vecOffset, 3.0f * Time.smoothDeltaTime);
            position = pos;
            
            yield return null;
        }
        
        NextState();
    }
}