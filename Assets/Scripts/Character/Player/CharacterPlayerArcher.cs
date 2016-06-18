using System.Collections.Generic;

public class CharacterPlayerArcher : CharacterPlayer
{
    protected static SkillDataList m_skillDataList = new SkillDataList();

    public override SkillDataList ListSkillData
    {
        get {return m_skillDataList;}
    }

    static CharacterPlayerArcher()
    {
        m_skillDataList.AddSkillData("파 크라이", "ArcherSkill", "skill_01");
        m_skillDataList.AddSkillData("파 크라이", "ArcherSkill", "skill_01");
        m_skillDataList.AddSkillData("파 크라이", "ArcherSkill", "skill_01");
        m_skillDataList.AddSkillData("파 크라이", "ArcherSkill", "skill_01");
        m_skillDataList.AddSkillData("파 크라이", "ArcherSkill", "skill_01");
        m_skillDataList.AddSkillData("파 크라이", "ArcherSkill", "skill_01");
        m_skillDataList.AddSkillData("파 크라이", "ArcherSkill", "skill_01");
        
    }

    public new static float AttackPerSecond
    {
        get
        {
            return 0.867f;
        }
    }

    public override void Init()
    {
        base.Init();
        m_castAttack = new CastPlayerArcherAttack(this);
        m_castSkill = new CastPlayerArcherSkill(this);
    }

    protected override string GetRunAnimation()
    {
        return "run_02";
    }
}