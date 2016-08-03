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
        m_skillDataList.AddSkillData("파 크라이", "WarriorSkill", "skill_a01", "SkillIcon/btle_icskill_wri_01b", "CastPlayerWarriorSkill");
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
            
            Vector3 pos = cachedTransform.position + new Vector3(11.25f * Time.smoothDeltaTime * 0.4f * GameManager.Instance.Direction, 0.0f);
            
            cachedTransform.position = pos;
            
            yield return null;
        }
        
        NextState();
    }
}