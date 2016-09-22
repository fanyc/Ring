using System.Collections.Generic;
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
       
       m_dictUpgrade.Add("WarriorAttackDamage", new Upgrade((int level)=>{ return (1.0f + Mathf.Pow(10.0f, Damage.Evaluate( Mathf.Max(0, (level - 1) * 3 - 2 )) ) ) * CharacterPlayerWarrior.AttackPerSecond; }));
       m_dictUpgrade.Add("WarriorAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 2 )) ) - Mathf.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 2 ) - 1 ) )) * 10.0f; }));
       m_dictUpgrade.Add("WarriorSkillDamage", new Upgrade((int level)=>{ return 5.0f + (level - 1) * 0.5f; }));
       m_dictUpgrade.Add("WarriorSkillPrice", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Price.Evaluate( level * 10 )) - Mathf.Pow(10.0f, Price.Evaluate( ( level - 1 ) * 10 )); }));
       
       
       m_dictUpgrade.Add("ArcherAttackDamage", new Upgrade((int level)=>{ return (1.0f + Mathf.Pow(10.0f, Damage.Evaluate( Mathf.Max(0, (level - 1) * 3 - 1 )) ) ) * CharacterPlayerArcher.AttackPerSecond; }));
       m_dictUpgrade.Add("ArcherAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 1 )) ) - Mathf.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 1) - 1 ) )) * 10.0f; }));
       m_dictUpgrade.Add("ArcherSkillDamage", new Upgrade((int level)=>{ return 5.0f + (level - 1) * 0.5f; }));
       m_dictUpgrade.Add("ArcherSkillPrice", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Price.Evaluate( level * 10 )) - Mathf.Pow(10.0f, Price.Evaluate( ( level - 1 ) * 10 )); }));
       
       m_dictUpgrade.Add("SoceressAttackDamage", new Upgrade((int level)=>{ return (1.0f + Mathf.Pow(10.0f, Damage.Evaluate( Mathf.Max(0, (level - 1) * 3 - 0 )) ) ) * CharacterPlayerMage.AttackPerSecond; }));
       m_dictUpgrade.Add("SoceressAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 0 )) ) - Mathf.Pow(10.0f, Price.Evaluate( Mathf.Max(0, (level) * 3 - 0) - 1 ) )) * 10.0f; }));
       m_dictUpgrade.Add("SoceressSkillDamage", new Upgrade((int level)=>{ return 5.0f + (level - 1) * 0.5f; }));
       m_dictUpgrade.Add("SoceressSkillPrice", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Price.Evaluate( level * 10 )) - Mathf.Pow(10.0f, Price.Evaluate( ( level - 1 ) * 10 )); }));
       
    //    m_dictUpgrade.Add("WarriorAttackDamage", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Damage.Evaluate( level - 1 )) / 3.0f * 10.0f ; }));
    //    m_dictUpgrade.Add("WarriorAttackSpeed", new UpgradeWarrior());
    //    m_dictUpgrade.Add("WarriorAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( level )) - Mathf.Pow(10.0f, Price.Evaluate( level - 1 ) )) * 10.0f / 3.0f; } ));
       
    //    m_dictUpgrade.Add("ArcherAttackDamage", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Damage.Evaluate( level - 1 )) * 2.0f / 3.0f * 10.0f ; }));
    //    m_dictUpgrade.Add("ArcherAttackSpeed", new UpgradeWarrior());
    //    m_dictUpgrade.Add("ArcherAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( level )) - Mathf.Pow(10.0f, Price.Evaluate( level - 1 ) )) * 10.0f / 3.0f; } ));
       
    //    m_dictUpgrade.Add("MageAttackDamage", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, Damage.Evaluate( level - 1 )) * 5.0f / 3.0f * 10.0f ; }));
    //    m_dictUpgrade.Add("MageAttackSpeed", new UpgradeWarrior());
    //    m_dictUpgrade.Add("MageAttackPrice", new Upgrade((int level)=>{ return (Mathf.Pow(10.0f, Price.Evaluate( level )) - Mathf.Pow(10.0f, Price.Evaluate( level - 1 ) )) * 10.0f / 3.0f; } ));
    
       m_dictUpgrade.Add("EnemyHP", new Upgrade((int level)=>{ return Mathf.Pow(10.0f, EnemyHP.Evaluate(level - 1)); }));
       m_dictUpgrade.Add("Reward", new Upgrade((int level)=>{ return 
       ( Mathf.Pow( 10.0f, Reward.Evaluate( level ) ) - Mathf.Pow( 10.0f, Reward.Evaluate( level-1 ) ) ) * 10.0f; }));
       
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
