using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemData 是所有物品数据的抽象基类，继承自 ScriptableObject。
/// 用于存储物品的基础信息（名称、描述），并提供初始化物品实例的方法。
/// </summary>
public abstract class ItemData : ScriptableObject
{
    /// <summary>
    /// 物品的名称
    /// </summary>
    public string Name;

    /// <summary>
    /// 物品的描述信息
    /// </summary>
    public string Description;

    /// <summary>
    /// 抽象方法，用于根据当前物品数据初始化对应的物品实例，并将其加入指定背包。
    /// </summary>
    /// <param name="inventory">目标背包</param>
    /// <returns>初始化后的物品实例</returns>
    public abstract Item Initialize(Inventory inventory);
}
