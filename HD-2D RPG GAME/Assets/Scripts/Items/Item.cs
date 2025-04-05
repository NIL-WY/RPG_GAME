using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示可用于战斗或场景中的物品的抽象基类。
/// 每个物品都与其数据引用 (ItemData) 及所属背包 (Inventory) 关联。
/// </summary>
public abstract class Item
{
    /// <summary>
    /// 与该物品相关联的数据资源（ScriptableObject）
    /// </summary>
    public readonly ItemData ReferenceData;

    /// <summary>
    /// 此物品所在的背包实例
    /// </summary>
    public readonly Inventory Inventory;

    /// <summary>
    /// 物品名称（从 ItemData 获取）
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 物品描述（从 ItemData 获取）
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// 当前物品的堆叠数量
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// 构造函数，初始化物品的基本信息
    /// </summary>
    /// <param name="referenceData">该物品的数据引用</param>
    /// <param name="inventory">所属背包</param>
    protected Item(ItemData referenceData, Inventory inventory)
    {
        ReferenceData = referenceData;
        Name = ReferenceData.Name;
        Description = ReferenceData.Description;
        Quantity = 1;
        Inventory = inventory;
    }

    /// <summary>
    /// 增加堆叠数量（拾取同类物品）
    /// </summary>
    public void AddToStack()
    {
        Quantity++;
    }

    /// <summary>
    /// 减少堆叠数量（使用或丢弃物品）
    /// </summary>
    public void RemoveFromStack()
    {
        Quantity--;
    }

    /// <summary>
    /// 使用物品的抽象方法，需要在子类中具体实现逻辑。
    /// </summary>
    /// <param name="user">使用该物品的角色</param>
    public abstract void Use(Battler user);
}
