using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAbilityIcon : ObjectBase {
    
    public CharacterPlayer Caster;
	public Image ImageEnable;
	public Image ImageDisable;
    
    public Image InnerFrameEnable;
    public Image InnerFrameDisable;
    public float Cost = 0.0f;
    
    protected Button m_cachedButton;
    
    protected virtual void Awake()
    {
        m_cachedButton = GetComponent<Button>();

        ImageEnable.enabled = false;
        ImageDisable.enabled = true;

        InnerFrameEnable.enabled = false;
        InnerFrameDisable.enabled = true;
        
        m_cachedButton.enabled = false;
    }
    
    public virtual void Use()
    {
        UIAbilitySlot.Instance.MP -= Cost;
    }

    public virtual void Update()
    {
        if(UIAbilitySlot.Instance.MP >= Cost)
        {
            ImageEnable.enabled = false;
            ImageDisable.enabled = true;

            InnerFrameEnable.enabled = false;
            InnerFrameDisable.enabled = true;

            m_cachedButton.enabled = true;
        }
        else
        {
            ImageEnable.enabled = false;
            ImageDisable.enabled = true;

            InnerFrameEnable.enabled = false;
            InnerFrameDisable.enabled = true;

            m_cachedButton.enabled = false;
        }
    }
}
