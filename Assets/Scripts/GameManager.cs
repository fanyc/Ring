using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class GameManager : MonoSingleton<GameManager>
{
    //====================Properties Start====================
    protected int m_nLevel;
    protected int m_nCurrnetWave;
    protected const int WAVE_COUNT = 5;
    public enum StateInGame
    {
        IDLE,
        MOVE,
        BATTLE,
    }
    
    protected StateInGame m_InGameState;
    public StateInGame InGameState
    {
        get { return m_InGameState; }
    }
    
    protected CharacterEnemy m_currentEnemy;
    public CharacterEnemy CurrentEnemy
    {
        get { return m_currentEnemy; }
    }
    
    protected int m_nMoveCount;
    public int MoveCount
    {
        get { return m_nMoveCount; }
        set
        {
            m_nMoveCount = value;
            if(m_nMoveCount <= 0)
            {
                m_InGameState = StateInGame.BATTLE;
            }
            
        }
    }
    
    public List<CharacterPlayer> PlayerList;
    
    public float PlayerSpeed
    {
        get
        {
            return 1.0f + (UIRingButton.Instance.IsCharging ? 0.5f : 0.0f);
        }
    }
    
    protected BigDecimal m_fGold = new BigDecimal(0);
    public BigDecimal Gold
    {
        get
        {
            return m_fGold;
        }
        set
        {
            m_fGold = value;
            UIWallet.Instance.textGold.text = m_fGold.ToUnit();
        }
    }
    
    //====================Properties End====================
    //====================Functions Start====================
    void Awake()
    {
        CharacterEnemy[] list = Resources.LoadAll<CharacterEnemy>("Enemy/");
        for(int i = 0; i < list.Length; ++i)
        {
            ObjectPool<CharacterEnemy>.CreatePool(list[i].name, list[i].gameObject, 2);
        }
        
        ObjectPool<ItemGold>.CreatePool("@ItemGold", Resources.Load<GameObject>("Item/@ItemGold"), 20);
    }
    
    void Start()
    {
        Init();
    }
    
    public void Init()
    {
        m_fGold = new BigDecimal(0);
        m_InGameState = StateInGame.IDLE;
        m_nCurrnetWave = 0;
        
        NextWave();
    }
    
    public CharacterEnemy SpawnEnemy()
    {
        m_currentEnemy = ObjectPool<CharacterEnemy>.Spawn("@Enemy001");
        m_currentEnemy.cachedTransform.position = cachedTransform.position + new Vector3(m_currentEnemy.Offset, 0.0f);
        m_currentEnemy.Init();
        
        return m_currentEnemy;
    }
    
    public void NextWave()
    {
        m_nCurrnetWave++;
        cachedTransform.position = new Vector3((m_nCurrnetWave - 1) / 8 * 16.875f, 0.0f);
        if((m_nCurrnetWave - 1) % 8 == 0 && m_nCurrnetWave != 1)
        {
            UpgradeManager.Instance.GetUpgrade("EnemyHP").Level++;
            UpgradeManager.Instance.GetUpgrade("Reward").Level++;
        }
        // if(m_nCurrnetWave > WAVE_COUNT)
        // {
        //     m_nCurrnetWave = 0;
        // }
        // else
        m_InGameState = StateInGame.IDLE;
        
        
        StartCoroutine("_waitIdle");
        // for(int i = 0; i < PlayerList.Count; ++i)
        // {
        //     PlayerList[i].State = Character.STATE.IDLE;
        // }
        
    }
    
    public List<CharacterPlayer> GetPlayers()
    {
        return PlayerList;
    }
    
    IEnumerator _waitIdle()
    {
        bool all = false;
        while(all == false)
        {
            all = true;
            for(int i = 0; i < PlayerList.Count; ++i)
            {
                if(PlayerList[i].State != Character.STATE.IDLE)
                {
                    all = false;
                    break;
                }  
            }
            
            if(all == true) break;
            yield return null;
        }
        {
            SpawnEnemy();
        }
        m_InGameState = StateInGame.MOVE;
        m_nMoveCount = PlayerList.Count;
    }
}