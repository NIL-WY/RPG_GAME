using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 队伍管理器类，管理玩家的物品背包（Inventory）
/// 并实现单例模式，确保游戏中始终只有一个 PartyManager 实例。
/// </summary>
public class PartyManager : MonoBehaviour
{
    /// <summary>
    /// 单例实例，确保全局唯一。
    /// </summary>
    public static PartyManager Instance;

    /// <summary>
    /// 玩家队伍的物品背包实例。
    /// </summary>
    public Inventory Inventory;

    /// <summary>
    /// 构造函数中初始化 Inventory 实例。
    /// </summary>
    public PartyManager()
    {
        Inventory = new Inventory();
    }

    /// <summary>
    /// 在 Awake 阶段初始化单例逻辑，确保本对象在场景切换时不被销毁。
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // 场景切换时保留该 GameObject
        }
        else
        {
            Destroy(this.gameObject); // 如果已有实例，则销毁当前重复实例
        }
    }

    /// <summary>
    /// 在游戏开始时调用，用于加载预设的测试物品。
    /// </summary>
    void Start()
    {
        PreloadItems();
    }

    /// <summary>
    /// 临时方法：将预设的物品数据加载进玩家的物品背包中。
    /// 用于测试或初始化背包内容。
    /// </summary>
    private void PreloadItems()
    {
        foreach (var itemData in Inventory.preloadItems)
        {
            if (itemData == null) continue;
            Inventory.AddItem(itemData);
        }
    }
}
