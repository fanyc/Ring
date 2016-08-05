using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAbilityIcon : ObjectBase {
    
    public enum STATE
    {
        NULL = 0,
        INIT,
        MOVE,
        READY,
        RELEASE
    }

    public CharacterPlayer Caster;
	public Image ImageEnable;
	public Image ImageDisable;
    
    public Image InnerFrameEnable;
    public Image InnerFrameDisable;
    public float Cost = 0.0f;

    protected int m_nSlot;
    public int Slot
    {
        set{ m_nSlot = value; }
        get{ return m_nSlot; }
    }
    
    protected STATE m_State = STATE.NULL;
    public STATE State
    {
        get
        {
            return m_State;
        }
    }

    protected Button m_cachedButton;
    
    protected virtual void Awake()
    {
        m_cachedButton = GetComponent<Button>();
    }

    public virtual void Init()
    {
        m_State = STATE.INIT;
    }

    public void Move()
    {
        m_State = STATE.MOVE;
        StopCoroutine("_move");
        StartCoroutine("_move");
    }
    
    IEnumerator _move()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 position = rt.anchoredPosition;
        float dist = position.x - 159.0f * m_nSlot;
        float speed = 159.0f * 15.0f;
        yield return null;
        
        while(dist > speed * Time.deltaTime)
        {
            position -= new Vector2(speed * Time.deltaTime, 0.0f);
            dist = position.x - 159.0f * m_nSlot;
            rt.anchoredPosition = position;
            yield return null;
        }   

        rt.anchoredPosition = new Vector2(159.0f * m_nSlot, 0.0f);
        m_State = STATE.READY;

    }
    
    public virtual void Use()
    {
        UIAbilitySlot.Instance.MP -= Cost;
        UIAbilitySlot.Instance.Remove(this);
        UIAbilitySlot.Instance.SpawnIcon();
    }

    public virtual void Update()
    {
        if(UIAbilitySlot.Instance.MP >= Cost)
        {
            SetEnable(true);
        }
        else
        {
            SetEnable(false);
        }
    }

    public void SetEnable(bool enable)
    {
        if(m_cachedButton.enabled != enable)
        {
            m_cachedButton.enabled = enable;

            ImageEnable.enabled = enable;
            ImageDisable.enabled = !enable;

            InnerFrameEnable.enabled = enable;
            InnerFrameDisable.enabled = !enable;
        }
    }
}
