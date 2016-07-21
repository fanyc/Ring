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
        m_skillDataList.AddSkillData("파 크라이", "WarriorSkill", "skill_a01", "SkillIcon/btle_icskill_wri_01b");
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
        base.Init();
        m_castAttack = new CastPlayerWarriorAttack(this);
        m_castSkill = new CastPlayerWarriorSkill(this);
    }
}