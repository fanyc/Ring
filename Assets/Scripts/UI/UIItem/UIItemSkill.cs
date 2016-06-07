using System.Numerics;

public class UIItemSkill : UIItem {
    
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
        
        Name.text = $"<color=#6F5151FF>Archer</color>\nLv. <color=#D75A23FF>{damage.Level}</color>";
        Desc.text = $"ATK  {(damage.currentValue + 2.0f).ToUnit()}";
        
        ButtonText.text =
        $" ATK+{(damage.nextValue - damage.currentValue).ToUnit()}\n COIN <size=32><color=#391E0EFF>{price.currentValue.ToUnit()}</color></size>";
    }
    public override void _Function()
    {
        BigDecimal price = UpgradeManager.Instance.GetUpgrade("ArcherAttackPrice").currentValue;
        
        if(GameManager.Instance.Gold >= price)
        {
            UpgradeManager.Instance.GetUpgrade("ArcherAttackDamage").Level++;
            UpgradeManager.Instance.GetUpgrade("ArcherAttackSpeed").Level++;
            UpgradeManager.Instance.GetUpgrade("ArcherAttackPrice").Level++;
            
            GameManager.Instance.Gold -= price;
            
            Init();
        }
    }
    
}
