using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

/// <summary>
/// 管理游戏中不同摄像机之间的切换逻辑，主要处理战斗场景摄像机与聚焦摄像机之间的切换
/// </summary>
public class CameraManager : MonoBehaviour
{
    // 战斗场景使用的虚拟摄像机
    [SerializeField] CinemachineVirtualCamera _battleSceneCamera;
    // 用于聚焦单个英雄的虚拟摄像机
    [SerializeField] CinemachineVirtualCamera _focusHeroCamera;

    [Header("Focus Group On Targeting Settings")]
    [Tooltip("目标单位在焦点组中的权重值")]
    [SerializeField] float _targetFocusWeight = 1.0f;
    [Tooltip("其他单位在焦点组中的权重值")]
    [SerializeField] float _otherFocusWeight = 1.5f;

    // 战斗场景摄像机使用的目标组（包含所有战斗单位）
    [SerializeField] private CinemachineTargetGroup _battleSceneTargetGroup;
    // 聚焦模式使用的目标组（临时存放需要聚焦的单位）
    [SerializeField] private CinemachineTargetGroup _focusTargetGroup;

    private bool _focusedOnGroup; // 标记当前是否处于群体聚焦模式
    private bool _isInitialized;  // 标记是否已完成初始化

    /// <summary>
    /// 当组件启用时，如果已初始化则重新订阅事件
    /// </summary>
    private void OnEnable()
    {
        if (_isInitialized)
        {
            SubscribeToEvents();
        }
    }

    /// <summary>
    /// 当组件禁用时取消所有事件订阅
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// 初始化摄像机设置，订阅事件，设置初始摄像机状态
    /// </summary>
    private void Start()
    {
        _focusHeroCamera.gameObject.SetActive(false);  // 初始隐藏聚焦摄像机
        SubscribeToEvents();
        InitializeBattleSceneCam();
        _isInitialized = true;
    }

    /// <summary>
    /// 为所有英雄单位订阅相关事件
    /// </summary>
    private void SubscribeToEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            // 当英雄选择自己作为目标时触发
            hero.OnTargetSelf += FocusOnHero;
            // 当英雄选择其他目标时触发
            hero.OnTargetOther += FocusOnGroup;
            // 当英雄结束回合时触发
            hero.OnEndTurn += ReturnToSceneCamera;
        }
    }

    /// <summary>
    /// 取消所有英雄单位的事件订阅
    /// </summary>
    private void UnsubscribeToEvents()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.OnTargetSelf -= FocusOnHero;
            hero.OnTargetOther -= FocusOnGroup;
            hero.OnEndTurn -= ReturnToSceneCamera;
        }
    }

    /// <summary>
    /// 初始化战斗场景摄像机目标组
    /// 将所有英雄和敌人添加到目标组中
    /// </summary>
    private void InitializeBattleSceneCam()
    {
        if (_battleSceneTargetGroup == null)
        {
            Debug.LogWarning("BattleScene CM Target Group not assigned!");
            return;
        }

        // 添加所有英雄到目标组
        foreach (var hero in BattleManager.Instance.heroes)
        {
            _battleSceneTargetGroup.AddMember(hero.transform, 1, 0);
        }

        // 添加所有敌人到目标组
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            _battleSceneTargetGroup.AddMember(enemy.transform, 1, 0);
        }

        // 设置战斗场景摄像机的观察目标
        _battleSceneCamera.LookAt = _battleSceneTargetGroup.transform;
    }

    /// <summary>
    /// 聚焦到单个英雄单位
    /// </summary>
    /// <param name="battler">需要聚焦的战斗单位</param>
    private void FocusOnHero(Battler battler)
    {
        _focusHeroCamera.gameObject.SetActive(true);
        _focusHeroCamera.LookAt = battler.transform;
    }

    /// <summary>
    /// 聚焦到多个单位组成的群体
    /// </summary>
    /// <param name="battler">发起动作的单位</param>
    /// <param name="other">目标单位</param>
    private void FocusOnGroup(Battler battler, Battler other)
    {
        if (_focusTargetGroup == null)
        {
            Debug.LogWarning("Focus CM Target Group not assigned!");
            return;
        }

        // 添加目标单位到焦点组并设置权重
        _focusTargetGroup.AddMember(battler.transform, _targetFocusWeight, 0);
        _focusTargetGroup.AddMember(other.transform, _otherFocusWeight, 0);

        _focusHeroCamera.gameObject.SetActive(true);
        _focusHeroCamera.LookAt = _focusTargetGroup.transform;
        _focusedOnGroup = true;  // 标记当前处于群体聚焦模式
    }

    /// <summary>
    /// 返回默认的战斗场景摄像机
    /// </summary>
    private void ReturnToSceneCamera()
    {
        if (_focusedOnGroup)
        {
            // 清空焦点组中的所有目标
            foreach (var target in _focusTargetGroup.m_Targets)
            {
                _focusTargetGroup.RemoveMember(target.target);
            }
            _focusedOnGroup = false;
        }

        _focusHeroCamera.LookAt = null;
        _focusHeroCamera.gameObject.SetActive(false);
    }
}