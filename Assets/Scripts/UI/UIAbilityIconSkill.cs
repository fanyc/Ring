using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIAbilityIconSkill : UIAbilityIcon {
    protected static Dictionary<string, Sprite> m_dictIconCache = new Dictionary<string, Sprite>();

    protected Character m_Caster;
    protected CharacterPlayer.SkillData m_SkillData; 
    protected Castable m_castSkill;

    public void Init(Character caster, CharacterPlayer.SkillData skillData)
    {
        base.Init();
        
        m_Caster = caster;
        m_SkillData = skillData;
        m_castSkill = Castable.CreateCast(skillData.castableName, caster);

        Sprite iconSprite = null;

        if(m_dictIconCache.TryGetValue(skillData.thumbnailName, out iconSprite) == false)
        {
            iconSprite = Resources.Load<Sprite>(skillData.thumbnailName);
            m_dictIconCache.Add(skillData.thumbnailName, iconSprite);
        }

        ImageEnable.sprite = ImageDisable.sprite = iconSprite;

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
