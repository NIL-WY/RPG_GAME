using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AbilityData 是所有技能数据的抽象基类，继承自 ScriptableObject，
/// 用于存储技能的基础属性和特效预制体，并提供初始化技能实例的方法。
/// </summary>
public abstract class AbilityData : ScriptableObject
{
    /// <summary>
    /// 技能名称，默认为 "New Ability"。
    /// </summary>
    public string AbilityName = "New Ability";

    /// <summary>
    /// 技能倍率，用于计算技能效果（例如伤害乘数），默认为 1。
    /// </summary>
    public int Multiplier = 1;

    /// <summary>
    /// 技能法力消耗，施放该技能时消耗的法力值，默认为 1。
    /// </summary>
    public int ManaCost = 1;

    /// <summary>
    /// 用户端粒子特效预制体，在施放技能时由施法者播放的特效。
    /// </summary>
    public GameObject userParticlePrefab;

    /// <summary>
    /// 目标端粒子特效预制体，在施放技能时由目标播放的特效。
    /// </summary>
    public GameObject targetParticlePrefab;

    /// <summary>
    /// 附带的状态效果数据列表，用于技能施放时对目标附加额外状态效果。
    /// </summary>
    public List<StatusEffectData> statusEffectDataList;

    /// <summary>
    /// 抽象方法，根据当前 AbilityData 数据初始化并返回一个具体的技能实例。
    /// 子类需要实现此方法以创建特定技能逻辑的 Ability 对象。
    /// </summary>
    /// <param name="source">技能来源的 GameObject，例如施法者或技能预制体。</param>
    /// <returns>初始化后的技能实例。</returns>
    public abstract Ability Initialize(GameObject source);
}
