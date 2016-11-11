using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerHeal : Castable
{
    public override float Cost
    {
        get { return 1.0f; }
    }

    private int m_cachedMask = 0;
    public override int TargetMask
    {
        get { if(m_cachedMask == 0) m_cachedMask = 1 << LayerMask.NameToLayer("Enemy"); return m_cachedMask; }
    }

    public override Vector2 Rect
    {
        get
        {
            return new Vector2(15.0f * m_caster.Direction, 1.5f);
        }
    }

    public CastPlayerHeal(Character caster) : base(caster)
    {
    }
    public override bool Condition()
    {
        if(IsCoolTime()) return false;
        return true;
    }
    
    protected override void Prepare()
    {
    }
    protected override IEnumerator Cast()
    {
        List<CharacterPlayer> list = CharacterPlayer.PlayerList;

        if(list.Count > 0)
        {
            CharacterPlayer lowest = list[0];

            for(int i = 1, c = list.Count; i < c; ++i)
            {
                if(lowest.HP / lowest.MaxHP > list[i].HP / list[i].MaxHP)
                {
                    lowest = list[i];
                }
            }
            lowest.Heal(100.0f);
            ObjectPool<Effect>.Spawn("@Effect_Heal_Target", Vector3.zero, lowest.cachedTransform).Init(Vector3.zero);
        }
        
        yield break;
    }
    
    protected override void Release()
    {
        base.Release();
    }
}