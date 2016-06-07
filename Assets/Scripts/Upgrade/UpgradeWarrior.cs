using UnityEngine;

public class UpgradeWarrior : Upgrade
{
    public UpgradeWarrior() : base(null)
    {
        
    }
    // public override float GetValue(int level)
    // {
    //     if(level <= 0) return 1.0f;
    //     return 1.0f * Mathf.Pow(1.1f, (float)(level - 1));
    // }
}