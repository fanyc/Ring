using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    //====================Properties Start====================
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
    
    public float PlayerSpeed
    {
        get
        {
            return 1.0f + (UIRingButton.Instance.IsCharging ? 1.0f : 0.0f);
        }
    }
    
    public int Direction = 1;

    public float LimitDistance = 500.0f;

    public List<CharacterPlayer> PlayerList;

    public StageData CurrentStage
    {
        protected set;
        get;
    }

    public int CurrentWave
    {
        protected set;
        get;
    }
    
    //====================Properties End====================
    //====================Functions Start====================
    void Awake()
    {
        Application.targetFrameRate = -1;
        Screen.SetResolution(1280, 720, true);
        CharacterEnemy[] list = Resources.LoadAll<CharacterEnemy>("Enemy/");
        for(int i = 0; i < list.Length; ++i)
        {
            ObjectPool<CharacterEnemy>.CreatePool(list[i].name, list[i].gameObject, 2);
        }
        
        ObjectPool<ItemGold>.CreatePool("@ItemGold", Resources.Load<GameObject>("Item/@ItemGold"), 20);
        ObjectPool<DamageText>.CreatePool("@DamageText", Resources.Load<GameObject>("@DamageText"), 20);
        ObjectPool<UIItemSkill>.CreatePool("@ItemSkill", Resources.Load<GameObject>("@ItemSkill"), 16);
        ObjectPool<UIHPGauge>.CreatePool("@HPGauge", Resources.Load<GameObject>("@HPGauge"), 10);

        ObjectPool<UIAbilityIcon>.CreatePool("AbilityIcon_Mana", Resources.Load<GameObject>("Abilities/@AbilityIcon_Mana"), 10);
        UIAbilitySlot.Instance.Add("AbilityIcon_Mana");

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

        
        CurrentStage = new StageData(Resources.Load<TextAsset>("TempStage").text);
        
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

        CurrentWave = -1;

        NextWave();
    }
    
    public CharacterEnemy SpawnEnemy(string name, float offset)
    {
        CharacterEnemy enemy = ObjectPool<CharacterEnemy>.Spawn(name);
        enemy.cachedTransform.position = cachedTransform.position + new Vector3(7.5f + offset + enemy.Offset, 0.0f);
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
        
        return enemy;
    }

    public void RemoveEnemy(CharacterEnemy enemy)
    {
        m_currentEnemies.Remove(enemy);

        if(m_currentEnemies.Count <= 0)
        {
            NextWave();
        }
    }
    
    public void NextWave()
    {
        cachedTransform.position = new Vector3(PlayerList[0].cachedTransform.position.x, 0.0f);
        
        ++CurrentWave;
        CurrentStage.Spawn(0);
    }
    
    public List<CharacterPlayer> GetPlayers()
    {
        return PlayerList;
    }

    public void Rebirth()
    {
        
        for(int i = 0; i < PlayerList.Count; ++i)
        {
            PlayerList[i].CastCancel();
        }
        Init();
    }
}