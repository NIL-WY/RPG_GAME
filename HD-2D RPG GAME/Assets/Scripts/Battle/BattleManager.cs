using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 战斗管理器，负责管理战斗流程、回合切换、以及响应 UI 操作等。
/// 此脚本为单例模式，仅允许存在一个 BattleManager 实例。
/// </summary>
public class BattleManager : MonoBehaviour
{
    #region 公共字段

    /// <summary>
    /// 当前战斗中的敌人列表（包含所有敌人对象）。
    /// </summary>
    public List<Enemy> enemies;

    /// <summary>
    /// 当前战斗中的我方角色（英雄）列表。
    /// </summary>
    public List<Hero> heroes;

    #endregion

    #region 序列化字段

    [Header("生成设置")]
    [Tooltip("英雄生成点父物体（包含所有英雄生成位置）")]
    [SerializeField] Transform heroSpawnRoot;
    [Tooltip("敌人生成点父物体（包含所有敌人生成位置）")]
    [SerializeField] Transform enemySpawnRoot;
    [Tooltip("角色预制体数据库，用于根据 ID 获取预制体")]
    [SerializeField] CharacterDatabase database;

    [Header("调试模式")]
    [Tooltip("是否在生成过程中输出调试日志")]
    [SerializeField] bool logSpawnProcess;

    [Header("回合控制")]
    [Tooltip("回合结束后延迟进入下一回合的时间（用于播放攻击/技能动画）")]
    [SerializeField] private float _turnDelaySeconds = 0.75f;

    #endregion

    #region 私有变量

    /// <summary>
    /// 内部存储的已生成英雄列表（用于生成流程时临时保存）。
    /// </summary>
    private List<Hero> activeHeroes = new();

    /// <summary>
    /// 内部存储的已生成敌人列表（用于生成流程时临时保存）。
    /// </summary>
    private List<Enemy> activeEnemies = new();

    /// <summary>
    /// 当前正在操作的英雄（处于回合中）。
    /// </summary>
    private Hero _currentHero;

    /// <summary>
    /// 标记是否处于主动回合状态（当前单位是否可以执行操作）。
    /// </summary>
    private bool _isActiveTurn = false;

    /// <summary>
    /// 标记当前战斗是否激活（战斗是否正在进行）。
    /// </summary>
    private bool _isFightActive = false;

    #endregion

    #region 事件定义

    /// <summary>
    /// 主动回合状态改变事件委托（参数：当前主动状态）。
    /// </summary>
    public delegate void ActiveTurnEvent(bool value);

    /// <summary>
    /// 当主动回合状态改变时触发的事件。
    /// </summary>
    public static ActiveTurnEvent OnActiveTurnChanged;

    /// <summary>
    /// 战斗结束事件委托（参数：是否胜利）。
    /// </summary>
    public delegate void BattleEndHandler(bool isVictory);

    /// <summary>
    /// 战斗结束时触发的事件。
    /// </summary>
    public static event BattleEndHandler OnBattleEnd;

    #endregion

    #region 单例模式

    /// <summary>
    /// BattleManager 单例实例（全局唯一）。
    /// </summary>
    public static BattleManager Instance { get; private set; }

    #endregion

    #region 生命周期

    /// <summary>
    /// OnEnable 生命周期：订阅 UI 事件（如选择攻击目标、技能目标）。
    /// </summary>
    private void OnEnable()
    {
        BattleUIHandler.OnSelectEnemyAttack += ChoseAttack;
        BattleUIHandler.OnSelectEnemyAbility += ChoseAbility;
    }
    public void OnVictoryButtonClicked()
    {
        // 在胜利界面点击按钮后调用此方法
        StartCoroutine(HandleVictoryReturn());
        
    }
    private IEnumerator HandleVictoryReturn()
    {

        PartyData party = PartyManager.Instance.currentParty;
        

        // 更新主角状态
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = PreBattleData.playerPosition;
        }
        AsyncOperation async = SceneManager.UnloadSceneAsync("Battle");
        yield return async;
    }

    /// <summary>
    /// Awake 生命周期：设置单例实例，并初始化英雄和敌人列表。
    /// </summary>
    private void Awake()
    {
        // 单例判断，确保只有一个 BattleManager 实例存在
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化列表，防止后续使用空引用
        heroes = new List<Hero>();
        enemies = new List<Enemy>();
    }

    /// <summary>
    /// Start 生命周期：获取战斗配置并初始化战斗。
    /// </summary>
    private void Start()
    {
        // 从 BattleSetupManager 中获取战斗配置（包含各方单位 ID）
        var config = BattleSetupManager.Instance.GetConfig();
        InitializeBattle(config);
    }

    /// <summary>
    /// Update 生命周期：每帧检测战斗是否结束。
    /// </summary>
    private void Update()
    {
        // 若战斗未激活则不处理后续逻辑
        if (!_isFightActive) return;

        // 检查是否满足战斗结束条件（胜利或失败）
        CheckBattleEndCondition();
    }

    #endregion

    #region 战斗初始化

    /// <summary>
    /// 初始化战斗，根据传入的战斗配置生成单位并开始战斗。
    /// </summary>
    /// <param name="config">战斗配置，包含英雄与敌人的 ID 数组</param>
    void InitializeBattle(BattleConfig config)
    {
        // 清除场景中已有的英雄和敌人
        ClearExistingUnits();

        // 获取英雄和敌人的有效生成点（只取处于激活状态的子物体）
        var heroSpawns = GetValidSpawnPoints(heroSpawnRoot);
        var enemySpawns = GetValidSpawnPoints(enemySpawnRoot);

        // 根据配置生成英雄和敌人单位
        SpawnHeroes(config.heroIDs, heroSpawns);
        SpawnEnemies(config.enemyIDs, enemySpawns);
        // 启动战斗开始序列（用于延迟初始化、播放过渡动画等）
        StartCoroutine(BattleStartSequence());
    }
    /// <summary>
    /// 清除战斗场景中已有的英雄与敌人对象，并清空列表。
    /// </summary>
    void ClearExistingUnits()
    {
        foreach (var hero in heroes)
        {
            if (hero != null)
            {
                Debug.Log("销毁英雄: " + hero.name);
                DestroyImmediate(hero.gameObject);
            }
        }
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                Debug.Log("销毁敌人: " + enemy.name);
                DestroyImmediate(enemy.gameObject);
            }
        }
        heroes.Clear();
        enemies.Clear();
    }

    /// <summary>
    /// 战斗开始序列协程，等待一帧确保所有组件初始化完毕，然后初始化单位状态并激活战斗。
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator BattleStartSequence()
    {
        yield return new WaitForEndOfFrame();

        // 初始化各个单位（英雄和敌人）的状态和事件监听
        Initialize();

        // 激活战斗，允许 Update 中检测战斗结束条件
        _isFightActive = true;

        Debug.Log("战斗初始化完成");
    }

    /// <summary>
    /// 获取指定生成点父物体下所有有效的生成点（处于激活状态的子物体）。
    /// </summary>
    /// <param name="root">生成点父物体</param>
    /// <returns>有效生成点列表</returns>
    List<Transform> GetValidSpawnPoints(Transform root)
    {
        var points = new List<Transform>();
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            if (child.gameObject.activeSelf)
                points.Add(child);
        }
        return points;
    }

    /// <summary>
    /// 根据传入的英雄 ID 数组和生成点列表生成英雄单位。
    /// </summary>
    /// <param name="ids">英雄 ID 数组</param>
    /// <param name="spawnPoints">有效生成点列表</param>
    void SpawnHeroes(string[] ids, List<Transform> spawnPoints)
    {
        if (database == null)
        {
            Debug.LogError("角色数据库未分配！");
            return;
        }

        activeHeroes.Clear();
        for (int i = 0; i < Mathf.Min(ids.Length, spawnPoints.Count); i++)
        {
            if (string.IsNullOrEmpty(ids[i]))
            {
                Debug.LogError($"第 {i} 个英雄ID为空！");
                continue;
            }

            var prefab = database.GetPrefab(ids[i], true);
            if (prefab == null)
            {
                Debug.LogError($"英雄预制体 '{ids[i]}' 未找到，请检查数据库配置或ID拼写！");
                continue;
            }

            var spawnPos = spawnPoints[i].position;
            var heroObj = Instantiate(prefab, spawnPos, spawnPoints[i].rotation);
            heroObj.name = prefab.name; // 重置名称，去除"(Clone)"
            SceneManager.MoveGameObjectToScene(heroObj, SceneManager.GetSceneByName("Battle"));

            var hero = heroObj.GetComponent<Hero>();
            if (hero == null)
            {
                Debug.LogError($"预制体 '{prefab.name}' 缺少Hero组件！");
                Destroy(heroObj);
                continue;
            }
            activeHeroes.Add(hero);
            if (logSpawnProcess)
                Debug.Log($"生成英雄: {hero.name} 位置: {spawnPoints[i].name}");
        }
        heroes = activeHeroes;
    }

    /// <summary>
    /// 根据传入的敌人 ID 数组和生成点列表生成敌人单位。
    /// </summary>
    /// <param name="ids">敌人 ID 数组</param>
    /// <param name="spawnPoints">有效生成点列表</param>
    void SpawnEnemies(string[] ids, List<Transform> spawnPoints)
    {
        // 清空之前生成的敌人列表
        activeEnemies.Clear();

        // 按照最小生成数（ID 数量与生成点数中的较小值）生成敌人
        for (int i = 0; i < Mathf.Min(ids.Length, spawnPoints.Count); i++)
        {
            // 从数据库中获取对应敌人预制体（false 表示敌人）
            var prefab = database.GetPrefab(ids[i], false);
            if (prefab == null)
            {
                Debug.LogError($"敌人预制体未找到: {ids[i]}");
                continue;
            }

            // 实例化敌人对象
            var enemyObj = Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
            var enemy = enemyObj.GetComponent<Enemy>();
            SceneManager.MoveGameObjectToScene(enemyObj, SceneManager.GetSceneByName("Battle"));
            if (enemy == null)
            {
                Debug.LogError($"预制体 '{prefab.name}' 缺少Enemy组件！");
                Destroy(enemyObj);
                continue;
            }

            // 将生成的敌人添加到活动列表中
            activeEnemies.Add(enemy);
            if (logSpawnProcess)
                Debug.Log($"生成敌人: {enemy.name} 位置: {spawnPoints[i].name}");
        }

        // 将生成的敌人列表赋值给公共 enemies 列表
        enemies = activeEnemies;
    }

    #endregion

    #region 战斗结束与回合控制

    /// <summary>
    /// 检查战斗是否满足结束条件（胜利或失败）。
    /// 若所有英雄均死亡或英雄列表为空，则判定战斗失败；
    /// 若敌人列表为空，则判定战斗胜利。
    /// </summary>
    private void CheckBattleEndCondition()
    {
        if (heroes.Count == 0 || AllHeroesDead())
        {
            EndBattle(false); // 战斗失败
            return;
        }

        if (enemies.Count == 0)
        {
            EndBattle(true); // 战斗胜利
        }
    }

    /// <summary>
    /// 检查所有英雄是否均已死亡。
    /// </summary>
    /// <returns>若所有英雄死亡则返回 true，否则返回 false</returns>
    private bool AllHeroesDead()
    {
        return heroes.All(hero => hero.IsDead);
    }

    /// <summary>
    /// 结束战斗，停止战斗活动，触发结束事件，并关闭所有单位的回合计时器。
    /// 同时通知 UI 显示战斗结果。
    /// </summary>
    /// <param name="isVictory">战斗是否胜利</param>
    private void EndBattle(bool isVictory)
    {
        // 停止战斗流程
        _isFightActive = false;
        // 触发战斗结束事件，通知订阅者（例如 UI 系统）
        OnBattleEnd?.Invoke(isVictory);

        // 遍历所有英雄，停止其回合计时器
        foreach (var hero in heroes)
        {
            hero.ToggleTurnTimer(true);
        }
        // 遍历所有敌人，停止其回合计时器
        foreach (var enemy in enemies)
        {
            enemy.ToggleTurnTimer(true);
        }

        // 启动延时协程
        StartCoroutine(DelayedBattleResult(isVictory));
    }
    private IEnumerator DelayedBattleResult(bool isVictory)
    {
        // 等待2秒（受Time.timeScale影响）
        yield return new WaitForSeconds(2f);

        // 显示战斗结果
        BattleUIHandler.Instance.ShowBattleResult(isVictory);

        // 停止所有协程（可选）
        StopAllCoroutines();
    }
    private void OnDisable()
    {
       
    }
    /// <summary>
    /// 初始化各单位（英雄和敌人）的事件监听器，用于回合控制和状态更新。
    /// </summary>
    private void Initialize()
    {
        // 为每个英雄订阅死亡、开始回合和结束回合事件
        foreach (Hero hero in heroes)
        {
            hero.OnDeath += HandleHeroDeath;
            hero.OnStartTurn += StartTurn;
            hero.OnEndTurn += EndTurn;
        }

        // 为每个敌人订阅死亡、开始回合和结束回合事件
        foreach (Enemy enemy in enemies)
        {
            enemy.OnDeath += HandleEnemyDeath;
            enemy.OnStartTurn += StartTurn;
            enemy.OnEndTurn += EndTurn;
        }
    }

    /// <summary>
    /// 处理英雄死亡事件，检测战斗是否需要结束。
    /// </summary>
    /// <param name="hero">死亡的英雄对象</param>
    private void HandleHeroDeath(Battler hero)
    {
        CheckBattleEndCondition();
    }

    /// <summary>
    /// 处理敌人死亡事件，检测战斗是否需要结束。
    /// </summary>
    /// <param name="enemy">死亡的敌人对象</param>
    private void HandleEnemyDeath(Battler enemy)
    {
        CheckBattleEndCondition();
    }

    #endregion

    #region 英雄操作

    /// <summary>
    /// 当英雄选择普通攻击时调用，执行攻击操作。
    /// </summary>
    /// <param name="enemy">被攻击的敌人对象</param>
    public void ChoseAttack(Enemy enemy)
    {
        _currentHero.Attack(enemy);
    }

    /// <summary>
    /// 当英雄选择使用技能（不指定目标）时调用。
    /// </summary>
    /// <param name="ability">英雄使用的技能</param>
    public void ChoseAbility(Ability ability)
    {
        _currentHero.UseAbility(ability);
    }

    /// <summary>
    /// 当英雄选择使用技能（指定目标敌人）时调用。
    /// </summary>
    /// <param name="enemy">目标敌人对象</param>
    /// <param name="ability">英雄使用的技能</param>
    public void ChoseAbility(Enemy enemy, Ability ability)
    {
        _currentHero.UseAbility(enemy, ability);
    }

    /// <summary>
    /// 当英雄选择使用物品时调用。
    /// </summary>
    /// <param name="item">使用的物品对象</param>
    /// <param name="target">物品使用目标（英雄或敌人）</param>
    public void ChoseUseItem(Item item, Battler target)
    {
        _currentHero.UseItem(item, target);
    }

    /// <summary>
    /// 当英雄选择防御时调用，执行防御操作。
    /// </summary>
    public void ChoseDefend()
    {
        _currentHero.Defend();
    }

    /// <summary>
    /// 获取当前正在操作的英雄对象。
    /// </summary>
    /// <returns>当前操作的英雄</returns>
    public Hero GetCurrentHero()
    {
        return _currentHero;
    }

    /// <summary>
    /// 当某个英雄的回合开始时调用，设置当前英雄并显示操作菜单。
    /// </summary>
    /// <param name="hero">当前开始回合的英雄</param>
    private void StartTurn(Hero hero)
    {
        _currentHero = hero;
        _isActiveTurn = true;
        // 触发主动回合状态改变事件
        OnActiveTurnChanged?.Invoke(_isActiveTurn);
        // 显示战斗操作菜单（通过 UI 系统控制）
        BattleUIHandler.Instance.ToggleActionMenu(true);
    }

    /// <summary>
    /// 当某个敌人的回合开始时调用（敌人回合无需显示操作菜单）。
    /// </summary>
    /// <param name="enemy">当前开始回合的敌人</param>
    private void StartTurn(Enemy enemy)
    {
        _isActiveTurn = true;
        OnActiveTurnChanged?.Invoke(_isActiveTurn);
    }

    /// <summary>
    /// 当前单位结束回合时调用，关闭操作菜单并延迟进入下一回合。
    /// </summary>
    private void EndTurn()
    {
        _isActiveTurn = false;
        // 启动协程延迟结束回合，便于播放动画
        StartCoroutine(EndTurnDelay(_turnDelaySeconds));
        // 关闭操作菜单
        BattleUIHandler.Instance.ToggleActionMenu(false);
    }

    /// <summary>
    /// 延迟结束回合的协程，等待指定时间后触发主动回合状态改变事件。
    /// </summary>
    /// <param name="seconds">等待的秒数</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator EndTurnDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnActiveTurnChanged?.Invoke(_isActiveTurn);
    }

    #endregion
}
