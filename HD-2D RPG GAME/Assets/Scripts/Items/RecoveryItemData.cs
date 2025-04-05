using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemRecoveryType 枚举表示物品恢复的类型，可用于恢复生命值或法力值。
/// </summary>
public enum ItemRecoveryType
{
    Health, // 恢复生命值
    Mana    // 恢复法力值
}

/// <summary>
/// RecoveryItemData 类用于定义可恢复物品的数据，
/// 包括恢复类型和恢复数值。该类继承自 ItemData，
/// 并通过实现 Initialize 方法生成具体的 RecoveryItem 实例。
/// </summary>
[CreateAssetMenu(fileName = "New Recovery Item", menuName = "Item/Recovery Item")]
public class RecoveryItemData : ItemData
{
    /// <summary>
    /// 指定该物品恢复的是生命值还是法力值。
    /// </summary>
    public ItemRecoveryType RecoveryType;

    /// <summary>
    /// 指定使用该物品时所恢复的数值。
    /// </summary>
    public float AmountToRecover;

    /// <summary>
    /// 根据当前 RecoveryItemData 数据初始化一个 RecoveryItem 实例，
    /// 并将该实例加入指定的背包中。
    /// </summary>
    /// <param name="inventory">目标背包</param>
    /// <returns>初始化后的 RecoveryItem 实例</returns>
    public override Item Initialize(Inventory inventory)
    {
        return new RecoveryItem(this, inventory);
    }
}
