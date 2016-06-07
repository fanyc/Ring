using System;
using System.Numerics;
using UnityEngine;

public class Upgrade
{
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
        return m_Eval(level);
    }
    
    public Upgrade(Func<int, BigDecimal> eval)
    {
        m_Eval = eval;
    }
}