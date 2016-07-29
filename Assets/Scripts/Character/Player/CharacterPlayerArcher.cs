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
        m_skillDataList.AddSkillData("매그넘 샷", "ArcherSkill", "skill_01", "SkillIcon/btle_icskill_elf_01b");
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
        m_castSkill = new CastPlayerArcherSkill(this);
        base.Init();
    }

    protected override void IdleThought()
    {
        float minDistance =
            Mathf.Min(m_castAttack.MinDistance,
                      Mathf.Min(GameManager.Instance.LimitDistance,
                                Mathf.Abs(cachedTransform.position.x - GameManager.Instance.cachedTransform.position.x))) - 1.0f;
        Character[] targets = m_castAttack.GetTargets();
        if(targets?.Length > 0 &&
           (minDistance <= Mathf.Abs(Castable.GetNearestTarget(this, targets).cachedTransform.position.x - cachedTransform.position.x))
        {
            Attack();
        }
        else
        {
            State = STATE.MOVE;
        }
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
                if(Mathf.Abs(Mathf.Abs(dist) - m_castAttack.MinDistance) <= step)
                {
                    pos.x = target.cachedTransform.position.x + (m_castAttack.MinDistance * Mathf.Sign(dist));
                    Direction = GameManager.Instance.Direction;
                    Attack();
                    yield return null;
                    break;
                }
                else
                {
                    Direction = GameManager.Instance.Direction * System.Math.Sign(Mathf.Abs(dist) - m_castAttack.MinDistance);
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