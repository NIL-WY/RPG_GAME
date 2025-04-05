using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加速状态效果，实现来自 SpeedStatusEffectData 的速度加成逻辑。
/// </summary>
public class SpeedStatusEffect : StatusEffect
{
    // 加速状态数据（包含速度加成信息）
    SpeedStatusEffectData _speedStatusEffect;

    // 作用的战斗单位
    Battler _battler;

    /// <summary>
    /// 构造函数，从传入的数据中提取加速状态效果信息。
    /// </summary>
    /// <param name="data">速度状态效果数据</param>
    public SpeedStatusEffect(SpeedStatusEffectData data) : base(data)
    {
        _speedStatusEffect = Data as SpeedStatusEffectData;
    }

    /// <summary>
    /// 应用状态效果到目标单位，添加速度属性修改器。
    /// </summary>
    /// <param name="battler">目标单位</param>
    protected override void ApplyEffect(Battler battler)
    {
        _battler = battler;
        _battler.AddModifier(_speedStatusEffect.speedModifier);
    }

    /// <summary>
    /// 当状态效果结束时，移除来源为本效果的数据的所有速度修改器。
    /// </summary>
    public override void End()
    {
        _battler.RemoveAllModifierFromSource(_speedStatusEffect);
        base.End();
    }
}
