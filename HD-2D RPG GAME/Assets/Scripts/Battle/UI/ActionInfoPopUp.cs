using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 弹出提示的类型，表示是伤害、法力值回复还是生命值回复。
/// </summary>
public enum PopUpType
{
    Damage,
    Mana,
    Health
}

/// <summary>
/// 在角色头顶弹出显示战斗信息（如伤害数值、回血、回蓝）的文本组件。
/// 具有自动淡出和关闭功能。
/// </summary>
public class ActionInfoPopUp : MonoBehaviour
{
    /// <summary>
    /// 显示文本的 TextMeshPro 组件。
    /// </summary>
    private TextMeshPro _text;


    public Action OnFadeComplete; // 新增：淡出完成后通知池
    /// <summary>
    /// 弹出信息在屏幕中存在的时间（秒）。
    /// </summary>
    [SerializeField] private float _lifeTimeInSeconds = 1f;

    /// <summary>
    /// 文本淡出的速度，数值越大淡出越快。
    /// </summary>
    [SerializeField] private float _fadeSpeed = 0.0025f;

    /// <summary>
    /// 显示伤害时使用的颜色。
    /// </summary>
    [SerializeField] private Color _damageColor;

    /// <summary>
    /// 显示生命恢复时使用的颜色。
    /// </summary>
    [SerializeField] private Color _healthRecoverColor;

    /// <summary>
    /// 显示法力恢复时使用的颜色。
    /// </summary>
    [SerializeField] private Color _manaRecoverColor;

    /// <summary>
    /// 在物体创建或启用时自动获取子物体上的文本组件。
    /// </summary>
    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshPro>();
    }

    /// <summary>
    /// 激活文本提示，根据类型设置不同颜色并显示对应文本内容。
    /// 然后启动淡出协程。
    /// </summary>
    /// <param name="text">要显示的内容</param>
    /// <param name="type">提示的类型（伤害、回血、回蓝）</param>
    public void Activate(string text, PopUpType type)
    {
        StopAllCoroutines(); // 防止旧的 FadeOut 还在跑
        _text.SetText(text);

        switch (type)
        {
            case PopUpType.Damage:
                _text.color = _damageColor;
                break;
            case PopUpType.Health:
                _text.color = _healthRecoverColor;
                break;
            case PopUpType.Mana:
                _text.color = _manaRecoverColor;
                break;
        }

        Color color = _text.color;
        color.a = 1f; // 重置透明度
        _text.color = color;

        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// 控制文本逐渐淡出直至不可见，并自动关闭该游戏物体。
    /// </summary>
    private IEnumerator FadeOut()
    {
        var color = _text.color;

        // 等待显示时间到达后开始淡出
        yield return new WaitForSeconds(_lifeTimeInSeconds);

        // 逐步降低透明度直到完全消失
        for (float alpha = 1f; alpha >= 0; alpha -= _fadeSpeed)
        {
            color.a = alpha;
            _text.color = color;
            yield return null;
        }

        // 完全透明后禁用该物体
        gameObject.SetActive(false);
        OnFadeComplete?.Invoke();
    }
}
