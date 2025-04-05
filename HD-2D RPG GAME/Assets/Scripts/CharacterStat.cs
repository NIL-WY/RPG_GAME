using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// 定义角色属性类型的枚举，如最大生命值、最大法力值、物理攻击、物理防御、速度、暴击和闪避。
/// </summary>
public enum StatType
{
    /// <summary>最大生命值</summary>
    MaxHealth,
    /// <summary>最大法力值</summary>
    MaxMana,
    /// <summary>物理攻击</summary>
    PhysicalAttack,
    /// <summary>物理防御</summary>
    PhysicalDefense,
    /// <summary>速度</summary>
    Speed,
    /// <summary>暴击率</summary>
    Critical,
    /// <summary>闪避率</summary>
    Evasion
}

/// <summary>
/// 表示角色属性的类，封装了基础值、属性修正器以及最终计算后的属性值。
/// </summary>
[System.Serializable]
public class CharacterStat
{
    /// <summary>
    /// 属性的基础数值。
    /// </summary>
    public float BaseValue;

    /// <summary>
    /// 属性的类型（只读），用于标识该属性是生命值、攻击、防御等哪一项。
    /// </summary>
    public readonly StatType Type;

    /// <summary>
    /// 上一次记录的基础值，用于判断基础值是否发生改变。
    /// 初始值设置为 float.MinValue，确保第一次访问时能够重新计算。
    /// </summary>
    private float _lastBaseValue = float.MinValue;

    /// <summary>
    /// 标识属性是否被标记为“isDirty”，即是否需要重新计算最终值。
    /// </summary>
    private bool _isDirty = true;

    /// <summary>
    /// 内部列表，存储所有影响该属性的修正器。
    /// </summary>
    private readonly List<StatModifier> _statModifiers;

    /// <summary>
    /// 属性修正器的只读集合，供外部只读访问。
    /// </summary>
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    /// <summary>
    /// 缓存的属性最终值，通过计算基础值和所有修正器获得。
    /// </summary>
    [SerializeField]
    private float _value;

    /// <summary>
    /// 获取属性的最终值，如果基础值改变或者标记为“isDirty”，则重新计算最终值。
    /// </summary>
    public float Value
    {
        get
        {
            if (_isDirty || _lastBaseValue != BaseValue)
            {
                _value = CalculateFinalValue();
            }
            return _value;
        }
    }

    /// <summary>
    /// 无参构造函数，初始化修正器列表和只读集合。
    /// </summary>
    public CharacterStat()
    {
        _statModifiers = new List<StatModifier>();
        StatModifiers = _statModifiers.AsReadOnly();
    }

    /// <summary>
    /// 构造函数，根据给定基础值和属性类型初始化 CharacterStat 对象。
    /// </summary>
    /// <param name="baseValue">属性的基础数值。</param>
    /// <param name="type">属性类型。</param>
    public CharacterStat(float baseValue, StatType type) : this()
    {
        this.BaseValue = baseValue;
        this.Type = type;
    }

    /// <summary>
    /// 构造函数，仅根据属性类型初始化 CharacterStat 对象，基础值默认为 0。
    /// </summary>
    /// <param name="type">属性类型。</param>
    public CharacterStat(StatType type) : this(0, type) { }

    /// <summary>
    /// 添加一个属性修正器，并将属性标记为“脏”，以便下次重新计算最终值。
    /// 同时按修正器的顺序进行排序。
    /// </summary>
    /// <param name="mod">要添加的属性修正器对象。</param>
    public void AddModifier(StatModifier mod)
    {
        _isDirty = true;
        _statModifiers.Add(mod);
        _statModifiers.Sort(CompareModifierOrder);
    }

    /// <summary>
    /// 移除指定的属性修正器，如果成功移除则标记属性为“脏”并返回 true，否则返回 false。
    /// </summary>
    /// <param name="mod">要移除的属性修正器对象。</param>
    /// <returns>如果修正器成功移除则返回 true，否则返回 false。</returns>
    public bool RemoveModifier(StatModifier mod)
    {
        if (_statModifiers.Remove(mod))
        {
            _isDirty = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除所有来源为指定对象的属性修正器，如果至少移除一个则返回 true，否则返回 false。
    /// </summary>
    /// <param name="source">修正器来源对象，用于识别需要移除的修正器。</param>
    /// <returns>如果至少移除一个修正器则返回 true，否则返回 false。</returns>
    public bool RemoveAllModifierFromSource(object source)
    {
        bool didRemove = false;
        for (int i = _statModifiers.Count - 1; i >= 0; i--)
        {
            if (_statModifiers[i].Source == source)
            {
                _isDirty = true;
                didRemove = true;
                _statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    /// <summary>
    /// 比较两个属性修正器的顺序，用于对修正器列表进行排序。
    /// </summary>
    /// <param name="a">第一个属性修正器。</param>
    /// <param name="b">第二个属性修正器。</param>
    /// <returns>
    /// 如果 a 的顺序小于 b 返回 -1，如果大于则返回 1，否则返回 0。
    /// </returns>
    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
        {
            return -1;
        }
        else if (a.Order > b.Order)
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// 根据基础值和所有修正器计算属性的最终数值，修正器包括平坦加值、百分比加值和百分比乘值。
    /// 在计算过程中，平坦加值直接累加，百分比加值累积后一次性计算，百分比乘值逐个乘以当前值。
    /// 最后结果会被四舍五入到小数点后四位。
    /// </summary>
    /// <returns>计算后的属性最终数值。</returns>
    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;
        for (int i = 0; i < _statModifiers.Count; i++)
        {
            if (_statModifiers[i].Type == StatModifierType.Flat)
            {
                finalValue += _statModifiers[i].Value;
            }
            else if (_statModifiers[i].Type == StatModifierType.PercentAdd)
            {
                sumPercentAdd += _statModifiers[i].Value;
                if (_statModifiers.Count - 1 < i + 1 || _statModifiers[i + 1].Type != StatModifierType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (_statModifiers[i].Type == StatModifierType.PercentMultiply)
            {
                finalValue *= 1 + _statModifiers[i].Value;
            }
        }
        _isDirty = false;
        return (float)Math.Round(finalValue, 4);
    }
}
