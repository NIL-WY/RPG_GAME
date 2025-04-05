using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人单位实现类 - 处理敌方单位的战斗逻辑和行为
/// </summary>
/// <remarks>
/// 继承自Battler基类，实现敌人特有的回合控制逻辑
/// </remarks>
public class Enemy : Battler
{
    // 目标管理
    protected List<Hero> heroes;  // 战场中的英雄列表引用

    // 回合事件系统
    public delegate void StartTurnEventHandler(Enemy enemy);
    public event StartTurnEventHandler OnStartTurn;  // 回合开始事件（用于AI决策/UI更新）
    public delegate void EndTurnEventHandler();
    public event EndTurnEventHandler OnEndTurn;      // 回合结束事件（用于战斗流程控制）

    /// <summary>
    /// 初始化敌人单位
    /// </summary>
    protected override void Start()
    {
        base.Start();  // 调用基类初始化
        heroes = BattleManager.Instance.heroes;  // 从战斗管理器获取英雄列表
    }

    /// <summary>
    /// 执行攻击行为（基础攻击逻辑）
    /// </summary>
    /// <param name="hero">攻击目标</param>
    /// <remarks>
    /// 执行流程：
    /// 1. 显示攻击提示
    /// 2. 计算并施加伤害
    /// 3. 延迟结束回合（等待攻击动画完成）
    /// </remarks>
    protected virtual void Attack(Hero hero)
    {
        Debug.Log(gameObject.name + " attacked " + hero.gameObject.name);
        OnDisplayAlert("Attack");  // 触发UI提示

        // 使用基础伤害倍率计算伤害
        hero.TakeDamage(CalculateDamage(baseDamageMultiplier));

        // 延迟结束回合以匹配攻击动画时长
        StartCoroutine(DelayEndTurn(1));
    }

    /// <summary>
    /// 随机选择目标英雄
    /// </summary>
    /// <returns>随机选取的英雄对象</returns>
    /// <remarks>
    /// 当前实现为完全随机选择，后续可扩展为：
    /// - 优先攻击血量最低目标
    /// - 使用仇恨系统选择目标
    /// </remarks>
    protected virtual Hero PickRandomHero()
    {
        int index = Random.Range(0, heroes.Count);
        Hero hero = heroes[index];
        return hero;
    }

    /// <summary>
    /// 敌人回合开始逻辑
    /// </summary>
    /// <remarks>
    /// 触发顺序：
    /// 1. 触发OnStartTurn事件通知其他系统
    /// 2. 执行攻击决策
    /// </remarks>
    protected override void StartTurn()
    {
        OnStartTurn?.Invoke(this);  // 通知战斗管理器/UI系统
        Attack(PickRandomHero());   // 执行攻击行为
    }

    /// <summary>
    /// 敌人回合结束逻辑
    /// </summary>
    /// <remarks>
    /// 1. 重置计时器
    /// 2. 触发结束回合事件
    /// </remarks>
    protected override void EndTurn()
    {
        turnTimer = 0;  // 重置行动条
        OnEndTurn?.Invoke();  // 通知战斗管理器
    }

    /// <summary>
    /// 对象销毁时的清理操作
    /// </summary>
    protected void OnDestroy()
    {
        // 安全取消事件订阅
        if (FindObjectOfType<BattleManager>())
        {
            BattleManager.OnActiveTurnChanged -= ToggleTurnTimer;
        }
    }
    protected override void Die()
    {
        base.Die();

        // 从战斗管理器移除
        BattleManager.Instance.enemies.Remove(this);

        // 直接销毁对象
        Destroy(gameObject);
    }
    /// <summary>
    /// 延迟结束回合协程
    /// </summary>
    /// <param name="seconds">延迟秒数</param>
    /// <returns>IEnumerator协程对象</returns>
    /// <remarks>
    /// 用于等待攻击动画/特效播放完成再正式结束回合
    /// </remarks>
    public IEnumerator DelayEndTurn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EndTurn();
    }
}

/* 典型使用场景：
1. 创建不同敌人类型：
   - 继承Enemy类实现BossEnemy等子类
   - 重写Attack方法实现特殊攻击模式

2. 事件订阅示例：
   enemy.OnStartTurn += (e) => {
       Debug.Log($"敌人 {e.Name} 开始行动");
       // 显示敌人行动提示UI
   };

3. 扩展AI决策逻辑：
   protected override void StartTurn()
   {
       if(ShouldUseSpecialAbility())
       {
           UseSpecialAbility();
       }
       else
       {
           base.StartTurn();
       }
   }
*/

/* 注意事项：
1. 目标选择安全性：
   - 当前实现未检查heroes列表是否为空
   - 建议添加防御性检查：
     if (heroes.Count == 0) return null;

2. 动画事件集成：
   - 理想情况下应通过动画事件回调触发EndTurn
   - 当前使用固定时间延迟可能产生不同步

3. 扩展建议：
   - 添加行为树/状态机实现复杂AI
   - 增加技能使用逻辑（而不仅是普通攻击）
   - 实现仇恨值系统替代随机目标选择
*/