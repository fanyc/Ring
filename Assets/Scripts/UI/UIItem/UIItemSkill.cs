using System.Numerics;

public class UIItemSkill : UIItem {
    
    public string StrUpgradeGroup;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        Init();
    }
    
    public override void Init()
    {
        base.Init();
        Upgrade damage = UpgradeManager.Instance.GetUpgrade(StrUpgradeGroup + "Damage");
        Upgrade price = UpgradeManager.Instance.GetUpgrade(StrUpgradeGroup + "Price");
        
        Name.text = $"<color=#6F5151FF>Skill</color>\n<size=+2>Lv. <color=#D75A23FF>{damage.Level}</color>";
        Desc.text = $"<size=42><sprite=1></size> {(damage.currentValue.ToType<float>()):0%}";
        
        ButtonText.text =
        $"<size=42><sprite=1></size>+{(damage.nextValue - damage.currentValue).ToType<float>():0%} \n<pos=-6><size=44><sprite=2><size=16> <size=30><color=#391E0EFF>{price.currentValue.ToUnit()}</color></size>";
    }
    public override void _Function()
    {
        BigDecimal price = UpgradeManager.Instance.GetUpgrade(StrUpgradeGroup + "Price").currentValue;
        
        if(GameManager.Instance.Gold >= price)
        {
            UpgradeManager.Instance.GetUpgrade(StrUpgradeGroup + "Damage").Level++;
            UpgradeManager.Instance.GetUpgrade(StrUpgradeGroup + "Price").Level++;
            
            GameManager.Instance.Gold -= price;
            
            Init();
        }
    }
    
}
