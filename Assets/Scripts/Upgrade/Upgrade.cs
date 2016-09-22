using System;
using System.Collections.Generic;

public class Upgrade
{
    protected Dictionary<int, float> m_dictCache = new Dictionary<int, float>();
    protected Func<int, float> m_Eval;
    public int Level
    {
        get;
        set;
    } = 1;
    
    public float currentValue
    {
        get
        {
            return GetValue(Level);
        }
    }
    
    public float prevValue
    {
        get
        {
            return GetValue(Level - 1);
        }
    }
    
    public float nextValue
    {
        get
        {
            return GetValue(Level + 1);
        }
    }
    
    public virtual float GetValue(int level)
    {
        float val;
        if(m_dictCache.TryGetValue(level, out val) == false)
        {
            val = m_Eval(level);
            m_dictCache[level] = val;
        }
        return val;
    }
    
    public Upgrade(Func<int, float> eval)
    {
        m_Eval = eval;
        m_dictCache.Clear();
    }
}