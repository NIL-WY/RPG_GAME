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
    public static PartyManager Instance { get; private set; }
    public PartyData currentParty;


    public void SaveFormation(Vector3[] positions)
    {
        currentParty.formationPositions = positions;
    }
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
            DontDestroyOnLoad(this.gameObject);
            // 初始化队伍数据，初始只有玩家角色
            currentParty = new PartyData { memberIDs = new string[] { "Fox" } };
            Inventory = new Inventory(); // 初始化背包数据
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

}
