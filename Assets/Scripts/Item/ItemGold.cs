public class ItemGold : Item {
    
    protected float m_fAmount;
    
    void Awake()
    {
        m_tfTarget = UIWallet.Instance.imgGold.transform;
    }
    public void Init(float amount)
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
