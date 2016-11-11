using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIAbilitySlot : MonoSingleton<UIAbilitySlot>
{
    protected float m_fMP = 4.0f;
    public float MP
    {
        set
        {
            m_fMP = Mathf.Clamp(value, 0.0f, MPMax);
        }

        get
        {
            return m_fMP;
        }
    }
    public float MPMax = 10.0f;

    public Image MPGauge;
    protected List<string> m_listAbilityKey = new List<string>();

    public int Slot
    {
        get
        {
            return m_nSlotCount;
        }
    }
    protected int m_nSlotCount = 8;

    protected List<UIAbilityIcon> m_listSlot;

    protected bool m_isTimerEnable = false;
    protected float m_fTimer = 0.0f;

    public void Init()
    {
        if(m_listSlot != null)
        {
            for(int i = 0, c = m_listSlot.Count; i < c; ++i)
            {
                m_listSlot[i].Recycle();
            }

            m_listSlot.Clear();
        }

        m_listSlot = new List<UIAbilityIcon>(m_nSlotCount);
        m_isTimerEnable = false;
        StartCoroutine(_init());

    }

    IEnumerator _init()
    {
        for(int i = 0; i < m_nSlotCount; ++i)
        { 
            UIAbilityIcon icon = SpawnIcon();

            while(icon.State == UIAbilityIcon.STATE.MOVE)
                yield return null;
        }

        m_isTimerEnable = true;
    }

    public void Add(string name)
    {   
        m_listAbilityKey.Add(name);
    }

    public UIAbilityIcon SpawnIcon()
    {
        UIAbilityIcon icon = ObjectPool<UIAbilityIcon>.Spawn(m_listAbilityKey[Random.Range(0, m_listAbilityKey.Count)]);
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.SetParent(cachedTransform, false);
        rt.anchoredPosition = new Vector2(-220.0f, 0.0f);
        icon.Init();

        icon.Slot = m_listSlot.Count;
        icon.Move();

        m_listSlot.Add(icon);
        
        return icon;
    }

    public void Remove(UIAbilityIcon icon)
    {
        int i = 0;
        for(; i < m_listSlot.Count; ++i)
        {
            if(icon == m_listSlot[i])
            {
                break;
            }
        }

        for(; i < m_listSlot.Count - 1; ++i)
        {
            m_listSlot[i] = m_listSlot[i + 1];
            if(m_listSlot[i] != null)
            {
                m_listSlot[i].Slot = i;
                m_listSlot[i].Move();
            }
        }

        m_listSlot.RemoveAt(m_listSlot.Count - 1);
        icon.Recycle();
    }


    void Update()
    {
        MP += Time.deltaTime;
        MPGauge.fillAmount = MP / MPMax;

        if(m_listSlot.Count < m_nSlotCount)
        {
            if(m_isTimerEnable)
            {
                m_fTimer += Time.deltaTime;
            }

            if(m_fTimer >= 2.0f)
            {
                UIAbilitySlot.Instance.SpawnIcon();
                m_fTimer = 0.0f;
            }
        }
        else
        {
            m_fTimer = 0.0f;
        }
    }
}