using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 支援型技能数据配置类（ScriptableObject）
/// 继承自基础技能数据类，用于创建具体的支援类技能配置
/// </summary>
/// <remarks>
/// 在Unity资源菜单中创建路径：
/// Assets > Create > Abilities > Support Ability
/// </remarks>
[CreateAssetMenu(
    fileName = "New Support Ability",
    menuName = "Abilities/Support Ability",
    order = 1)]
public class SupportAbilityData : AbilityData
{
    /// <summary>
    /// 触发支援技能的效果表现
    /// </summary>
    /// <param name="heroUser">施放技能的英雄单位</param>
    /// <remarks>
    /// 执行逻辑：
    /// 1. 在控制台输出技能使用日志
    /// 2. 在施法者位置生成使用者粒子特效
    /// 3. 在施法者位置生成目标粒子特效（当前配置可能有误）
    /// </remarks>
    /// <example>
    /// 示例调用：
    /// <code>
    /// supportData.TriggerEffect(currentHero);
    /// </code>
    /// </example>
    public void TriggerEffect(Hero heroUser)
    {
        // 输出技能使用调试信息（包含施法者名称和技能名称）
        Debug.Log(heroUser.Name + " used " + AbilityName);

        // 生成使用者粒子特效（如果已配置预制体）
        if (targetParticlePrefab != null)
        {
            // 注意：此处参数名可能为配置错误，targetParticlePrefab应用于目标位置
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }

        // 生成目标粒子特效（当前逻辑与上方重复，可能需要单独配置目标特效预制体）
        if (userParticlePrefab != null)
        {
            Instantiate(userParticlePrefab, heroUser.transform.position, userParticlePrefab.transform.rotation);
        }
    }

    /// <summary>
    /// 初始化技能实例的工厂方法
    /// </summary>
    /// <param name="source">技能来源的游戏对象</param>
    /// <returns>初始化完成的支援技能实例</returns>
    /// <seealso cref="SupportAbility"/>
    public override Ability Initialize(GameObject source)
    {
        // 创建支援技能实例并注入依赖：
        // - 当前数据对象（this）
        // - 技能来源对象
        // - 状态效果数据列表
        return new SupportAbility(this, source, statusEffectDataList);
    }
}

/* 注意事项（代码未修改但需注意）：
1. 粒子特效参数疑似配置错误：
   - targetParticlePrefab 和 userParticlePrefab 被重复使用
   - 建议：targetParticlePrefab 应用于目标位置，userParticlePrefab 用于施法者位置

2. 缺乏效果实现逻辑：
   - 当前仅实现特效生成，未包含实际治疗效果/属性加成等逻辑
   - 建议在派生类或配套系统中实现具体效果

3. 缺少空引用检查：
   - heroUser参数未进行null检查，可能引发空引用异常
   - 建议添加防御性编程检查

4. 特效旋转方向固定：
   - 使用预制体默认旋转角度，未考虑场景实际方向
   - 建议改为 Quaternion.identity 或根据场景需求调整
*/