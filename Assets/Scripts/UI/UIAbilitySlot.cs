using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIAbilitySlot : MonoSingleton<UIAbilitySlot>
{
    public float MP = 4.0f;
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
    protected int m_nSlotCount = 6;

    protected List<UIAbilityIcon> m_listSlot;

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
        rt.anchoredPosition = new Vector2(-159.0f, 0.0f);

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
        MP = Mathf.Clamp(MP + Time.deltaTime, 0.0f, MPMax);
        MPGauge.fillAmount = MP / MPMax;
    }
}