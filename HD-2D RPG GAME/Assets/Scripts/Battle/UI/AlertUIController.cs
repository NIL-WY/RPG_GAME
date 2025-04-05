using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 战斗中显示角色动作提示信息的 UI 控制器。
/// 用于在战斗中展示当前角色正在执行的行为（如攻击、技能等）。
/// </summary>
public class AlertUIController : MonoBehaviour
{
    /// <summary>
    /// 用于显示提示信息的 UI 面板对象。
    /// </summary>
    [SerializeField] GameObject Alertbox;

    /// <summary>
    /// 用于显示文本内容的 TextMeshProUGUI 组件。
    /// </summary>
    private TextMeshProUGUI _text;

    /// <summary>
    /// 在脚本激活时获取子物体中的文本组件。
    /// </summary>
    private void Awake()
    {
        _text = Alertbox.GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// 启动时关闭提示框，并订阅战斗角色的事件。
    /// </summary>
    private void Start()
    {
        Alertbox.SetActive(false);
        SubscribeToEvents();
    }

    /// <summary>
    /// 订阅所有战斗角色（英雄与敌人）的事件：
    /// DisplayAlert 用于展示信息，OnEndTurn 用于结束提示。
    /// </summary>
    private void SubscribeToEvents()
    {
        if (BattleManager.Instance != null)
        {
            foreach (var hero in BattleManager.Instance.heroes)
            {
                hero.DisplayAlert += DisplayMessage;
                hero.OnEndTurn += EndAlert;
            }

            foreach (var enemy in BattleManager.Instance.enemies)
            {
                enemy.DisplayAlert += DisplayMessage;
                enemy.OnEndTurn += EndAlert;
            }
        }
    }

    /// <summary>
    /// 显示传入的提示信息并打开提示框。
    /// </summary>
    /// <param name="message">需要显示的消息字符串</param>
    public void DisplayMessage(string message)
    {
        Alertbox.SetActive(true);
        _text.SetText(message);
    }

    /// <summary>
    /// 关闭提示框，一般在角色回合结束后调用。
    /// </summary>
    public void EndAlert()
    {
        Alertbox.SetActive(false);
    }

    /// <summary>
    /// 脚本销毁前取消事件订阅，避免内存泄漏。
    /// </summary>
    private void OnDestroy()
    {
        foreach (var hero in BattleManager.Instance.heroes)
        {
            hero.DisplayAlert -= DisplayMessage;
            hero.OnEndTurn -= EndAlert;
        }

        foreach (var enemy in BattleManager.Instance.enemies)
        {
            enemy.DisplayAlert -= DisplayMessage;
            enemy.OnEndTurn -= EndAlert;
        }
    }
}
