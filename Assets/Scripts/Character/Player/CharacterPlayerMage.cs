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
        m_skillDataList.AddSkillData("파 크라이", "SoceressSkill", "skill_01");
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