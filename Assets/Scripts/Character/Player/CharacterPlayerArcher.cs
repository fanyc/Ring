using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPlayerArcher : CharacterPlayer
{
    protected static SkillDataList m_skillDataList = new SkillDataList();

    public override SkillDataList ListSkillData
    {
        get {return m_skillDataList;}
    }

    static CharacterPlayerArcher()
    {
        m_skillDataList.AddSkillData("매그넘 샷", "ArcherSkill", "skill_01", "SkillIcon/btle_icskill_elf_01b", "CastPlayerArcherSkill");
    }

    public new static float AttackPerSecond
    {
        get
        {
            return 0.867f;
        }
    }


    public override void Init()
    {
        m_castAttack = new CastPlayerArcherAttack(this);
        m_castSkill = Castable.CreateCast(m_skillDataList[0].castableName, this);

        base.Init();
    }

    protected override void IdleThought()
    {
        Character[] targets = m_castAttack.GetTargets();
        
        if(targets?.Length > 0)
        {
            Character target = Castable.GetNearestTarget(this, targets);
            float minDistance =
            Mathf.Min(m_castAttack.MinDistance - 1.0f,
                      Mathf.Abs((GameManager.Instance.cachedTransform.position.x - GameManager.Instance.LimitDistance * GameManager.Instance.Direction) - target.cachedTransform.position.x));
                                
            if(minDistance <= Mathf.Abs(target.cachedTransform.position.x - cachedTransform.position.x))
            {
                Attack();
                return;
                
            }
        }

        State = STATE.MOVE;
    }

    protected override IEnumerator MOVE()
    {
        PlayAnimation(GetRunAnimation(), false, true);
        while(State == STATE.MOVE)
        {
            Vector3 pos = cachedTransform.position;
            float step = 11.25f * Time.smoothDeltaTime * 0.4f;
            Character[] targets = m_castAttack.GetTargets();
            if(targets?.Length > 0)
            {
                Character target = Castable.GetNearestTarget(this, targets);
                float dist = target.cachedTransform.position.x - pos.x;
                float dest = Mathf.Min(m_castAttack.MinDistance,
                Mathf.Abs((GameManager.Instance.cachedTransform.position.x - GameManager.Instance.LimitDistance * GameManager.Instance.Direction) - target.cachedTransform.position.x));
                if(Mathf.Abs(Mathf.Abs(dist) - dest) <= step)
                {
                    pos.x = target.cachedTransform.position.x + (dest * Mathf.Sign(dist));
                    Direction = GameManager.Instance.Direction;
                    Attack();
                    yield return null;
                    break;
                }
                else
                {
                    Direction = GameManager.Instance.Direction * System.Math.Sign(Mathf.Abs(dist) - dest);
                }
            }
            else
            {
                Direction = GameManager.Instance.Direction;
            }
            
            pos += new Vector3(step * Direction, 0.0f);
            
            cachedTransform.position = pos;
            
            yield return null;
        }
        
        NextState();
    }

    public override string GetRunAnimation()
    {
        return "run_02";
    }
}