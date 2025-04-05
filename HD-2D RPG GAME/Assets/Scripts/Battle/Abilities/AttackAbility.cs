using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AttackAbility 类是用于实现攻击技能的具体实现，
/// 继承自 Ability 基类，包含触发攻击技能效果和计算伤害的逻辑。
/// </summary>
public class AttackAbility : Ability
{
    /// <summary>
    /// 构造函数，根据传入的技能数据、来源对象和状态效果数据列表初始化 AttackAbility 实例。
    /// </summary>
    /// <param name="data">技能数据，必须为 AttackAbilityData 类型</param>
    /// <param name="source">技能来源的 GameObject，例如施法者</param>
    /// <param name="statusEffectDataList">附加的状态效果数据列表</param>
    public AttackAbility(AbilityData data, GameObject source, List<StatusEffectData> statusEffectDataList)
        : base(data, source, statusEffectDataList) { }

    /// <summary>
    /// 触发攻击技能，计算并返回造成的原始伤害，同时对目标执行技能特效并为使用者附加状态效果。
    /// </summary>
    /// <param name="heroUser">使用该技能的英雄角色</param>
    /// <param name="enemyTarget">技能的目标敌人</param>
    /// <param name="rawDamage">
    /// 输出参数，表示英雄基于技能倍率计算得到的原始伤害数值
    /// </param>
    public virtual void Trigger(Hero heroUser, Enemy enemyTarget, out float rawDamage)
    {
        // 将 abilityData 转换为 AttackAbilityData 类型以调用特定的触发效果
        AttackAbilityData data = abilityData as AttackAbilityData;
        // 触发攻击技能的特效（例如动画、粒子效果等）
        data.TriggerEffect(heroUser, enemyTarget);
        // 根据英雄的属性和技能倍率计算原始伤害
        rawDamage = heroUser.CalculateDamage(Multiplier);
        // 如果技能附带的状态效果不为空，为英雄添加所有状态效果
        if (statusEffects == null) return;
        foreach (StatusEffect status in statusEffects)
        {
            heroUser.AddStatusEffect(status);
        }
    }
}
