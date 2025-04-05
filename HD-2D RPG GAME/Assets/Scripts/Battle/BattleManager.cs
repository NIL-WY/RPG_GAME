using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 战斗管理器，负责管理战斗流程、轮换回合、响应 UI 操作等。
/// </summary>
public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// 当前战斗中的敌人列表
    /// </summary>
    public List<Enemy> enemies;

    /// <summary>
    /// 当前战斗中的我方角色（英雄）列表
    /// </summary>
    public List<Hero> heroes;

    /// <summary>
    /// 英雄初始位置，用于战斗开始时布阵
    /// </summary>
    public List<Transform> heroesStartingPositions;

    private Hero _currentHero;                // 当前操作的英雄
    private bool _isActiveTurn = false;       // 是否处于主动回合中
    private bool _isFightActive = false;      // 战斗是否激活

    [SerializeField]
    private float _turnDelaySeconds = 0.75f;  // 回合结束后延迟时间（用于过渡动画）

    /// <summary>
    /// 当主动回合状态改变时触发的事件
    /// </summary>
    public delegate void ActiveTurnEvent(bool value);
    public static ActiveTurnEvent OnActiveTurnChanged;
    /// <summary>
    /// 战斗结束事件
    /// </summary>
    public delegate void BattleEndHandler(bool isVictory);
    public static event BattleEndHandler OnBattleEnd;
    /// <summary>
    /// 单例实例（BattleManager 仅存在一个）
    /// </summary>
    public static BattleManager Instance { get; private set; }

    /// <summary>
    /// 订阅 UI 事件
    /// </summary>
    private void OnEnable()
    {
        BattleUIHandler.OnSelectEnemyAttack += ChoseAttack;
        BattleUIHandler.OnSelectEnemyAbility += ChoseAbility;
    }

    /// <summary>
    /// Awake 生命周期，设置单例
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Start 生命周期，激活战斗并初始化战斗对象监听
    /// </summary>
    private void Start()
    {
        _isFightActive = true;
        Initialize();
    }
    private void Update()
    {
        if (!_isFightActive) return;
        CheckBattleEndCondition();

    }
    // 战斗结束检查方法
    private void CheckBattleEndCondition()
    {
        if (heroes.Count == 0 || AllHeroesDead())
        {
            EndBattle(false); // 失败
            return;
        }

        if (enemies.Count == 0)
        {
            EndBattle(true); // 胜利
        }
    }
    // 新增全英雄死亡检查
    private bool AllHeroesDead()
    {
        return heroes.All(hero => hero.IsDead);
    }

    // 新增战斗结束处理
    private void EndBattle(bool isVictory)
    {
        _isFightActive = false;
        OnBattleEnd?.Invoke(isVictory);

        // 停止所有单位的回合计时器
        foreach (var hero in heroes)
        {
            hero.ToggleTurnTimer(true);
        }
        foreach (var enemy in enemies)
        {
            enemy.ToggleTurnTimer(true);
        }

        // 触发UI显示
        BattleUIHandler.Instance.ShowBattleResult(isVictory);      
    }

    

    /// <summary>
    /// 初始化英雄和敌人对象的事件监听（用于回合控制）
    /// </summary>
    private void Initialize()
    {
        foreach (Hero hero in heroes)
        {
            hero.OnDeath += HandleHeroDeath;
            hero.OnStartTurn += StartTurn;
            hero.OnEndTurn += EndTurn;
        }

        foreach (Enemy enemy in enemies)
        {
            enemy.OnDeath += HandleEnemyDeath;
            enemy.OnStartTurn += StartTurn;
            enemy.OnEndTurn += EndTurn;
        }
    }
    // 新增死亡事件处理
    private void HandleHeroDeath(Battler hero)
    {
        CheckBattleEndCondition();
    }

    private void HandleEnemyDeath(Battler enemy)
    {
        CheckBattleEndCondition();
    }
    /// <summary>
    /// 英雄选择普通攻击敌人
    /// </summary>
    /// <param name="enemy">被攻击的敌人</param>
    public void ChoseAttack(Enemy enemy)
    {
        _currentHero.Attack(enemy);
    }

    /// <summary>
    /// 英雄选择使用技能（不指定目标时）
    /// </summary>
    /// <param name="ability">技能</param>
    public void ChoseAbility(Ability ability)
    {
        _currentHero.UseAbility(ability);
    }

    /// <summary>
    /// 英雄选择使用技能（指定目标敌人）
    /// </summary>
    /// <param name="enemy">目标敌人</param>
    /// <param name="ability">技能</param>
    public void ChoseAbility(Enemy enemy, Ability ability)
    {
        _currentHero.UseAbility(enemy, ability);
    }

    /// <summary>
    /// 英雄选择使用物品
    /// </summary>
    /// <param name="item">使用的物品</param>
    /// <param name="target">目标角色</param>
    public void ChoseUseItem(Item item, Battler target)
    {
        _currentHero.UseItem(item, target);
    }

    /// <summary>
    /// 英雄选择防御
    /// </summary>
    public void ChoseDefend()
    {
        _currentHero.Defend();
    }

    /// <summary>
    /// 获取当前操作的英雄对象
    /// </summary>
    /// <returns>当前英雄</returns>
    public Hero GetCurrentHero()
    {
        return _currentHero;
    }

    /// <summary>
    /// 英雄开始自己的回合（显示操作菜单）
    /// </summary>
    /// <param name="hero">当前回合的英雄</param>
    private void StartTurn(Hero hero)
    {
        _currentHero = hero;
        _isActiveTurn = true;
        OnActiveTurnChanged?.Invoke(_isActiveTurn);
        BattleUIHandler.Instance.ToggleActionMenu(true);
    }

    /// <summary>
    /// 敌人开始自己的回合（无需显示 UI 菜单）
    /// </summary>
    /// <param name="enemy">当前回合的敌人</param>
    private void StartTurn(Enemy enemy)
    {
        _isActiveTurn = true;
        OnActiveTurnChanged?.Invoke(_isActiveTurn);
    }

    /// <summary>
    /// 当前单位结束回合，关闭操作菜单并延迟进入下一回合
    /// </summary>
    private void EndTurn()
    {
        _isActiveTurn = false;
        StartCoroutine(EndTurnDelay(_turnDelaySeconds));
        BattleUIHandler.Instance.ToggleActionMenu(false);
    }

    /// <summary>
    /// 结束回合的延迟协程（用于播放攻击/技能动画）
    /// </summary>
    /// <param name="seconds">延迟时间</param>
    private IEnumerator EndTurnDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnActiveTurnChanged?.Invoke(_isActiveTurn);
    }
}
