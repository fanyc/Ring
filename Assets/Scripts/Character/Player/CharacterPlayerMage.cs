using System.Collections.Generic;

public class CharacterPlayerMage : CharacterPlayer
{
    protected static SkillDataList m_skillDataList = new SkillDataList();

    public override SkillDataList ListSkillData
    {
        get {return m_skillDataList;}
    }

    static CharacterPlayerMage()
    {
        m_skillDataList.AddSkillData("미티어 스트라이크", "SoceressSkill", "skill_01", "SkillIcon/btle_icskill_sor_01b");
    }


    public new static float AttackPerSecond
    {
        get
        {
            return 1.867f;
        }
    }

    public override void Init()
    {
        base.Init();
        m_castAttack = new CastPlayerMageAttack(this);
        m_castSkill = new CastPlayerMageSkill(this);
    }

    protected override string GetRunAnimation()
    {
        return "run_02";
    }
}