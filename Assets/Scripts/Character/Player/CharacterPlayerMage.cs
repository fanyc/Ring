using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPlayerMage : CharacterPlayer
{
    protected static SkillDataList m_skillDataList = new SkillDataList();

    public override SkillDataList ListSkillData
    {
        get {return m_skillDataList;}
    }

    static CharacterPlayerMage()
    {
        m_skillDataList.AddSkillData("미티어 스트라이크", "SoceressSkill", "skill_01", "Icons/icskill_sor_02", "CastPlayerMageSkill_Ball");
        m_skillDataList.AddSkillData("미티어 스트라이크", "SoceressSkill", "skill_01", "Icons/icskill_sor_01", "CastPlayerMageSkill");
    }


    public new static float AttackPerSecond
    {
        get
        {
            return 1.867f;
        }
    }

    public override void Init()
    {
        m_castAttack = new CastPlayerMageAttack(this);
        m_castSkill = Castable.CreateCast(m_skillDataList[0].castableName, this);

        for(int i = 0; i < m_skillDataList.Count; ++i)
        {
            UIAbilityIconSkill orig = Resources.Load<UIAbilityIconSkill>("Abilities/@AbilityIconSkill_Mage");
            ObjectPool<UIAbilityIcon>.CreatePool("Ability" + m_skillDataList[i].castableName, orig.cachedGameObject, 10, (UIAbilityIcon icon)=>
            {
                ((UIAbilityIconSkill)icon).Init(this, m_skillDataList[i]);
            });
            UIAbilitySlot.Instance.Add("Ability" + m_skillDataList[i].castableName);
        }

        m_fHP = MaxHP = 100.0f;

        base.Init();
    }
    protected override void IdleThought()
    {
        Character[] targets = m_castAttack.GetTargets();
        
        if(targets?.Length > 0)
        {
            Character target = Castable.GetNearestTarget(this, targets);
            float minDistance =
            Mathf.Min(m_castAttack.Distance - 1.0f,
                      Mathf.Abs((GameManager.Instance.cachedTransform.position.x - GameManager.Instance.LimitDistance * GameManager.Instance.Direction) - target.position.x));
                                
            if(minDistance <= Mathf.Abs(target.position.x - position.x))
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
            Vector3 pos = position;
            float step = 11.25f * Time.smoothDeltaTime * 0.4f;
            Character[] targets = m_castAttack.GetTargets();
            if(targets?.Length > 0)
            {
                Character target = Castable.GetNearestTarget(this, targets);
                float dist = target.position.x - pos.x;
                float dest = Mathf.Min(m_castAttack.Distance,
                Mathf.Abs((GameManager.Instance.cachedTransform.position.x - GameManager.Instance.LimitDistance * GameManager.Instance.Direction) - target.position.x));
                if(Mathf.Abs(Mathf.Abs(dist) - dest) <= step)
                {
                    pos.x = target.position.x + (dest * Mathf.Sign(dist));
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
            
            position = pos;
            
            yield return null;
        }
        
        NextState();
    }

    public override string GetRunAnimation()
    {
        return "run_02";
    }
}