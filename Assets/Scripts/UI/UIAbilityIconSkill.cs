using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAbilityIconSkill : UIAbilityIcon {

    protected Character m_Caster;
    protected CharacterPlayer.SkillData m_SkillData; 
    protected Castable m_castSkill;

    public void Init(Character caster, CharacterPlayer.SkillData skillData)
    {
        base.Init();
        
        m_Caster = caster;
        m_SkillData = skillData;
        m_castSkill = Castable.CreateCast(skillData.castableName, caster);

        Cost = m_castSkill.Cost;
    }

    protected override void Awake()
    {
        base.Awake();
    }
    
    public override void Use()
    {
        base.Use();
        m_Caster?.Cast(m_castSkill);
    }
}
