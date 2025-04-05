using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SupportAbility 类用于实现辅助型技能，
/// 该技能主要对自身生效，可为英雄附加状态效果或执行其他辅助行为。
/// 此类继承自 Ability 并实现 ISelfTargetable 接口。
/// </summary>
public class SupportAbility : Ability, ISelfTargetable
{
    /// <summary>
    /// 构造函数，根据传入的技能数据、来源对象以及状态效果数据列表初始化 SupportAbility 实例。
    /// </summary>
    /// <param name="data">技能数据，通常为 SupportAbilityData 类型</param>
    /// <param name="source">技能来源的 GameObject，例如施法者</param>
    /// <param name="statusEffectDataList">附加的状态效果数据列表</param>
    public SupportAbility(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList)
        : base(data, source, statusEffectDataList) { }

    /// <summary>
    /// 触发辅助技能，对指定的英雄施放技能效果。
    /// 调用技能数据中的 TriggerEffect 方法执行辅助效果，
    /// 并将所有附加的状态效果添加到英雄身上。
    /// </summary>
    /// <param name="hero">技能作用目标，通常为施法者自身</param>
    public void Trigger(Hero hero)
    {
        // 将 abilityData 转换为 SupportAbilityData 以调用特定的触发效果方法
        SupportAbilityData data = abilityData as SupportAbilityData;
        data.TriggerEffect(hero);

        // 为英雄添加所有附带的状态效果
        foreach (StatusEffect status in statusEffects)
        {
            hero.AddStatusEffect(status);
        }
    }
}
