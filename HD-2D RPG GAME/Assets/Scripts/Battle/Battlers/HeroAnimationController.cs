using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 英雄动画控制器 - 管理英雄单位的所有动画状态和过渡
/// [组件需求] 需要与Hero组件挂载在同一游戏对象上
/// </summary>
[RequireComponent(typeof(Hero))]
public class HeroAnimationController : MonoBehaviour
{
    // 动画器组件引用
    private Animator _animator;

    /// <summary>
    /// 公开访问的动画器组件（只读）
    /// </summary>
    public Animator Animator
    {
        get { return _animator; }
        private set { _animator = value; }
    }

    /// <summary>
    /// 初始化时获取动画器组件
    /// </summary>
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // 动画状态控制方法组 ======================================================

    /// <summary>
    /// 切换至待机状态（非战斗姿态）
    /// </summary>
    public void PlayIdle()
    {
        _animator.SetBool("isReady", false);
    }

    /// <summary>
    /// 播放使用道具动画（触发型动画）
    /// </summary>
    public void PlayUseItem()
    {
        _animator.SetTrigger("ItemTrigger");
    }

    /// <summary>
    /// 播放普通攻击动画（触发型动画）
    /// </summary>
    public void PlayAttack()
    {
        _animator.SetTrigger("AttackTrigger");
    }

    /// <summary>
    /// 播放特殊攻击动画（触发型动画）
    /// </summary>
    public void PlaySpecialAttack()
    {
        _animator.SetTrigger("SpecialTrigger");
    }

    /// <summary>
    /// 播放受击反应动画（触发型动画）
    /// </summary>
    public void PlayGetDamaged()
    {
        _animator.SetTrigger("HurtTrigger");
    }

    /// <summary>
    /// 播放增益效果动画（触发型动画）
    /// </summary>
    public void PlayBuff()
    {
        _animator.SetTrigger("BuffTrigger");
    }

    /// <summary>
    /// 进入防御姿态（持续状态动画）
    /// </summary>
    public void PlayDefend()
    {
        _animator.SetBool("isDefending", true);
    }

    /// <summary>
    /// 退出防御姿态（持续状态动画）
    /// </summary>
    public void StopDefend()
    {
        _animator.SetBool("isDefending", false);
    }

    /// <summary>
    /// 切换至战斗准备姿态（持续状态动画）
    /// </summary>
    public void PlayReady()
    {
        _animator.SetBool("isReady", true);
    }

    /// <summary>
    /// 播放前进移动动画（触发型动画）
    /// </summary>
    public void PlayMoveForward()
    {
        _animator.SetTrigger("MoveForward");
    }

    /// <summary>
    /// 播放后退移动动画（触发型动画）
    /// </summary>
    public void PlayMoveBackward()
    {
        _animator.SetTrigger("MoveBackward");
    }

    /// <summary>
    /// 播放闪避成功动画（触发型动画）
    /// </summary>
    public void PlayEvade()
    {
        _animator.SetTrigger("EvadeTrigger");
    }

    /// <summary>
    /// 播放胜利动画（待实现）
    /// </summary>
    public void PlayWin()
    {
        // TODO: 添加胜利动画逻辑
        // 建议使用动画触发器："WinTrigger"
    }
    //死亡动画
    public void PlayDeath()
    {
        _animator.SetTrigger("DieTrigger");
        _animator.SetBool("isDead", true); // 添加永久死亡状态
    }
}

/* 动画状态机配置指南：
1. 参数列表需求：
   - Bool参数：
     * isReady（战斗准备状态）
     * isDefending（防御状态）
   - Trigger参数：
     * ItemTrigger（使用道具）
     * AttackTrigger（普通攻击）
     * SpecialTrigger（特殊攻击）
     * HurtTrigger（受击反应）
     * BuffTrigger（增益效果）
     * MoveForward（前进移动）
     * MoveBackward（后退移动）
     * EvadeTrigger（闪避动作）

2. 状态过渡建议：
   - 所有Trigger类动画应设置退出时间（Exit Time）
   - 防御状态(isDefending)应有对应循环动画
   - 移动动画建议使用混合树处理不同方向

3. 动画层级配置：
   - 建议使用动画层处理上半身/下半身分离
   - 例如：上层处理攻击动作，下层处理移动

注意事项：
1. 动画触发器名称必须与Animator Controller中完全一致
2. 避免在同一个动画周期内重复触发相同Trigger
3. 持续状态动画（Bool参数）需要明确的进入/退出逻辑
4. 动画事件需要与Hero类中的方法正确绑定
*/