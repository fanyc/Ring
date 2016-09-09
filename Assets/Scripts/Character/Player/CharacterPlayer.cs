using UnityEngine;
using System.Collections;
using System.Numerics;
using System.Collections.Generic;

public abstract class CharacterPlayer : Character
{
    public static List<CharacterPlayer> PlayerList = new List<CharacterPlayer>();
    
    public struct SkillData
    {
        public string name;
        public string upgradeGroup;
        public string animationName;
        public string thumbnailName;

        public string castableName;
        

        public SkillData(string name, string upgradeGroup, string animationName, string thumbnailName, string castableName)
        {
            this.name = name;
            this.upgradeGroup = upgradeGroup;
            this.animationName = animationName;
            this.thumbnailName = thumbnailName;
            this.castableName = castableName;
        }
    }

    public class SkillDataList
    {
        protected List<SkillData> m_listSkillData = new List<SkillData>();
        
        public SkillData this[int index]
        {
            get
            {
                return m_listSkillData[index];
            }
        }

        public int Count
        {
            get
            {
                return m_listSkillData.Count;
            }
        }

        public void AddSkillData(string name, string upgradeGroup, string animationName, string thumbnailName, string castableName)
        {
            m_listSkillData.Add(new SkillData(name, upgradeGroup, animationName, thumbnailName, castableName));
        }
    }
    public abstract SkillDataList ListSkillData
    {
        get;
    }

    private int m_nCachedLayer = 0;
    public override int Layer
    {
        get
        {
            if(m_nCachedLayer == 0)
            {
                m_nCachedLayer = 1 << LayerMask.NameToLayer("Ally");
            }
            return m_nCachedLayer;
        }
    }

    public UISkillButton SkillButton;

    public static float AttackPerSecond
    {
        get
        {
            return 0.0f;
        }
    }
    public float Offset;
    protected Castable m_castAttack;
    protected Castable m_castSkill;
    
    public float MP
    {
        get; set;
    }

    public override void Init()
    {
        PlayerList.Add(this);
        position = new Vector3(Offset, 0.0f);
        base.Init();
    }

    void Update()
    {
        MP += 1.0f * (UIRingButton.Instance.IsCharging ? 0.0f : 1.0f) * Time.deltaTime;

        // if(GameManager.Instance.AutoSkill && m_castSkill.Cost + 0.5f < MP)
        // {
        //     SkillButton?.CastSkill();
        // }
    }
    
    protected override void IdleThought()
    {
        switch (GameManager.Instance.InGameState)
        {
            case GameManager.StateInGame.MOVE:
            {
                if(GameManager.Instance.cachedTransform.position.x + Offset > position.x)
                {
                    State = STATE.MOVE;
                }
            }
            break;
            case GameManager.StateInGame.BATTLE:
            {
                Attack();
            }
            break;
        }
    }
    
    protected virtual void Attack()
    {
        if(m_castAttack != null)
            Cast(m_castAttack);
    }

    public Castable GetSkill()
    {
        return m_castSkill;
    }

    protected virtual IEnumerator MOVE()
    {
        PlayAnimation(GetRunAnimation(), false, true);
        while(State == STATE.MOVE)
        {
            Vector3 pos = position + new Vector3(11.25f * Time.smoothDeltaTime * 0.5f, 0.0f);
            
            if(GameManager.Instance.cachedTransform.position.x + Offset < pos.x)
            {
                position = new Vector3(GameManager.Instance.cachedTransform.position.x + Offset, 0.0f);
                break;
            }
            
            position = pos;
            
            yield return null;
        }
        
        GameManager.Instance.MoveCount--;
        State = STATE.IDLE;
        NextState();
    }

    public override void Beaten(BigDecimal damage, DAMAGE_TYPE type, bool isSmash = false)
    {
        //base.Beaten(damage);
        //MP = 0.0f;
    }

    public override void Dead()
    {
        PlayerList.Remove(this);
        base.Dead();
    }
}