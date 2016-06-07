using System.Numerics;

public class UIItemUpgradeMage : UIItem {
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        Init();
    }
    
    public override void Init()
    {
        base.Init();
        Upgrade damage = UpgradeManager.Instance.GetUpgrade("MageAttackDamage");
        Upgrade speed = UpgradeManager.Instance.GetUpgrade("MageAttackSpeed");
        Upgrade price = UpgradeManager.Instance.GetUpgrade("MageAttackPrice");
        
        Name.text = $"<color=#6F5151FF>Soceress</color>\n<size=+2>Lv. <color=#D75A23FF>{damage.Level}</color>";
        Desc.text = $"<size=42><sprite=1></size> {(damage.currentValue + 5.0f).ToUnit()}";
        
        ButtonText.text =
        $"<size=42><sprite=1></size>+{(damage.nextValue - damage.currentValue).ToUnit()} \n<pos=-6><size=44><sprite=2><size=16> <size=30><color=#391E0EFF>{price.currentValue.ToUnit()}</color></size>";
    }
    public override void _Function()
    {
        BigDecimal price = UpgradeManager.Instance.GetUpgrade("MageAttackPrice").currentValue;
        
        if(GameManager.Instance.Gold >= price)
        {
            UpgradeManager.Instance.GetUpgrade("MageAttackDamage").Level++;
            UpgradeManager.Instance.GetUpgrade("MageAttackSpeed").Level++;
            UpgradeManager.Instance.GetUpgrade("MageAttackPrice").Level++;
            
            GameManager.Instance.Gold -= price;
            
            Init();
        }
    }
    
}
