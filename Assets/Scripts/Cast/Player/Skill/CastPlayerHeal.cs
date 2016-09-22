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

        for(int i = 0, c = list.Count; i < c; ++i)
        {
            list[i].Heal(10.0f);
        }

        yield break;
    }
    
    protected override void Release()
    {
        base.Release();
    }
}