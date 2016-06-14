using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISkillButton : ObjectBase {
    
    public CharacterPlayer Caster;
	public Image ImageGauge;

    public TMPro.TextMeshProUGUI textCoolTime;

    public TMPro.TextMeshProUGUI textLevel;
    
    
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

            textCoolTime.gameObject.SetActive(true);
            textCoolTime.text = Caster.GetSkill().Cost.ToString("0");
        }
    }
    
    void Update()
    {
        if(m_cachedButton.enabled == false)
        {
            float cost = Caster.GetSkill().Cost;
            float mp = Caster.MP;
            if(cost - mp >= 1.0f)
            {
                textCoolTime.text = (cost - mp).ToString("0");
            }
            else
            {
                textCoolTime.text = Mathf.Clamp01(cost - mp).ToString("0.0");
            }
            float per = Mathf.Clamp01(mp / cost);
            ImageGauge.fillAmount = 1.0f - per;
            if(per >= 1.0f)
            {
                textCoolTime.gameObject.SetActive(false);
                m_cachedButton.enabled = true;
            }
        }
    }
}
