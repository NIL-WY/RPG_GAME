using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ActionInfoPopUp 的对象池管理器，用于在战斗中高效重复利用弹出提示组件，
/// 避免频繁生成与销毁带来的性能开销。
/// </summary>
public class PopUpPooler : MonoBehaviour
{
    /// <summary>
    /// 单例实例，便于全局访问。
    /// </summary>
    public static PopUpPooler Instance;

    /// <summary>
    /// 对象池中存放的弹出提示对象列表。
    /// </summary>
    private List<ActionInfoPopUp> _pool;

    /// <summary>
    /// 初始时创建的对象池容量。
    /// </summary>
    [SerializeField] int _startingPoolSize;

    /// <summary>
    /// 用于实例化的 ActionInfoPopUp 预制体。
    /// </summary>
    [SerializeField] ActionInfoPopUp _actionInfoPrefab;

    /// <summary>
    /// 弹出位置在 X 轴上的偏移量，用于美观地展示文本。
    /// </summary>
    [SerializeField] private float _offsetPositionX;

    /// <summary>
    /// 弹出位置在 Z 轴上的偏移量，用于美观地展示文本。
    /// </summary>
    [SerializeField] private float _offsetPositionZ;

    /// <summary>
    /// 构造函数中初始化对象池列表。
    /// </summary>
    public PopUpPooler()
    {
        _pool = new List<ActionInfoPopUp>();
    }

    /// <summary>
    /// Mono 生命周期 Awake，在这里设置单例。
    /// </summary>
    void Awake()
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
    /// 初始化对象池并订阅角色事件。
    /// </summary>
    void Start()
    {
        InitializePool();
        SubscribeToBattlerEvents();
    }

    /// <summary>
    /// 根据设定的初始容量，实例化弹出提示对象并加入池中，初始状态为隐藏。
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < _startingPoolSize; i++)
        {
            var popUp = Instantiate(_actionInfoPrefab, transform.position, Quaternion.identity);
            _pool.Add(popUp.GetComponent<ActionInfoPopUp>());
            popUp.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 订阅所有战斗单位（英雄与敌人）的 DisplayPopUp 事件。
    /// 该事件在战斗中触发，用于显示文本弹窗。
    /// </summary>
    private void SubscribeToBattlerEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.DisplayPopUp += TriggerPopUp;
        }
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.DisplayPopUp += TriggerPopUp;
        }
    }

    /// <summary>
    /// 处理弹出提示的显示逻辑，从对象池中获取可用对象并激活。
    /// </summary>
    /// <param name="battler">触发弹窗的战斗单位</param>
    /// <param name="message">显示的文本内容</param>
    /// <param name="type">弹窗类型（伤害、生命、法力）</param>
    private void TriggerPopUp(Battler battler, string message, PopUpType type)
    {
        ActionInfoPopUp popUp = GetPooledPopUp();
        Vector3 offsetPosition = new Vector3(_offsetPositionX, 0, _offsetPositionZ);
        popUp.transform.position = battler.transform.position + offsetPosition;
        popUp.gameObject.SetActive(true);
        popUp.Activate(message, type);
    }

    /// <summary>
    /// 从对象池中获取一个未激活的弹窗对象，
    /// 若无可用对象，则实例化一个新的并加入池中。
    /// </summary>
    /// <returns>可用的 ActionInfoPopUp 实例</returns>
    private ActionInfoPopUp GetPooledPopUp()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeInHierarchy)
            {
                return _pool[i];
            }
        }
        var popUp = Instantiate(_actionInfoPrefab, transform.position, Quaternion.identity);
        _pool.Add(popUp);

        return popUp;
    }

    /// <summary>
    /// 在该组件销毁前取消所有事件订阅，避免事件未释放造成内存泄漏。
    /// </summary>
    private void OnDestroy()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.DisplayPopUp -= TriggerPopUp;
        }
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.DisplayPopUp -= TriggerPopUp;
        }
    }
}
