using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPlayerFairy : CharacterPlayer
{
    protected static SkillDataList m_skillDataList = new SkillDataList();

    public override SkillDataList ListSkillData
    {
        get {return m_skillDataList;}
    }

    static CharacterPlayerFairy()
    {
        //m_skillDataList.AddSkillData("파 크라이", "WarriorSkill", "skill_a01", "SkillIcon/btle_icskill_wri_01b", "CastPlayerWarriorSkill");
        m_skillDataList.AddSkillData("회복", "", "", "Icons/icskill_far_01", "CastPlayerHeal");
    }


    public new static float AttackPerSecond
    {
        get
        {
            return 0.0f;
        }
    }

    protected Character m_following;
    protected Vector3 m_vecOffset;

    protected Bezier m_bezier;

    protected float m_fIdleTime = 0.0f;
    public override void Init()
    {
        m_castAttack = new CastPlayerHeal(this);
        for(int i = 0; i < m_skillDataList.Count; ++i)
        {
            UIAbilityIconSkill orig = Resources.Load<UIAbilityIconSkill>("Abilities/@AbilityIconSkill_Supporter");
            ObjectPool<UIAbilityIcon>.CreatePool("Ability" + m_skillDataList[i].castableName, orig.cachedGameObject, 10, (UIAbilityIcon icon)=>
            {
                ((UIAbilityIconSkill)icon).Init(this, m_skillDataList[i]);
            });
            UIAbilitySlot.Instance.Add("Ability" + m_skillDataList[i].castableName);
        }

        m_fHP = MaxHP = 10.0f;

        base.Init();

        m_HPGauge.Hide();

        m_following = CharacterPlayer.PlayerList[0];
        m_vecOffset = new Vector3(-0.75f * GameManager.Instance.Direction, 1.0f);

        Vector3[] r = new Vector3[3];
        
        m_bezier = new Bezier(cachedTransform.position,
                            CameraController.Instance.cachedTransform.position + new Vector3(Random.Range(-1.0f, -5.0f), Random.Range(1.5f, 3.5f)), 
                            CameraController.Instance.cachedTransform.position + new Vector3(Random.Range(-1.0f, -5.0f), Random.Range(1.5f, 3.5f)), 
                            CameraController.Instance.cachedTransform.position + new Vector3(Random.Range(-1.0f, -5.0f), Random.Range(1.5f, 3.5f)));
    }

    protected override void IdleThought()
    {
        // if(m_castAttack?.GetTargets()?.Length > 0)
        // {
        //     Attack();
        // }
        // else if(m_fKnockBack == 0.0f)
        if(m_fIdleTime > 0.0f)
        {
            m_fIdleTime -= Time.deltaTime;
        }
        else
        {
            State = STATE.MOVE;
        }
    }

    protected override IEnumerator MOVE()
    {
        //PlayAnimation(GetRunAnimation(), false, true);
        float time = 0.0f;
        
        while(State == STATE.MOVE)
        {
            
            // if(m_castAttack.GetTargets().Length > 0)
            // {
            //     Attack();
            //     yield return null;
            //     break;
            // }
            
            Vector3 pos = position;
            if(m_bezier != null)
            {
                
                pos = (Vector3)(Vector2)m_bezier.GetPointAtTime(time);
                time += Time.smoothDeltaTime; 
                if(time >= 1.0f)
                {
                    time = 0.0f;
                    
                    if(Random.Range(0, 3) == 0)
                    {
                        m_bezier = new Bezier(cachedTransform.position,
                                            pos + (m_bezier.p3 - m_bezier.p2), 
                                            CameraController.Instance.cachedTransform.position + new Vector3(Random.Range(-2.0f, -5.0f), Random.Range(-0.5f, 1.5f)), 
                                            CameraController.Instance.cachedTransform.position + new Vector3(Random.Range(-2.0f, -5.0f), Random.Range(-0.5f, 1.5f)));
                    }
                    else
                    {
                        float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
                        Vector3[] p = new Vector3[2];
                        p[0] = cachedTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                        angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
                        p[1] = cachedTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                        m_bezier = new Bezier(cachedTransform.position,
                                            pos + (m_bezier.p3 - m_bezier.p2), 
                                            p[0], 
                                            p[1]);
                    }
                    
                }
                //Vector3.Lerp(position, CharacterPlayer.PlayerList[0].position + m_vecOffset, 3.0f * Time.smoothDeltaTime);
            }
            position = pos;
            
            
            yield return null;
        }
        
        NextState();
    }
}