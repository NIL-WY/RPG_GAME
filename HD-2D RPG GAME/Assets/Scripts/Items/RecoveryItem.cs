using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// RecoveryItem 类表示用于恢复角色属性（如生命或法力）的物品。
/// 该类继承自 Item 类，并实现了使用物品时对目标进行恢复操作的逻辑。
/// </summary>
public class RecoveryItem : Item
{
    /// <summary>
    /// 恢复类型，标识该物品恢复的是生命值还是法力值。
    /// </summary>
    private ItemRecoveryType _recoveryType;

    /// <summary>
    /// 恢复量，表示该物品能恢复的数值。
    /// </summary>
    private float _recoveryAmount;

    /// <summary>
    /// 构造函数，通过传入物品数据引用和所属背包初始化 RecoveryItem 实例。
    /// 同时尝试将 ReferenceData 转换为 RecoveryItemData 以提取恢复类型和恢复量。
    /// </summary>
    /// <param name="referenceData">物品数据引用，应为 RecoveryItemData 类型</param>
    /// <param name="inventory">所属的背包</param>
    public RecoveryItem(ItemData referenceData, Inventory inventory) : base(referenceData, inventory)
    {
        var data = ReferenceData as RecoveryItemData;
        if (data == null) return;
        _recoveryType = data.RecoveryType;
        _recoveryAmount = data.AmountToRecover;
    }

    /// <summary>
    /// 使用物品，对指定的使用者执行恢复操作。
    /// 根据恢复类型调用使用者的 RecoverHealth 或 RecoverMana 方法进行相应的恢复，
    /// 使用后从所属背包中移除该物品。
    /// </summary>
    /// <param name="user">使用该物品的角色，需要实现恢复健康和法力的方法</param>
    public override void Use(Battler user)
    {
        switch (_recoveryType)
        {
            case ItemRecoveryType.Health:
                user.RecoverHealth(_recoveryAmount);
                break;
            case ItemRecoveryType.Mana:
                user.RecoverMana(_recoveryAmount);
                break;
        }
        Inventory.RemoveItem(ReferenceData);
    }
}
