using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class GameManager : MonoSingleton<GameManager>
{
    //====================Properties Start====================
    protected int m_nLevel;
    protected int m_nCurrentWave;
    protected const int WAVE_COUNT = 8;
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
    
    public float PlayerSpeed
    {
        get
        {
            return 1.0f + (UIRingButton.Instance.IsCharging ? 1.0f : 0.0f);
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

    public List<CharacterPlayer> PlayerList;

    public GameObject NormalInfo;
    public GameObject BossInfo;

    public TMPro.TextMeshProUGUI Level;
    public TMPro.TextMeshProUGUI Wave;
    
    
    
    
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
        ObjectPool<DamageText>.CreatePool("@DamageText", Resources.Load<GameObject>("@DamageText"), 100);
        
    }
    
    void Start()
    {
        Init();
    }
    
    public void Init()
    {
        m_fGold = new BigDecimal(0);
        m_InGameState = StateInGame.IDLE;
        m_nLevel = 1;
        m_nCurrentWave = 0;

        BossInfo.SetActive(false);
        NormalInfo.SetActive(true);

        Level.text = "Level 1";
        Wave.text = "1/" + WAVE_COUNT;
        
        NextWave();
    }
    
    public CharacterEnemy SpawnEnemy()
    {
        m_currentEnemy = ObjectPool<CharacterEnemy>.Spawn("@Enemy00" + Random.Range(1, 4));
        m_currentEnemy.cachedTransform.position = cachedTransform.position + new Vector3(m_currentEnemy.Offset, 0.0f);
        m_currentEnemy.Init();
        
        return m_currentEnemy;
    }
    
    public void NextWave()
    {
        m_nCurrentWave++;
        if(m_nCurrentWave > 1 && m_nCurrentWave % WAVE_COUNT == 1)
        {
            UpgradeManager.Instance.GetUpgrade("EnemyHP").Level++;
            UpgradeManager.Instance.GetUpgrade("Reward").Level++;
            ++m_nLevel;
        }

        cachedTransform.position = new Vector3(m_nCurrentWave * 5.625f, 0.0f);
        if(m_nCurrentWave > 0)
        {
            if(m_nCurrentWave % WAVE_COUNT == 0)
            {
                if(m_nCurrentWave % (WAVE_COUNT * 12) == 0)
                {
                    
                }
                //else
                
            }
        }
        
        // if(m_nCurrentWave > WAVE_COUNT)
        // {
        //     m_nCurrentWave = 0;
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

        

        Level.text = "Level " + m_nLevel;
        Wave.text = ((m_nCurrentWave - 1) % WAVE_COUNT + 1) + "/" + WAVE_COUNT;

        while(m_InGameState == StateInGame.MOVE) yield return null;
        if(m_nCurrentWave > 0 && m_nCurrentWave % WAVE_COUNT == 0)
        {
            BossInfo.SetActive(true);
            NormalInfo.SetActive(false);
        }

        HPGauge.UpdateRatio();
    }
}