using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 属性加成的类型枚举：
/// Flat 表示直接加数值，
/// PercentAdd 表示百分比相加（多个加一起再计算），
/// PercentMultiply 表示百分比相乘（每个单独计算）
/// </summary>
public enum StatModifierType
{
    Flat = 100,               // 固定数值加成
    PercentAdd = 200,         // 百分比叠加加成
    PercentMultiply = 300     // 百分比乘法加成
}

/// <summary>
/// 表示一个属性修正器，用于修饰角色属性（如攻击、防御等）
/// </summary>
public class StatModifier
{
    /// <summary>
    /// 修正器的应用顺序。数值越小越早应用。
    /// 一般使用 StatModifierType 枚举值作为默认顺序。
    /// </summary>
    public readonly int Order;

    /// <summary>
    /// 修正的值。具体数值或百分比，取决于 Type。
    /// </summary>
    public readonly float Value;

    /// <summary>
    /// 修正类型（加法、百分比加成、百分比乘法）
    /// </summary>
    public readonly StatModifierType Type;

    /// <summary>
    /// 被修饰的属性类型（如攻击力、速度等）
    /// </summary>
    public readonly StatType StatType;

    /// <summary>
    /// 修正来源，可用于标记该修正来自哪个技能、装备等，便于移除。
    /// </summary>
    public readonly object Source;

    /// <summary>
    /// 主构造函数：指定所有属性。
    /// </summary>
    /// <param name="value">修正数值</param>
    /// <param name="statType">对应的属性类型</param>
    /// <param name="type">修正类型</param>
    /// <param name="order">应用顺序</param>
    /// <param name="source">修正来源</param>
    public StatModifier(float value, StatType statType, StatModifierType type, int order, object source)
    {
        this.Value = value;
        this.Type = type;
        this.Order = order;
        this.Source = source;
        this.StatType = statType;
    }

    /// <summary>
    /// 构造函数（默认应用顺序，默认无来源）
    /// </summary>
    public StatModifier(float value, StatType statype, StatModifierType type)
        : this(value, statype, type, (int)type, null) { }

    /// <summary>
    /// 构造函数（指定顺序，默认无来源）
    /// </summary>
    public StatModifier(float value, StatType statype, StatModifierType type, int order)
        : this(value, statype, type, order, null) { }

    /// <summary>
    /// 构造函数（默认顺序，指定来源）
    /// </summary>
    public StatModifier(float value, StatType statype, StatModifierType type, object source)
        : this(value, statype, type, (int)type, source) { }
}
