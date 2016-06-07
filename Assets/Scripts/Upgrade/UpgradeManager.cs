using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    
    public AnimationCurve EnemyHP;
    public AnimationCurve Damage;
    public AnimationCurve Reward;
    public AnimationCurve Price;
    protected Dictionary<string, Upgrade> m_dictUpgrade = new Dictionary<string, Upgrade>();
    
   public UpgradeManager()
   {
       
       m_dictUpgrade.Add("WarriorAttackDamage", new Upgrade((int level)=>{ return 1.0f + BigDecimal.Pow(10.0f, Damage.Evaluate( Mathf.Max(0, (level - 1) * 3 - 2 )) ) * 1.0f / 3.0f; }));
       m_dictUpgrade.Add("WarriorAttackPrice", new Upgrade((int level)=>{ return (BigDecimal.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 2 )) ) - BigDecimal.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 2 ) - 1 ) )) * 10.0f / 3.0f; }));
       
       m_dictUpgrade.Add("ArcherAttackDamage", new Upgrade((int level)=>{ return 2.0f + BigDecimal.Pow(10.0f, Damage.Evaluate( Mathf.Max(0, (level - 1) * 3 - 1 )) ) * 1.0f / 3.0f; }));
       m_dictUpgrade.Add("ArcherAttackPrice", new Upgrade((int level)=>{ return (BigDecimal.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 1 )) ) - BigDecimal.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 1) - 1 ) )) * 10.0f / 3.0f; }));
       
       m_dictUpgrade.Add("SoceressAttackDamage", new Upgrade((int level)=>{ return 5.0f + BigDecimal.Pow(10.0f, Damage.Evaluate( Mathf.Max(0, (level - 1) * 3 - 0 )) ) * 1.0f / 3.0f; }));
       m_dictUpgrade.Add("SoceressAttackPrice", new Upgrade((int level)=>{ return (BigDecimal.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 0 )) ) - BigDecimal.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 0) - 1 ) )) * 10.0f / 3.0f; }));
       
    //    m_dictUpgrade.Add("WarriorAttackDamage", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Damage.Evaluate( level - 1 )) / 3.0f * 10.0f ; }));
    //    m_dictUpgrade.Add("WarriorAttackSpeed", new UpgradeWarrior());
    //    m_dictUpgrade.Add("WarriorAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( level )) - Mathf.Pow(10.0f, Price.Evaluate( level - 1 ) )) * 10.0f / 3.0f; } ));
       
    //    m_dictUpgrade.Add("ArcherAttackDamage", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Damage.Evaluate( level - 1 )) * 2.0f / 3.0f * 10.0f ; }));
    //    m_dictUpgrade.Add("ArcherAttackSpeed", new UpgradeWarrior());
    //    m_dictUpgrade.Add("ArcherAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( level )) - Mathf.Pow(10.0f, Price.Evaluate( level - 1 ) )) * 10.0f / 3.0f; } ));
       
    //    m_dictUpgrade.Add("MageAttackDamage", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Damage.Evaluate( level - 1 )) * 5.0f / 3.0f * 10.0f ; }));
    //    m_dictUpgrade.Add("MageAttackSpeed", new UpgradeWarrior());
    //    m_dictUpgrade.Add("MageAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( level )) - Mathf.Pow(10.0f, Price.Evaluate( level - 1 ) )) * 10.0f / 3.0f; } ));
    
       m_dictUpgrade.Add("EnemyHP", new Upgrade((int level)=>{ return BigDecimal.Pow(10.0f, EnemyHP.Evaluate(level - 1)) * 10.0f ; }));
       m_dictUpgrade.Add("Reward", new Upgrade((int level)=>{ return 
       ( BigDecimal.Pow( 10.0, Reward.Evaluate( level ) ) - BigDecimal.Pow( 10.0, Reward.Evaluate( level-1 ) ) ) * 10.0f; }));
       
   }
        
    public Upgrade GetUpgrade(string key)
    {
        Upgrade ret = m_dictUpgrade[key];
        
        // if(m_dictUpgrade.TryGetValue(key, out ret) == false)
        // {
        //     //ret = 
        // }
        
        return ret;
    }
}
