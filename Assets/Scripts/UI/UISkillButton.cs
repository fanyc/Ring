using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISkillButton : ObjectBase {
    
    public CharacterPlayer Caster;
	public Image ImageGauge;
    
    protected Button m_cachedButton;
    
    void Awake()
    {
        m_cachedButton = GetComponent<Button>();
    }
    
    public virtual void CastSkill()
    {
        Castable skill = Caster.GetSkill();
        if(skill.Condition())
        {
            ImageGauge.fillAmount = 1.0f;
            m_cachedButton.enabled = false;
            
            Caster.MP = 0.0f;
            Caster.Cast(skill);
        }
    }
    
    void Update()
    {
        if(m_cachedButton.enabled == false)
        {
            float per = Mathf.Clamp01(Caster.MP / Caster.GetSkill().Cost);
            ImageGauge.fillAmount = 1.0f - per;
            if(per >= 1.0f)
            {
                m_cachedButton.enabled = true;
            }
        }
    }
}
