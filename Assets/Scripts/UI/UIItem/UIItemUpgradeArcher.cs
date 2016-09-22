public class UIItemUpgradeArcher : UIItem {
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        Init();
    }
    
    public override void Init()
    {
        base.Init();
        Upgrade damage = UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage");
        Upgrade speed = UpgradeManager.Instance.GetUpgrade("ArcherAttackSpeed");
        Upgrade price = UpgradeManager.Instance.GetUpgrade("ArcherAttackPrice");
        
        Name.text = $"<color=#6F5151FF>Archer</color>\n<size=+2>Lv. <color=#D75A23FF>{damage.Level}</color>";
        Desc.text = $"<size=42><sprite=1></size> {(damage.currentValue + 2.0f).ToString()}";
        
        ButtonText.text =
        $"<size=42><sprite=1></size>+{(damage.nextValue - damage.currentValue).ToString()} \n<pos=-6><size=44><sprite=2><size=16> <size=30><color=#391E0EFF>{price.currentValue.ToString()}</color></size>";
    }
    public override void _Function()
    {
        float price = UpgradeManager.Instance.GetUpgrade("ArcherAttackPrice").currentValue;
        
        if(UIWallet.Instance.Gold >= price)
        {
            UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").Level++;
            UpgradeManager.Instance.GetUpgrade("ArcherAttackSpeed").Level++;
            UpgradeManager.Instance.GetUpgrade("ArcherAttackPrice").Level++;
            
            UIWallet.Instance.Gold -= price;
            
            Init();
        }
    }
    
}
