using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态效果数据的抽象基类，所有具体状态效果的数据类需继承此类。
/// 该类用于描述状态效果的基本配置，如持续时间、是否可叠加等。
/// </summary>
public abstract class StatusEffectData : ScriptableObject
{
    /// <summary>
    /// 状态效果的名称，用于在界面或日志中显示。
    /// </summary>
    public string Name;

    /// <summary>
    /// 状态效果的持续回合数，表示效果将持续多少轮战斗。
    /// </summary>
    public int turnDuration;

    /// <summary>
    /// 表示该状态效果是否支持效果叠加（如攻击力+10 可以叠加为+20）。
    /// </summary>
    public bool isEffectStackable;

    /// <summary>
    /// 表示该状态效果是否支持持续时间叠加（如再加一次延长持续时间）。
    /// </summary>
    public bool isDurationStackable;

    /// <summary>
    /// 初始化具体的状态效果逻辑类，需在子类中实现。
    /// </summary>
    /// <returns>返回一个对应的 StatusEffect 实例</returns>
    public abstract StatusEffect Initialize();
}
