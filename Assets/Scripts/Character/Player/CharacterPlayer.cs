using UnityEngine;
using System.Collections;
using System.Numerics;

public class CharacterPlayer : Character
{
    public float Offset;
    protected Castable m_castAttack;
    protected Castable m_castSkill;
    
    public float MP
    {
        get; set;
    }
    void OnEnable()
    {
        Init();
    }
    
    protected override void IdleThought()
    {
        switch (GameManager.Instance.InGameState)
        {
            case GameManager.StateInGame.MOVE:
            {
                State = STATE.MOVE;
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
        PlayAnimation("run_01", false, true, GameManager.Instance.PlayerSpeed);
        while(m_currentState == STATE.MOVE)
        {
            SetAnimationTimeScale(GameManager.Instance.PlayerSpeed);
            Vector3 pos = cachedTransform.position + new Vector3(11.25f * Time.smoothDeltaTime * 0.5f * GameManager.Instance.PlayerSpeed, 0.0f);
            
            if(GameManager.Instance.cachedTransform.position.x + Offset < pos.x)
            {
                cachedTransform.position = new Vector3(GameManager.Instance.cachedTransform.position.x + Offset, 0.0f);
                break;
            }
            
            cachedTransform.position = pos;
            
            yield return null;
        }
        
        GameManager.Instance.MoveCount--;
        State = STATE.IDLE;
        NextState();
    }
    
    public override void Beaten(BigDecimal damage)
    {
        //base.Beaten(damage);
        //MP = 0.0f;
    }
}