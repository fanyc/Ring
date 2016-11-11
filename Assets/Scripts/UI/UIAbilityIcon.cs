using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAbilityIcon : ObjectBase {
    
    protected RectTransform cachedRectTransform;
    public enum STATE
    {
        NULL = 0,
        INIT,
        MOVE,
        READY,
        RELEASE
    }

	public Image ImageEnable;
	public Image ImageDisable;
    
    public Image Frame;

    public Color[] FrameColor = new Color[2];
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
        cachedRectTransform = GetComponent<RectTransform>();
        m_cachedButton = GetComponent<Button>();
    }

    public virtual void Init()
    {
        m_State = STATE.INIT;
        SetEnable(UIAbilitySlot.Instance.MP >= Cost);
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
        int slot = (UIAbilitySlot.Instance.Slot - m_nSlot) - 1;
        float dist = 220.0f * slot - position.x;
        float speed = 220.0f * 15.0f;
        yield return null;
        
        while(dist > speed * Time.deltaTime)
        {
            position += new Vector2(speed * Time.deltaTime, 0.0f);
            dist = 220.0f * slot - position.x;
            rt.anchoredPosition = position;
            yield return null;
        }   

        rt.anchoredPosition = new Vector2(220.0f * slot, 0.0f);
        m_State = STATE.READY;

    }
    
    public virtual void Use()
    {
        UIAbilitySlot.Instance.MP -= Cost;
        UIAbilitySlot.Instance.Remove(this);
    }

    public virtual void Update()
    {
        SetEnable(UIAbilitySlot.Instance.MP >= Cost);
    }

    public void SetEnable(bool enable)
    {
        if(m_cachedButton.enabled != enable)
        {
            m_cachedButton.enabled = enable;

            ImageEnable.enabled = enable;
            ImageDisable.enabled = !enable;

            Frame.color = FrameColor[(enable ? 0 : 1)];
        }
    }
}
