using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 英雄UI控制器 - 管理单个英雄单位的界面元素显示
/// </summary>
/// <remarks>
/// 功能说明：
/// 1. 实时显示英雄名称、生命值、法力值和回合计时条
/// 2. 通过事件驱动实现数据与UI的自动同步
/// 3. 支持动态绑定和解绑英雄数据源
/// 4. 自动处理UI元素的取值范围和文本格式化
/// </remarks>
public class HeroUIController : MonoBehaviour
{
    [Header("UI组件绑定")]
    [Tooltip("英雄名称文本组件")]
    [SerializeField] TextMeshProUGUI _nameText;
    [Tooltip("回合计时进度条")]
    [SerializeField] Slider _timerBar;
    [Tooltip("法力值进度条")]
    [SerializeField] Slider _manaBar;
    [Tooltip("生命值进度条")]
    [SerializeField] Slider _healthBar;
    [Tooltip("生命值数值文本")]
    [SerializeField] TextMeshProUGUI _healthText;
    [Tooltip("法力值数值文本")]
    [SerializeField] TextMeshProUGUI _manaText;

    // 状态缓存
    private float _maxHealth;    // 最大生命值缓存
    private float _maxMana;      // 最大法力值缓存
    private Hero _hero;          // 绑定的英雄对象引用

    /// <summary>
    /// 组件启用时重新初始化（确保对象池重用时的正确状态）
    /// </summary>
    private void OnEnable()
    {
        if (_hero != null)
            Initialize(_hero);
    }

    /// <summary>
    /// 组件禁用时清理资源（防止内存泄漏）
    /// </summary>
    private void OnDisable()
    {
        if (_hero != null)
        {
            // 取消所有事件订阅
            _hero.OnHealthChanged -= UpdateHealth;
            _hero.OnManaChanged -= UpdateMana;
            _hero.OnTurnTimeChanged -= UpdateTurnTimer;
        }
    }

    /// <summary>
    /// 初始化UI控制器并绑定英雄数据
    /// </summary>
    /// <param name="hero">要绑定的英雄对象</param>
    /// <exception cref="System.NullReferenceException">当传入空英雄对象时抛出</exception>
    /// <remarks>
    /// 执行流程：
    /// 1. 缓存基础数值
    /// 2. 初始化所有UI元素
    /// 3. 订阅数值变更事件
    /// </remarks>
    public void Initialize(Hero hero)
    {
        // 清除旧事件绑定（防止重复订阅）
        if (_hero != null)
        {
            _hero.OnHealthChanged -= UpdateHealth;
            _hero.OnManaChanged -= UpdateMana;
            _hero.OnTurnTimeChanged -= UpdateTurnTimer;
        }

        _hero = hero;

        // 名称显示
        _nameText.SetText(hero.Name);

        // 生命值系统初始化
        _maxHealth = hero.MaxHealth;
        _healthBar.maxValue = _maxHealth;
        UpdateHealth(hero.CurrentHealth);

        // 法力值系统初始化
        _maxMana = hero.MaxMana;
        _manaBar.maxValue = _maxMana;
        UpdateMana(hero.CurrentMana);

        // 回合计时条初始化
        _timerBar.maxValue = 100;
        gameObject.SetActive(true);

        // 事件订阅
        hero.OnHealthChanged += UpdateHealth;
        hero.OnTurnTimeChanged += UpdateTurnTimer;
        hero.OnManaChanged += UpdateMana;
    }

    /// <summary>
    /// 更新生命值显示
    /// </summary>
    /// <param name="health">当前生命值</param>
    /// <remarks>
    /// 同时更新进度条和数值文本，自动限制有效范围
    /// </remarks>
    private void UpdateHealth(float health)
    {
        float clampedHealth = Mathf.Clamp(health, 0, _maxHealth);
        _healthBar.value = clampedHealth;
        _healthText.SetText($"{clampedHealth} / {_maxHealth}");
    }

    /// <summary>
    /// 更新法力值显示
    /// </summary>
    /// <param name="mana">当前法力值</param>
    private void UpdateMana(float mana)
    {
        float clampedMana = Mathf.Clamp(mana, 0, _maxMana);
        _manaBar.value = clampedMana;
        _manaText.SetText($"{clampedMana} / {_maxMana}");
    }

    /// <summary>
    /// 更新回合计时条
    /// </summary>
    /// <param name="time">当前计时进度（0-100）</param>
    private void UpdateTurnTimer(float time)
    {
        _timerBar.value = Mathf.Clamp(time, 0, 100);
    }
}

/* 使用示例：
1. 在队伍生成时初始化：
   foreach (var hero in activeHeroes) {
       var uiController = Instantiate(uiPrefab).GetComponent<HeroUIController>();
       uiController.Initialize(hero);
   }

2. 自动更新机制：
   - 当英雄生命值变化时，UI自动更新
   - 回合计时条随战斗管理器更新

注意事项：
1. 确保所有UI组件在Inspector中正确绑定
2. 英雄对象销毁前需解除UI绑定
3. 数值文本格式可通过TextMeshPro组件自定义
4. 进度条动画效果需在Slider组件中配置

性能优化建议：
1. 添加UI更新频率限制（如每0.2秒最多更新一次）
2. 使用对象池管理UI实例
3. 对不可见的UI禁用更新逻辑
4. 使用颜色渐变提示危险值（如生命值低于30%变红）
*/