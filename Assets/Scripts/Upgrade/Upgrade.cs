using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    protected Dictionary<int, BigDecimal> m_dictCache = new Dictionary<int, BigDecimal>();
    protected Func<int, BigDecimal> m_Eval;
    public int Level
    {
        get;
        set;
    } = 1;
    
    public BigDecimal currentValue
    {
        get
        {
            return GetValue(Level);
        }
    }
    
    public BigDecimal prevValue
    {
        get
        {
            return GetValue(Level - 1);
        }
    }
    
    public BigDecimal nextValue
    {
        get
        {
            return GetValue(Level + 1);
        }
    }
    
    public virtual BigDecimal GetValue(int level)
    {
        BigDecimal val;
        if(m_dictCache.TryGetValue(level, out val) == false)
        {
            val = m_Eval(level);
            m_dictCache[level] = val;
        }
        return val;
    }
    
    public Upgrade(Func<int, BigDecimal> eval)
    {
        m_Eval = eval;
        m_dictCache.Clear();
    }
}