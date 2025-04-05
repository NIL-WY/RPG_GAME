using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AttackAbilityData 类继承自 AbilityData，用于定义攻击技能的数据配置，
/// 包括技能名称、倍率、法力消耗、特效预制体以及附加的状态效果数据列表。
/// 此类通过 [CreateAssetMenu] 属性使得在 Unity 编辑器中可以创建新的攻击技能数据资源。
/// </summary>
[CreateAssetMenu(fileName = "New Attack Ability", menuName = "Abilities/Attack Ability", order = 1)]
public class AttackAbilityData : AbilityData
{
    /// <summary>
    /// 触发攻击技能的效果方法，在技能施放时调用。
    /// 此方法会在控制台输出一条日志，并在目标与施法者位置生成相应的粒子特效（如果已配置）。
    /// </summary>
    /// <param name="heroUser">使用该技能的英雄角色</param>
    /// <param name="enemyTarget">技能的目标敌人</param>
    public void TriggerEffect(Hero heroUser, Enemy enemyTarget)
    {
        Debug.Log(heroUser.Name + " used " + AbilityName + " on " + enemyTarget.gameObject.name);
        // 如果配置了目标特效预制体，则在目标位置实例化该特效
        if (targetParticlePrefab != null)
        {
            Instantiate(targetParticlePrefab, enemyTarget.transform.position, targetParticlePrefab.transform.rotation);
        }
        // 如果配置了用户特效预制体，则在施法者位置实例化该特效
        if (userParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }
    }

    /// <summary>
    /// 根据当前 AttackAbilityData 数据初始化一个 AttackAbility 实例。
    /// 此方法用于创建具体的攻击技能对象，并传入来源对象和状态效果数据列表。
    /// </summary>
    /// <param name="source">技能来源的 GameObject，例如施法者</param>
    /// <returns>初始化后的 AttackAbility 实例</returns>
    public override Ability Initialize(GameObject source)
    {
        return new AttackAbility(this, source, statusEffectDataList);
    }
}
