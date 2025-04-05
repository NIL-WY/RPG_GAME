using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// 战斗单位抽象基类 - 定义角色战斗系统的核心逻辑和数据
/// </summary>
/// <remarks>
/// 包含属性管理、技能系统、状态效果、回合计时等核心机制
/// 派生类需实现具体的回合开始/结束逻辑
/// </remarks>
public abstract class Battler : MonoBehaviour
{
    // 常量配置
    protected const float CHANCE_CAP = 1000f;        // 概率计算上限值（千分比系统）
    protected const float TURN_TIMER_MAX = 100f;     // 回合计时器最大值

    // 基础属性字段
    [SerializeField] protected string charName = "";         // 角色名称
    [SerializeField] protected float currentHealth = 100f;   // 当前生命值
    [SerializeField] protected float currentMana = 100f;     // 当前法力值

    // 角色属性配置
    [Header("角色属性配置")]
    [SerializeField] protected CharacterStat maxHealthStat;         // 最大生命值属性
    [SerializeField] protected CharacterStat maxManaStat;           // 最大法力值属性
    [SerializeField] protected CharacterStat physicalAttackStat;    // 物理攻击属性
    [SerializeField] protected CharacterStat physicalDefenseStat;   // 物理防御属性
    [SerializeField] protected CharacterStat speedStat;             // 速度属性
    [SerializeField] protected CharacterStat criticalStat;          // 暴击率属性
    [SerializeField] protected CharacterStat evasionStat;           // 闪避率属性

    // 战斗系统相关
    [Header("战斗系统")]
    [SerializeField] protected float baseDamageMultiplier = 1.0f;    // 基础伤害倍率
    [SerializeField] protected float critDamageMultiplier = 1.25f;   // 暴击伤害倍率
    [SerializeField] protected List<AbilityData> abilitiesData;      // 可使用的技能数据列表

    // 状态管理
    public readonly List<CharacterStat> Stats;                       // 所有属性集合
    protected readonly List<Ability> abilities;                      // 实际技能实例列表
    public readonly ReadOnlyCollection<Ability> Abilities;           // 对外暴露的只读技能列表
    protected Dictionary<StatusEffectData, StatusEffect> statusEffects; // 当前生效的状态效果
    public bool IsDead { get; protected set; } // 死亡状态标识
    // 新增死亡事件
    public delegate void DeathEventHandler(Battler battler);
    public event DeathEventHandler OnDeath;
    // 回合计时
    protected float turnTimer = 0;                   // 当前回合计时进度
    protected bool isTurnTimerActive = false;        // 是否启用回合计时

    // 事件系统
    public delegate void DisplayPopUpEventHandler(Battler battler, string message, PopUpType type);
    public event DisplayPopUpEventHandler DisplayPopUp;  // 战斗信息弹出事件（伤害/治疗等）

    public delegate void DisplayAlertMessage(string message);
    public event DisplayAlertMessage DisplayAlert;       // 通用提示信息事件

    /// <summary>
    /// 角色名称属性（自动截断过长名称）
    /// </summary>
    public string Name
    {
        get { return charName; }
        set { if (value.Length > 49) { Debug.LogWarning("Battler set with a name longer than 49 characters! Name will not fit UI."); } }
    }

    // 属性访问器
    public float CurrentHealth { get { return currentHealth; } }
    public float CurrentMana { get { return currentMana; } }
    public float MaxHealth { get { return maxHealthStat.Value; } }
    public float MaxMana { get { return maxManaStat.Value; } }

    /// <summary>
    /// 构造函数初始化核心组件
    /// </summary>
    public Battler()
    {
        // 初始化容器
        abilitiesData = new List<AbilityData>();
        abilities = new List<Ability>();
        statusEffects = new Dictionary<StatusEffectData, StatusEffect>();
        Abilities = abilities.AsReadOnly();

        // 构建属性系统
        maxHealthStat = new CharacterStat(StatType.MaxHealth);
        maxManaStat = new CharacterStat(StatType.MaxMana);
        physicalAttackStat = new CharacterStat(StatType.PhysicalAttack);
        physicalDefenseStat = new CharacterStat(StatType.PhysicalDefense);
        speedStat = new CharacterStat(StatType.Speed);
        criticalStat = new CharacterStat(StatType.Critical);
        evasionStat = new CharacterStat(StatType.Evasion);

        Stats = new List<CharacterStat>
        {
            maxHealthStat,
            maxManaStat,
            physicalAttackStat,
            speedStat,
            criticalStat,
            evasionStat
        };
    }

    /// <summary>
    /// 初始化战斗单位
    /// </summary>
    protected virtual void Start()
    {
        // 初始化数值
        currentHealth = maxHealthStat.Value;
        currentMana = maxManaStat.Value;
        isTurnTimerActive = true;

        // 注册战斗管理器回调
        if (BattleManager.Instance != null)
        {
            BattleManager.OnActiveTurnChanged += ToggleTurnTimer;
        }
        else
        {
            Debug.LogError(gameObject.name + ": Can't find Battle Manager on scene!");
        }

        InitializeAbilities();
    }

    /// <summary>
    /// 每帧更新回合计时器
    /// </summary>
    protected virtual void Update()
    {
        if (isTurnTimerActive)
            TickTurnTimer();
    }

    /// <summary>
    /// 初始化技能系统
    /// </summary>
    /// <remarks>
    /// 遍历所有技能数据创建对应的技能实例
    /// </remarks>
    protected void InitializeAbilities()
    {
        foreach (AbilityData data in abilitiesData)
        {
            Ability ability = data.Initialize(this.gameObject);
            abilities.Add(ability);
        }
    }

    // 抽象方法定义
    protected abstract void StartTurn();  // 开始回合的派生类实现
    protected abstract void EndTurn();    // 结束回合的派生类实现

    /// <summary>
    /// 计算最终伤害值（含暴击判断）
    /// </summary>
    /// <param name="damageMultiplier">技能伤害倍率</param>
    /// <returns>最终伤害值（含1%随机浮动）</returns>
    public virtual float CalculateDamage(float damageMultiplier)
    {
        float finalDamage = physicalAttackStat.Value * damageMultiplier;
        float critChance = criticalStat.Value;
        float randValue = Random.value;

        // 暴击率计算（千分比系统）
        if (critChance > CHANCE_CAP) { critChance = CHANCE_CAP; }
        critChance /= CHANCE_CAP;

        if (randValue < critChance)
        {
            finalDamage *= critDamageMultiplier;
        }
        return Mathf.Round(Random.Range(finalDamage, (finalDamage * 1.01f)));
    }

    /// <summary>
    /// 更新回合计时器
    /// </summary>
    protected virtual void TickTurnTimer()
    {
        if (turnTimer < TURN_TIMER_MAX)
        {
            turnTimer += Time.deltaTime * speedStat.Value;
        }
        else if (turnTimer > TURN_TIMER_MAX)
        {
            Debug.Log(gameObject.name + " has reached it's turn.");
            StartTurn();
        }
    }

    /// <summary>
    /// 切换回合计时器状态
    /// </summary>
    /// <param name="value">是否暂停计时</param>
    public virtual void ToggleTurnTimer(bool value)
    {
        if (IsDead) return;
        isTurnTimerActive = !value;
    }

    /// <summary>
    /// 承受伤害处理
    /// </summary>
    /// <param name="rawDamage">原始伤害值</param>
    /// <remarks>
    /// 处理流程：
    /// 1. 闪避判定
    /// 2. 防御减伤计算
    /// 3. 实际伤害应用
    /// </remarks>
    public virtual void TakeDamage(float rawDamage)
    {
        if (IsDead) return;
        if (Evade())
        {
            Debug.Log(gameObject.name + " evaded.");
            OnDisplayPopUp(this, "MISSED", PopUpType.Damage);
            return;
        }

        float damage = rawDamage - physicalDefenseStat.Value;
        damage = Mathf.Max(damage, 0);  // 确保最小伤害为0
        currentHealth -= damage;
        OnDisplayPopUp(this, damage.ToString(), PopUpType.Damage);
        CheckDeath(); // 新增死亡检查
    }

    /// <summary>
    /// 消耗法力值
    /// </summary>
    /// <param name="manaUsed">消耗量</param>
    protected virtual void UseMana(float manaUsed)
    {
        currentMana -= manaUsed;
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="item">道具实例</param>
    /// <param name="user">使用者</param>
    public virtual void UseItem(Item item, Battler user)
    {
        OnDisplayAlert(item.Name);
        item.Use(user);
        EndTurn();
    }

    /// <summary>
    /// 恢复生命值
    /// </summary>
    /// <param name="amountRecovered">恢复量</param>
    public virtual void RecoverHealth(float amountRecovered)
    {
        OnDisplayPopUp(this, amountRecovered.ToString(), PopUpType.Health);
        currentHealth = Mathf.Min(currentHealth + amountRecovered, MaxHealth);
    }

    /// <summary>
    /// 恢复法力值
    /// </summary>
    /// <param name="amountRecovered">恢复量</param>
    public virtual void RecoverMana(float amountRecovered)
    {
        OnDisplayPopUp(this, amountRecovered.ToString(), PopUpType.Mana);
        currentMana = Mathf.Min(currentMana + amountRecovered, MaxMana);
    }

    /// <summary>
    /// 闪避判定
    /// </summary>
    /// <returns>是否成功闪避</returns>
    /// <remarks>
    /// 使用千分比系统计算概率
    /// </remarks>
    protected virtual bool Evade()
    {
        float evasionChance = evasionStat.Value;
        evasionChance = Mathf.Min(evasionChance, CHANCE_CAP);
        return Random.value < (evasionChance / CHANCE_CAP);
    }
    protected virtual void CheckDeath()
    {
        if (currentHealth <= 0 && !IsDead)
        {
            currentHealth = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
        isTurnTimerActive = false; // 停止回合计时器
        OnDeath?.Invoke(this);
    }

    /// <summary>
    /// 更新所有状态效果
    /// </summary>
    /// <remarks>
    /// 使用.ToList()创建副本遍历以避免修改集合错误
    /// </remarks>
    protected void TickStatusEffects()
    {
        foreach (StatusEffect status in statusEffects.Values.ToList())
        {
            status.Tick();
            if (status.isFinished)
            {
                statusEffects.Remove(status.Data);
            }
        }
    }

    /// <summary>
    /// 添加状态效果
    /// </summary>
    /// <param name="statusEffect">要添加的效果实例</param>
    /// <remarks>
    /// 已存在同类型效果时会刷新持续时间
    /// </remarks>
    public void AddStatusEffect(StatusEffect statusEffect)
    {
        Debug.Log("Status effect added: " + statusEffect.Data.Name + " on " + this.Name);
        if (statusEffects.ContainsKey(statusEffect.Data))
        {
            statusEffects[statusEffect.Data].Start(this);
        }
        else
        {
            statusEffects.Add(statusEffect.Data, statusEffect);
            statusEffects[statusEffect.Data].Start(this);
        }
    }

    /// <summary>
    /// 添加属性修饰符
    /// </summary>
    /// <param name="statMod">要添加的修饰符</param>
    /// <remarks>
    /// 线性查找对应属性，时间复杂度O(n)
    /// </remarks>
    public void AddModifier(StatModifier statMod)
    {
        foreach (CharacterStat stat in Stats)
        {
            if (stat.Type == statMod.StatType)
            {
                stat.AddModifier(statMod);
                return;
            }
        }
    }

    /// <summary>
    /// 移除属性修饰符
    /// </summary>
    /// <param name="statMod">要移除的修饰符</param>
    public void RemoveModifier(StatModifier statMod)
    {
        foreach (CharacterStat stat in Stats)
        {
            if (stat.Type == statMod.StatType)
            {
                stat.RemoveModifier(statMod);
                return;
            }
        }
    }

    /// <summary>
    /// 移除指定来源的所有修饰符
    /// </summary>
    /// <param name="source">效果来源对象</param>
    public void RemoveAllModifierFromSource(object source)
    {
        speedStat.RemoveAllModifierFromSource(source);
    }

    // 事件触发方法
    protected virtual void OnDisplayAlert(string message)
    {
        DisplayAlert?.Invoke(message);
    }

    protected virtual void OnDisplayPopUp(Battler battler, string message, PopUpType type)
    {
        DisplayPopUp?.Invoke(battler, message, type);
    }
}