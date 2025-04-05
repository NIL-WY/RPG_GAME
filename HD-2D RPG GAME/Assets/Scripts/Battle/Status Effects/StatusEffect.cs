using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态效果基类，用于定义所有具体状态效果（如中毒、加速等）的公共行为。
/// </summary>
public abstract class StatusEffect
{
    // 状态效果的数据，包含效果类型、持续时间等
    public StatusEffectData Data;

    // 当前剩余的持续回合数
    private int _currentTurnDuration;

    // 标志位，表示状态是否已结束
    public bool isFinished = false;

    // 效果的叠加次数
    protected int _effectStacks;

    /// <summary>
    /// 构造函数，初始化状态效果数据。
    /// </summary>
    /// <param name="data">状态效果数据</param>
    public StatusEffect(StatusEffectData data)
    {
        Data = data;
    }

    /// <summary>
    /// 每经过一回合时调用此方法，减少持续时间并检查是否应结束状态。
    /// </summary>
    public void Tick()
    {
        if (_currentTurnDuration > 0)
        {
            _currentTurnDuration--;
        }
        else if (_currentTurnDuration <= 0)
        {
            isFinished = true;
            End();
        }
    }

    /// <summary>
    /// 应用状态效果到指定战斗单位。
    /// 如果允许效果叠加或尚未生效，将重新应用并计数。
    /// 如果允许持续时间叠加或尚未生效，将叠加持续时间。
    /// </summary>
    /// <param name="battler">目标单位</param>
    public void Start(Battler battler)
    {
        isFinished = false;

        if (Data.isEffectStackable || _currentTurnDuration == 0)
        {
            ApplyEffect(battler);
            _effectStacks++;
        }

        if (Data.isDurationStackable || _currentTurnDuration == 0)
        {
            _currentTurnDuration += Data.turnDuration;
        }
    }

    /// <summary>
    /// 派生类必须实现的具体应用效果逻辑，例如加攻击力、降低速度等。
    /// </summary>
    protected abstract void ApplyEffect(Battler battle);

    /// <summary>
    /// 状态效果结束时调用，可用于移除加成/减益等。
    /// </summary>
    public virtual void End()
    {
        _effectStacks = 0;
    }
}
