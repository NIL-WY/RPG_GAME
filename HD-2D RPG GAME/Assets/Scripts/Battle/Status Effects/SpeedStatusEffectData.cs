using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 创建一个可在 Unity 编辑器中通过菜单创建的速度状态效果数据 ScriptableObject。
/// 用于定义影响速度属性的状态效果的相关数据和初始化逻辑。
/// </summary>
[CreateAssetMenu(fileName = "New Speed Status Effect", menuName = "Status Effect/Speed Status Effect")]
public class SpeedStatusEffectData : StatusEffectData
{
    /// <summary>
    /// 修改速度的数值。例如 +10 表示加速，-5 表示减速。
    /// </summary>
    public float ModifierValue = 0.0f;

    /// <summary>
    /// 要修改的属性，这里默认是速度（Speed），不可更改。
    /// </summary>
    private StatType StatToModify = StatType.Speed;

    /// <summary>
    /// 修改器的类型，表示是加成（乘法、加法等）还是其他类型。
    /// </summary>
    public StatModifierType ModifierType;

    /// <summary>
    /// 表示具体的速度属性修改器，实际用于改变目标单位的属性。
    /// </summary>
    public StatModifier speedModifier;

    /// <summary>
    /// 初始化该状态效果数据对应的具体状态效果逻辑类。
    /// </summary>
    /// <returns>返回一个新的 SpeedStatusEffect 实例</returns>
    public override StatusEffect Initialize()
    {
        return new SpeedStatusEffect(this);
    }

    /// <summary>
    /// 在 ScriptableObject 被启用（如加载资源或赋值时）自动调用，
    /// 用于初始化速度属性修改器。
    /// </summary>
    private void OnEnable()
    {
        speedModifier = new StatModifier(ModifierValue, StatToModify, ModifierType, this);
    }
}
