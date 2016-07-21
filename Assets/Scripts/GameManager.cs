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
    
    protected CharacterEnemy m_currentEnemy;
    public CharacterEnemy CurrentEnemy
    {
        get { return m_currentEnemy; }
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
        m_currentEnemy?.Recycle();
        m_currentEnemy = null;

        StopAllCoroutines();
        cachedTransform.position = new Vector3(0.0f, 0.0f);

        for(int i = 0; i < PlayerList.Count; ++i)
        {
            PlayerList[i].Init();
        }
        
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
        m_currentEnemy = ObjectPool<CharacterEnemy>.Spawn("@Enemy00" + Random.Range(1, 4));
        m_currentEnemy.cachedTransform.position = cachedTransform.position + new Vector3(m_currentEnemy.Offset, 0.0f);
        m_currentEnemy.Init();
        
        return m_currentEnemy;
    }

    public CharacterEnemy SpawnBoss()
    {
        m_currentEnemy = ObjectPool<CharacterEnemy>.Spawn("@LevelBoss00" + Random.Range(1, 2));
        m_currentEnemy.cachedTransform.position = cachedTransform.position + new Vector3(m_currentEnemy.Offset, 0.0f);
        m_currentEnemy.Init();

        m_bBossClear = false;
        
        return m_currentEnemy;
    }

    public void LeaveBoss()
    {
        m_currentEnemy.Recycle();
        SpawnEnemy();

        HPGauge.UpdateRatio();

        BossInfo.SetActive(false);
        NormalInfo.SetActive(true);
        BossButton.SetActive(true);

        StopCoroutine("_timer");
    }

    public void TryBoss()
    {
        if(m_nCurrentWave % WAVE_COUNT == 0)
        {
            m_currentEnemy.Recycle();

            BossInfo.SetActive(true);
            NormalInfo.SetActive(false);
            
            float timer = 30.0f;
            if(m_nCurrentWave % (WAVE_COUNT * 12) == 0)
            {
                timer = 60.0f;
            }

            SpawnBoss();
            StopCoroutine("_timer");
            StartCoroutine("_timer", timer);

            HPGauge.UpdateRatio();
        }

        BossButton.SetActive(false);
    }
    
    public void NextWave()
    {
        if( m_currentEnemy?.IsBoss == true && 
            m_currentEnemy?.State == Character.STATE.DEAD)
        {
            m_bBossClear = true;
        }

        if(m_bBossClear)
        {
            m_nCurrentWave++;
            if(m_nCurrentWave > 1 && m_nCurrentWave % WAVE_COUNT == 1)
            {
                UpgradeManager.Instance.GetUpgrade("EnemyHP").Level++;
                UpgradeManager.Instance.GetUpgrade("Reward").Level++;
                ++m_nLevel;
            }
        }

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
        bool all = false;
        while(all == false)
        {
            yield return null;
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
        }
        m_InGameState = StateInGame.MOVE;
        m_nMoveCount = PlayerList.Count;

        if(m_nCurrentWave > 0)
        {
            if(m_bBossClear == true && m_nCurrentWave % WAVE_COUNT == 0)
            {
                float timer = 30.0f;
                if(m_nCurrentWave % (WAVE_COUNT * 12) == 0)
                {
                    timer = 60.0f;
                }

                SpawnBoss();
                StopCoroutine("_timer");
                StartCoroutine("_timer", timer);
            }
            else
            {
                SpawnEnemy();
            }
        }

        Level.text = "Level " + m_nLevel;
        Wave.text = ((m_nCurrentWave - 1) % WAVE_COUNT + 1) + "/" + WAVE_COUNT;

        while(m_InGameState == StateInGame.MOVE) yield return null;
        if(m_currentEnemy.IsBoss)
        {
            BossInfo.SetActive(true);
            NormalInfo.SetActive(false);
        }

        HPGauge.UpdateRatio();
    }

    IEnumerator _timer(float timeLimit)
    {
        while(m_InGameState == StateInGame.MOVE) yield return null;
        
        float time = timeLimit;
        TimerText.text = $"{timeLimit:0.0}<size=-4> s</size>";
        TimerGauge.fillAmount = 1.0f;

        while(time > 0.0f)
        {
            yield return null;
            time -= Time.deltaTime;

            TimerText.text = $"{time:0.0}<size=-4> s</size>";
            TimerGauge.fillAmount = time / timeLimit;
        }

        TimerText.text = $"0.0<size=-4> s</size>";
        TimerGauge.fillAmount = 0.0f;

        LeaveBoss();
    }
}