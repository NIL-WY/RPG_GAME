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

    //死亡动画
    public void PlayDeath()
    {
        _animator.SetTrigger("DieTrigger");
        _animator.SetBool("isDead", true); // 添加永久死亡状态
    }
}
