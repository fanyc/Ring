using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.UI;

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
    
    protected List<CharacterEnemy> m_currentEnemies = new List<CharacterEnemy>();
    public List<CharacterEnemy> CurrentEnemies
    {
        get { return m_currentEnemies; }
    }
    
    protected bool m_bBossClear = true;

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
    
    public float Timer
    {
        get; set;
    }

    public int Direction = 1;

    public float LimitDistance = 10.0f;

    public List<CharacterPlayer> PlayerList;

    public GameObject NormalInfo;
    public GameObject BossInfo;
    public GameObject BossButton;
    

    public TMPro.TextMeshProUGUI Level;
    public TMPro.TextMeshProUGUI Wave;

    public TMPro.TextMeshProUGUI TimerText;
    public Image TimerGauge;
    
    
    
    //====================Properties End====================
    //====================Functions Start====================
    void Awake()
    {
        Application.targetFrameRate = -1;
        CharacterEnemy[] list = Resources.LoadAll<CharacterEnemy>("Enemy/");
        for(int i = 0; i < list.Length; ++i)
        {
            ObjectPool<CharacterEnemy>.CreatePool(list[i].name, list[i].gameObject, 2);
        }
        
        ObjectPool<ItemGold>.CreatePool("@ItemGold", Resources.Load<GameObject>("Item/@ItemGold"), 20);
        ObjectPool<DamageText>.CreatePool("@DamageText", Resources.Load<GameObject>("@DamageText"), 20);
        ObjectPool<UIItemSkill>.CreatePool("@ItemSkill", Resources.Load<GameObject>("@ItemSkill"), 16);

        Effect[] effects = Resources.LoadAll<Effect>("Effects/");
        for(int i = 0; i < effects.Length; ++i)
        {
            ObjectPool<Effect>.CreatePool(effects[i].name, effects[i].gameObject, 2);
        }

        Projectile[] projectiles = Resources.LoadAll<Projectile>("Projectiles/");
        for(int i = 0; i < projectiles.Length; ++i)
        {
            ObjectPool<Projectile>.CreatePool(projectiles[i].name, projectiles[i].gameObject, 2);
        }
    }
    
    void Start()
    {
        Init();
    }
    
    public void Init()
    {
        for(int i = 0; i < m_currentEnemies.Count; ++i)
        {
            m_currentEnemies[i].Recycle();
        }
        m_currentEnemies.Clear();

        StopAllCoroutines();
        cachedTransform.position = new Vector3(0.0f, 0.0f);

        for(int i = 0; i < PlayerList.Count; ++i)
        {
            PlayerList[i].Init();
        }

        UIAbilitySlot.Instance.Init();
        
        UIWallet.Instance.Init();
        m_InGameState = StateInGame.IDLE;
        m_nLevel = 1;
        m_nCurrentWave = 0;
        m_nMoveCount = 0;

        BossInfo.SetActive(false);
        NormalInfo.SetActive(true);

        Level.text = "Level 1";
        Wave.text = "1/" + WAVE_COUNT;

        m_bBossClear = true;
        
        NextWave();
    }
    
    public CharacterEnemy SpawnEnemy()
    {
        CharacterEnemy enemy = ObjectPool<CharacterEnemy>.Spawn("@Enemy00" + Random.Range(1, 2));
        enemy.cachedTransform.position = cachedTransform.position + new Vector3(m_currentEnemies.Count + enemy.Offset, 0.0f);
        enemy.Init();

        m_currentEnemies.Add(enemy);
        
        return enemy;
    }

    public CharacterEnemy SpawnBoss()
    {
        CharacterEnemy enemy = ObjectPool<CharacterEnemy>.Spawn("@LevelBoss00" + Random.Range(1, 2));
        enemy.cachedTransform.position = cachedTransform.position + new Vector3(m_currentEnemies.Count + enemy.Offset, 0.0f);
        enemy.Init();

        m_currentEnemies.Add(enemy);

        m_bBossClear = false;
        
        return enemy;
    }
    
    public void NextWave()
    {
        m_nCurrentWave++;

        cachedTransform.position += new Vector3(5.625f, 0.0f);
        
        
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

    public void Rebirth()
    {
        
        StopCoroutine("_waitIdle");
        for(int i = 0; i < PlayerList.Count; ++i)
        {
            PlayerList[i].CastCancel();
        }
        Init();
    }
    
    IEnumerator _waitIdle()
    {
        // bool all = false;
        // while(all == false)
        // {
        //     yield return null;
        //     all = true;
        //     for(int i = 0; i < PlayerList.Count; ++i)
        //     {
        //         if(PlayerList[i].State != Character.STATE.IDLE)
        //         {
        //             all = false;
        //             break;
        //         }  
        //     }
        //     if(all == true) break;
        // }

        m_InGameState = StateInGame.MOVE;
        m_nMoveCount = PlayerList.Count;

        for(int i = 0; i < 3; ++i)
        {
            SpawnEnemy();
        }

        //Level.text = "Level " + m_nLevel;
        Wave.text = ((m_nCurrentWave - 1) % WAVE_COUNT + 1) + "/" + WAVE_COUNT;

        while(m_InGameState == StateInGame.MOVE) yield return null;

        HPGauge.UpdateRatio();
    }

    // IEnumerator _timer(float timeLimit)
    // {
    //     while(m_InGameState == StateInGame.MOVE) yield return null;
        
    //     float time = timeLimit;
    //     TimerText.text = $"{timeLimit:0.0}<size=-4> s</size>";
    //     TimerGauge.fillAmount = 1.0f;

    //     while(time > 0.0f)
    //     {
    //         yield return null;
    //         time -= Time.deltaTime;

    //         TimerText.text = $"{time:0.0}<size=-4> s</size>";
    //         TimerGauge.fillAmount = time / timeLimit;
    //     }

    //     TimerText.text = $"0.0<size=-4> s</size>";
    //     TimerGauge.fillAmount = 0.0f;

    //     LeaveBoss();
    // }
}