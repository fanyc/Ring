using UnityEngine;
using System.Collections;
using System.Numerics;

public abstract class CharacterEnemy : Character
{
    public enum TYPE
    {
        NULL = -1,
        Normal = 0,
        LevelBoss,
        StageBoss,
    }

    public enum DAMAGE_TYPE
    {
        NULL = -1,
        WARRIOR,
        ELF,
        SORCERESS,
        ETC,
    }

    public virtual bool IsBoss
    {
        get
        {
            return Type == TYPE.LevelBoss || Type == TYPE.StageBoss;
        }
    }

    
    public virtual float HPFactor
    {
        get { return 1.0f; }
    }

    public BigDecimal MaxHP;
    public float Offset;
    public abstract TYPE Type
    {
        get;
    }
    protected Castable m_castAttack;
    protected BigDecimal m_fHP;
    public BigDecimal HP
    {
        get
        {
            return m_fHP;
        }
    }
    
    public override void Init()
    {
        base.Init();
        MaxHP = m_fHP = (UpgradeManager.Instance.GetUpgrade("EnemyHP").currentValue + new BigDecimal(8.0f)) * HPFactor;
        m_cachedAnimation.skeleton.a = 1.0f;

        SetCamera();
    }
    
    protected override void IdleThought()
    {
        if(GameManager.Instance.InGameState == GameManager.StateInGame.BATTLE)
        {
            Attack();
        }
    }
    
    protected virtual void Attack()
    {
        if(m_castAttack != null)
            Cast(m_castAttack);
    }
    
    public void Beaten(BigDecimal damage, DAMAGE_TYPE type, bool isSmash = false)
    {
        Color startColor = Color.white;
        Color endColor = Color.white;
        Color outlineColor = Color.black;
        Vector3 offset = Vector3.zero;

        switch (type)
        {
            case DAMAGE_TYPE.WARRIOR:
            startColor = new Color32(254, 216, 69, 255);
            endColor = new Color32(223, 121, 30, 255);
            outlineColor = new Color32(102, 35, 21, 192);
            offset = new Vector3(0.0f, 0.0f, 0.0f);
            break;

            case DAMAGE_TYPE.ELF:
            startColor = new Color32(210, 234, 74, 255);
            endColor = new Color32(116, 153, 39, 255);
            outlineColor = new Color32(59, 66, 15, 192);
            offset = new Vector3(-0.3f, 0.3f, 1.0f);
            break;

            case DAMAGE_TYPE.SORCERESS:
            startColor = new Color32(255, 235, 166, 255);
            endColor = new Color32(237, 65, 75, 255);
            outlineColor = new Color32(185, 15, 80, 192);
            offset = new Vector3(-0.6f, 0.6f, 2.0f);
            break;

            case DAMAGE_TYPE.ETC:
            startColor = new Color32(255, 81, 81, 255);
            endColor = new Color32(226, 14, 14, 255);
            outlineColor = new Color32(77, 1, 1, 192);
            offset = new Vector3(-0.9f, 0.9f, 3.0f);
            break;
        }

        ObjectPool<DamageText>.Spawn("@DamageText", new Vector3(cachedTransform.position.x + 0.8f, 2.0f)).Init(damage.ToUnit(), offset, startColor, endColor, outlineColor);
        Beaten(damage, isSmash);        
    }

    public override void Beaten(BigDecimal damage, bool isSmash = false)
    {
        if(State == STATE.DEAD || State == STATE.NULL) return;
        base.Beaten(damage, isSmash);
        m_fHP -= damage;
        if(Type != TYPE.StageBoss || isSmash == true)
            PlayBeatenAnimation();
        //m_cachedAnimation.state.AddAnimation(1, "stand_01", true, 0.233f);
        if(m_fHP <= 0.0f)
        {
            m_fHP = 0.0f;
            HPGauge.UpdateRatio();
            Dead();
        }
        else
        {
            HPGauge.UpdateRatio();
        }
    }

    protected IEnumerator DEAD()
    {
        PlayIdleAnimation();

        float t = 0.0f;
        
        while(t < 1.0f)
        {
            yield return null;
            t += Time.smoothDeltaTime * 2.0f;
            m_cachedAnimation.skeleton.a = 1.0f - t;
        }
        m_cachedAnimation.skeleton.a = 0.0f;
        Recycle();
    }
    
    public virtual void Dead()
    {
        State = STATE.DEAD;

        //UILevelInfo.Instance.SetBoss(false);
        //UILevelInfo.Instance.SetLeaveBoss(false);

        GameManager.Instance.NextWave();
        
        if(IsBoss == true)
        {
            GameManager.Instance.StopCoroutine("_timer");
        }
    }

    public virtual void SetCamera()
    {
        CameraController.Instance.TargetScale = 1.0f;
        CameraController.Instance.Offset = new Vector2(-0.2f, 1.65f);
    }
}