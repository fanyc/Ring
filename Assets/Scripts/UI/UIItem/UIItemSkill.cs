using System.Numerics;

public class UIItemSkill : UIItem {
    
    protected CharacterPlayer.SkillData m_SkillData;

    public void Init(CharacterPlayer.SkillData skillData)
    {
        m_SkillData = skillData;
        Init();
    }

    public override void Init()
    {
        base.Init();
        
        Upgrade damage = UpgradeManager.Instance.GetUpgrade(m_SkillData.upgradeGroup + "Damage");
        Upgrade price = UpgradeManager.Instance.GetUpgrade(m_SkillData.upgradeGroup + "Price");
        
        Name.text = $"<color=#6F5151FF>{m_SkillData.name}</color>\n<size=+2>Lv. <color=#D75A23FF>{damage.Level}</color>";
        Desc.text = $"<size=42><sprite=1></size> {(damage.currentValue.ToType<float>()):0%}";
        
        ButtonText.text =
        $"<size=42><sprite=1></size>+{(damage.nextValue - damage.currentValue).ToType<float>():0%} \n<pos=-6><size=44><sprite=2><size=16> <size=30><color=#391E0EFF>{price.currentValue.ToUnit()}</color></size>";
    }

    public override void _Function()
    {
        BigDecimal price = UpgradeManager.Instance.GetUpgrade(m_SkillData.upgradeGroup + "Price").currentValue;
        
        if(UIWallet.Instance.Gold >= price)
        {
            UpgradeManager.Instance.GetUpgrade(m_SkillData.upgradeGroup + "Damage").Level++;
            UpgradeManager.Instance.GetUpgrade(m_SkillData.upgradeGroup + "Price").Level++;
            
            UIWallet.Instance.Gold -= price;
            
            Init();
        }
    }
    
}
