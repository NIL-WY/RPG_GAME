using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory 类用于管理背包中的物品，包括添加、删除物品以及预加载测试物品。
/// </summary>
[System.Serializable]
public class Inventory
{
    /// <summary>
    /// 临时物品列表，用于在测试时预加载物品到背包中。
    /// </summary>
    public List<ItemData> preloadItems;

    /// <summary>
    /// 内部列表，存储当前背包中的物品。
    /// </summary>
    private List<Item> _items;

    /// <summary>
    /// 内部字典，用于快速查找物品，通过 ItemData 作为键，对应的 Item 作为值。
    /// </summary>
    private readonly Dictionary<ItemData, Item> _itemDictionary;

    /// <summary>
    /// 只读列表，供外部访问背包中当前所有的物品。
    /// </summary>
    public readonly IReadOnlyList<Item> Items;

    /// <summary>
    /// 构造函数，初始化内部物品列表、字典以及只读物品列表。
    /// </summary>
    public Inventory()
    {
        _items = new List<Item>();
        _itemDictionary = new Dictionary<ItemData, Item>();
        Items = _items;
    }

    /// <summary>
    /// 添加物品到背包中，如果物品已存在则增加堆叠数量，否则创建新的物品实例并加入背包。
    /// </summary>
    /// <param name="itemData">要添加的物品数据对象。</param>
    public void AddItem(ItemData itemData)
    {
        if (_itemDictionary.TryGetValue(itemData, out Item item))
        {
            // 如果物品已存在，则增加堆叠数量
            item.AddToStack();
        }
        else
        {
            // 如果物品不存在，则初始化物品实例并添加到列表和字典中
            item = itemData.Initialize(this);
            _items.Add(item);
            _itemDictionary.Add(itemData, item);
        }
    }

    /// <summary>
    /// 从背包中移除指定物品，如果物品数量大于 1 则减少堆叠数量，否则完全移除该物品。
    /// </summary>
    /// <param name="itemData">要移除的物品数据对象。</param>
    /// <returns>如果成功移除物品返回 true，否则返回 false。</returns>
    public bool RemoveItem(ItemData itemData)
    {
        if (_itemDictionary.TryGetValue(itemData, out Item item))
        {
            if (item.Quantity > 1)
            {
                // 如果堆叠数量大于 1，则减少堆叠数量
                item.RemoveFromStack();
                return true;
            }
            else
            {
                // 如果堆叠数量为 1，则完全移除该物品
                _itemDictionary.Remove(itemData);
                _items.Remove(item);
                return true;
            }
        }
        return false;
    }
}
