using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 英雄单位实现类 - 处理玩家控制角色的核心战斗逻辑与表现
/// </summary>
/// <remarks>
/// 继承自Battler基类，实现：
/// 1. 复杂技能系统
/// 2. 防御姿态机制
/// 3. UI事件驱动
/// 4. 动画/音效集成
/// [组件需求] HeroAnimationController, HeroAudioController
/// </remarks>
[RequireComponent(typeof(HeroAnimationController))]
[RequireComponent(typeof(HeroAudioController))]
public class Hero : Battler
{
    
    // 移动系统参数
    [SerializeField] float _moveOffset = 2f;       // 战斗姿态位移距离
    [SerializeField] float _moveSpeed = 9f;       // 位移动画速度
    protected float manaRegenRate = 10f;          // 每回合法力恢复量

    // 防御系统
    private StatModifier defendStanceModifier = new StatModifier(1, StatType.PhysicalDefense, StatModifierType.PercentMultiply); // 防御姿态加成
    private bool isDefending = false;             // 当前是否处于防御状态

    // 组件引用
    private HeroAnimationController animationController;  // 动画控制组件
    private HeroAudioController audioController;          // 音频控制组件

    // 战斗临时数据
    protected float rawDamage;                     // 当前待处理的伤害数值

    // 属性访问器
    public float MoveDistance => _moveOffset;      // 暴露给UI的移动距离

    // UI事件系统 ---------------------------------------------------------------
    public delegate void HealthEventHandler(float health);
    public event HealthEventHandler OnHealthChanged;    // 生命值变化事件

    public delegate void TurnTimerEventHandler(float time);
    public event TurnTimerEventHandler OnTurnTimeChanged; // 行动条变化事件

    public delegate void ManaEventHandler(float mana);
    public event ManaEventHandler OnManaChanged;        // 法力值变化事件

    // 战斗流程事件 -------------------------------------------------------------
    public delegate void StartTurnEventHandler(Hero hero);
    public event StartTurnEventHandler OnStartTurn;     // 回合开始事件

    public delegate void EndTurnEventHandler();
    public event EndTurnEventHandler OnEndTurn;         // 回合结束事件

    // 镜头控制事件 -------------------------------------------------------------
    public delegate void TargetSelfEventHandler(Battler battler);
    public event TargetSelfEventHandler OnTargetSelf;   // 自身目标事件

    public delegate void TargetGroupEventHandler(Battler battler, Battler other);
    public TargetGroupEventHandler OnTargetOther;       // 群体目标事件

    // 伤害回调系统
    private delegate void DealDamageCallback(float damage);
    private DealDamageCallback _dealDamageCallback;     // 伤害延迟回调引用

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    protected virtual void Awake()
    {

        animationController = GetComponent<HeroAnimationController>();
        audioController = GetComponent<HeroAudioController>();
    }

    /// <summary>
    /// 更新行动条UI状态
    /// </summary>
    protected override void TickTurnTimer()
    {
        if (IsDead) return;
        UpdateTimeUI();
        base.TickTurnTimer();
    }

    /// <summary>
    /// 执行普通攻击
    /// </summary>
    /// <param name="enemyTarget">目标敌人</param>
    /// <remarks>
    /// 流程：
    /// 1. 计算原始伤害
    /// 2. 绑定伤害回调
    /// 3. 播放攻击动画/音效
    /// 4. 触发目标选择事件
    /// </remarks>
    public virtual void Attack(Enemy enemyTarget)
    {
        if (animationController == null)
        {
            return;
        }

        if (enemyTarget == null)
        {
            return;
        }
        OnDisplayAlert("Attack");
        rawDamage = CalculateDamage(baseDamageMultiplier);
        _dealDamageCallback = enemyTarget.TakeDamage;
        audioController.PlayAttackVoice();
        animationController.PlayAttack();
        OnTargetOther?.Invoke(this, enemyTarget);
    }

    /// <summary>
    /// 动画事件回调 - 在攻击动画命中时刻触发实际伤害
    /// </summary>
    protected virtual void OnHitAnimation()
    {
        _dealDamageCallback?.Invoke(rawDamage);
    }

    /// <summary>
    /// 对敌人使用攻击型技能
    /// </summary>
    /// <param name="enemyTarget">目标敌人</param>
    /// <param name="ability">使用的技能</param>
    /// <remarks>
    /// 流程：
    /// 1. 触发技能效果
    /// 2. 消耗法力值
    /// 3. 播放特殊攻击动画
    /// </remarks>
    public virtual void UseAbility(Enemy enemyTarget, Ability ability)
    {
        if (animationController == null)
        {
            return;
        }

        if (enemyTarget == null)
        {
            return;
        }
        AttackAbility attackAbility = ability as AttackAbility;
        attackAbility.Trigger(this, enemyTarget, out rawDamage);
        _dealDamageCallback = enemyTarget.TakeDamage;
        UseMana(ability.ManaCost);
        OnDisplayAlert(ability.Name);
        audioController.PlaySpecialAttackVoice();
        animationController.PlaySpecialAttack();
        OnTargetOther?.Invoke(this, enemyTarget);
    }

    /// <summary>
    /// 对自身使用支援型技能
    /// </summary>
    /// <param name="ability">使用的技能</param>
    public virtual void UseAbility(Ability ability)
    {
        SupportAbility buffAbility = ability as SupportAbility;
        buffAbility.Trigger(this);
        UseMana(ability.ManaCost);
        OnDisplayAlert(ability.Name);
        audioController.PlaySelfBuffVoice();
        animationController.PlayBuff();
        OnTargetSelf?.Invoke(this);
    }

    /// <summary>
    /// 对队友使用技能
    /// </summary>
    /// <param name="allyTarget">目标队友</param>
    /// <param name="ability">使用的技能</param>
    public virtual void UseAbility(Hero allyTarget, Ability ability)
    {
        OnTargetOther?.Invoke(this, allyTarget);
        UseMana(ability.ManaCost);
    }

    /// <summary>
    /// 法力消耗处理（扩展基类功能）
    /// </summary>
    protected override void UseMana(float manaUsed)
    {
        base.UseMana(manaUsed);
        UpdateManaUI();
    }

    /// <summary>
    /// 进入防御姿态
    /// </summary>
    /// <remarks>
    /// 1. 添加防御属性加成
    /// 2. 播放防御动画
    /// 3. 持续至回合结束
    /// </remarks>
    public virtual void Defend()
    {
        if (animationController == null)
        {
            return;
        }
        OnDisplayAlert("Defend");
        audioController.PlayStartGuardVoice();
        isDefending = true;
        AddModifier(defendStanceModifier);
        animationController.PlayDefend();
        Debug.Log(gameObject.name + " defends.");
    }

    /// <summary>
    /// 重置防御状态
    /// </summary>
    protected virtual void ResetDefence()
    {
        if (isDefending)
        {
            RemoveModifier(defendStanceModifier);
            isDefending = false;
            animationController.StopDefend();
        }
    }

    /// <summary>
    /// 承受伤害处理（扩展基类逻辑）
    /// </summary>
    /// <remarks>
    /// 新增：
    /// 1. 防御状态音效区分
    /// 2. 受击动画播放
    /// 3. 防御状态不减伤提示
    /// </remarks>
    public override void TakeDamage(float rawDamage)
    {
        if (animationController == null)
        {
            return;
        }
        if (Evade() && !isDefending)
        {
            audioController.PlayEvadeVoice();
            animationController.PlayEvade();
            OnDisplayPopUp(this, "Missed", PopUpType.Damage);
            return;
        }

        if (isDefending) { audioController.PlayGuardVoice(); }
        else { audioController.PlayHurtVoice(); }
        animationController.PlayGetDamaged();
        float damage = rawDamage - physicalDefenseStat.Value;
        if (damage < 0)
            damage = 0;
        currentHealth -= damage;
        OnDisplayPopUp(this, damage.ToString(), PopUpType.Damage);
        UpdateHealthUI();
        CheckDeath();
    }

    /// <summary>
    /// 每回合自动恢复法力
    /// </summary>
    protected virtual void RegenerateMana()
    {
        if (currentMana + manaRegenRate > maxManaStat.Value)
            currentMana = maxManaStat.Value;
        else
            currentMana += manaRegenRate;
        OnDisplayPopUp(this, manaRegenRate.ToString(), PopUpType.Mana);
        UpdateManaUI();
    }

    /// <summary>
    /// 使用道具（扩展基类功能）
    /// </summary>
    /// <remarks>
    /// 新增道具使用动画/音效
    /// </remarks>
    public override void UseItem(Item item, Battler user)
    {
        if (animationController == null)
        {
            return;
        }
        OnDisplayAlert(item.Name);
        item.Use(user);
        audioController.PlayItemUseVoice();
        animationController.PlayUseItem();
        OnTargetSelf?.Invoke(this);
    }

    // UI更新方法组 -------------------------------------------------------------
    public override void RecoverHealth(float amountRecovered)
    {
        base.RecoverHealth(amountRecovered);
        UpdateHealthUI();
    }

    public override void RecoverMana(float amountRecovered)
    {
        base.RecoverMana(amountRecovered);
        UpdateManaUI();
    }

    /// <summary>
    /// 回合开始处理
    /// </summary>
    /// <remarks>
    /// 执行顺序：
    /// 1. 播放语音
    /// 2. 法力恢复
    /// 3. 重置防御
    /// 4. 前移角色
    /// 5. 触发事件
    /// </remarks>
    protected override void StartTurn()
    {
        if (IsDead) return;
        audioController.PlayStartTurnVoice();
        RegenerateMana();
        ResetDefence();
        StartCoroutine(MoveLeft());
        OnStartTurn?.Invoke(this);
    }

    /// <summary>
    /// 回合结束处理（动画事件触发）
    /// </summary>
    /// <remarks>
    /// 1. 更新状态效果
    /// 2. 移回原位
    /// 3. 重置计时器
    /// </remarks>
    protected override void EndTurn()
    {
        TickStatusEffects();
        StartCoroutine(MoveRight());
        turnTimer = 0;
        OnEndTurn?.Invoke();
    }
    private void PlayDeathAnimation()
    {
        animationController.PlayDeath();
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("英雄死亡");
        PlayDeathAnimation();
        isTurnTimerActive = false;
        OnStartTurn = null;
        OnEndTurn = null;
        OnHealthChanged = null;
        OnManaChanged = null;
        OnTurnTimeChanged = null;
        // 停止所有协程
        StopAllCoroutines();

        // 从战斗管理器移除
        BattleManager.Instance.heroes.Remove(this);

        // 禁用战斗相关组件
        enabled = false;
    }
    // UI更新辅助方法 -----------------------------------------------------------
    protected void UpdateManaUI() => OnManaChanged?.Invoke(currentMana);
    protected void UpdateHealthUI() => OnHealthChanged?.Invoke(currentHealth);
    protected void UpdateTimeUI() => OnTurnTimeChanged?.Invoke(turnTimer);

    #region 协程方法组
    /// <summary>
    /// 前移协程（进入战斗位置）
    /// </summary>
    private IEnumerator MoveLeft()
    {
        animationController.PlayMoveForward();
        float startingXpos = transform.position.x;
        while (transform.position.x > startingXpos - _moveOffset)
        {
            transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
        animationController.PlayReady();
    }

    /// <summary>
    /// 后移协程（返回原位）
    /// </summary>
    private IEnumerator MoveRight()
    {
        animationController.PlayMoveBackward();
        float startingXpos = transform.position.x;
        while (transform.position.x < startingXpos + _moveOffset)
        {
            transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
        animationController.PlayIdle();
    }
    private void OnDestroy()
    {
        // 解除所有动画事件绑定
        if (animationController != null)
        {
            animationController.PlayIdle();
            animationController = null;
        }

        // 取消所有UI事件订阅
        OnHealthChanged = null;
        OnManaChanged = null;
        OnTurnTimeChanged = null;
        OnStartTurn = null;
        OnEndTurn = null;
    }
    #endregion
}