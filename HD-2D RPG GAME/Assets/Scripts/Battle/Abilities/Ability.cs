using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability 类为所有技能提供了抽象基类，封装了技能名称、倍率、法力消耗、来源对象以及可能附带的状态效果等数据和初始化逻辑。
/// </summary>
public abstract class Ability
{
    /// <summary>
    /// 技能名称，从 AbilityData 获取。
    /// </summary>
    public string Name;

    /// <summary>
    /// 技能倍率，用于计算技能伤害或效果，值由 AbilityData 提供。
    /// </summary>
    public float Multiplier { get; }

    /// <summary>
    /// 技能法力消耗，值由 AbilityData 提供。
    /// </summary>
    public float ManaCost { get; }

    /// <summary>
    /// 技能来源的 GameObject（例如施法者或技能预制体）。
    /// </summary>
    public GameObject Source { get; }

    /// <summary>
    /// 内部保存的技能数据引用，用于初始化技能各项参数。
    /// </summary>
    protected readonly AbilityData abilityData;

    /// <summary>
    /// 附带的状态效果数据列表，用于初始化技能施加的状态效果。
    /// </summary>
    protected readonly List<StatusEffectData> statusEffectsDataList;

    /// <summary>
    /// 由状态效果数据初始化后的状态效果列表，技能使用时可能对目标施加这些状态。
    /// </summary>
    protected readonly List<StatusEffect> statusEffects;

    /// <summary>
    /// 构造函数，使用指定的 AbilityData、来源 GameObject 和状态效果数据列表初始化技能。
    /// 同时根据状态效果数据列表初始化状态效果。
    /// </summary>
    /// <param name="data">技能数据引用，包含技能各项基础参数</param>
    /// <param name="source">技能的来源对象</param>
    /// <param name="statusEffectDataList">技能附带的状态效果数据列表</param>
    public Ability(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList)
    {
        abilityData = data;
        ManaCost = data.ManaCost;
        Name = data.AbilityName;
        Multiplier = data.Multiplier;
        statusEffectsDataList = statusEffectDataList;
        statusEffects = new List<StatusEffect>();
        Source = source;
        InitializeStatusEffect();
    }

    /// <summary>
    /// 重载构造函数，当技能无附带状态效果数据时调用，默认传入 null。
    /// </summary>
    /// <param name="data">技能数据引用</param>
    /// <param name="source">技能来源对象</param>
    public Ability(AbilityData data, GameObject source) : this(data, source, null) { }

    /// <summary>
    /// 根据状态效果数据列表初始化状态效果列表。
    /// 若状态效果数据列表不为 null，则遍历列表并调用每个状态效果数据的 Initialize 方法生成对应状态效果实例。
    /// </summary>
    protected void InitializeStatusEffect()
    {
        if (statusEffectsDataList != null)
        {
            foreach (StatusEffectData data in statusEffectsDataList)
            {
                StatusEffect statusEffect = data.Initialize();
                statusEffects.Add(statusEffect);
            }
        }
    }
}
