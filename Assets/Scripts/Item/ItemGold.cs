using System.Numerics;
using UnityEngine;

public class ItemGold : Item {
    
    protected BigDecimal m_fAmount;
    
    void Awake()
    {
        m_tfTarget = UIWallet.Instance.imgGold.transform;
    }
    public void Init(BigDecimal amount)
    {
        base.Init();
        m_fAmount = amount;
    }
    protected override void Release()
    {
        UIWallet.Instance.Gold += m_fAmount;
        base.Release();
    }
}
