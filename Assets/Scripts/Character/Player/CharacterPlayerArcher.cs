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
        m_skillDataList.AddSkillData("매그넘 샷", "ArcherSkill", "skill_01", "SkillIcon/btle_icskill_elf_01b");
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