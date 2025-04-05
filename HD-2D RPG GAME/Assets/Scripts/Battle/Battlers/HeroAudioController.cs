using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 英雄音频控制器 - 管理英雄单位的所有语音和音效播放
/// [组件需求] 需要AudioSource组件处理音频播放
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class HeroAudioController : MonoBehaviour
{
    // 组件引用
    private AudioSource _audioSource;  // 音频播放组件

    // 音效配置列表（通过编辑器配置）
    [Header("攻击语音")]
    [SerializeField] List<AudioClip> _attackVoiceClips;

    [Header("特殊攻击语音")]
    [SerializeField] List<AudioClip> _specialAttackVoiceClips;

    [Header("受伤语音")]
    [SerializeField] List<AudioClip> _hurtVoiceClips;

    [Header("回合开始语音")]
    [SerializeField] List<AudioClip> _startTurnVoiceClips;

    [Header("闪避语音")]
    [SerializeField] List<AudioClip> _evadeVoiceClips;

    [Header("防御中语音")]
    [SerializeField] List<AudioClip> _guardVoiceClips;

    [Header("开始防御语音")]
    [SerializeField] List<AudioClip> _startGuardVoiceClips;

    [Header("自我增益语音")]
    [SerializeField] List<AudioClip> _selfBuffVoiceClips;

    [Header("使用道具语音")]
    [SerializeField] List<AudioClip> _itemUseVoiceClips;

    /// <summary>
    /// 初始化音频组件
    /// </summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // 音效播放方法组 ========================================================

    /// <summary>
    /// 播放随机普通攻击语音
    /// </summary>
    /// <remarks>
    /// 播放规则：
    /// 1. 立即停止当前播放的音频
    /// 2. 从配置列表中随机选择音频
    /// 3. 使用PlayOneShot允许音效叠加
    /// </remarks>
    public void PlayAttackVoice()
    {
        if (_attackVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _attackVoiceClips.Count);
        _audioSource.PlayOneShot(_attackVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机特殊攻击语音
    /// </summary>
    public void PlaySpecialAttackVoice()
    {
        if (_specialAttackVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _specialAttackVoiceClips.Count);
        _audioSource.PlayOneShot(_specialAttackVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机受伤语音
    /// </summary>
    public void PlayHurtVoice()
    {
        if (_hurtVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _hurtVoiceClips.Count);
        _audioSource.PlayOneShot(_hurtVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机回合开始语音
    /// </summary>
    public void PlayStartTurnVoice()
    {
        if (_startTurnVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _startTurnVoiceClips.Count);
        _audioSource.PlayOneShot(_startTurnVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机闪避语音
    /// </summary>
    public void PlayEvadeVoice()
    {
        if (_evadeVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _evadeVoiceClips.Count);
        _audioSource.PlayOneShot(_evadeVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机防御开始语音（当前存在配置检查错误）
    /// </summary>
    /// <remarks>
    /// 注意：当前检查的是_startTurnVoiceClips列表，应改为_startGuardVoiceClips
    /// </remarks>
    public void PlayStartGuardVoice()
    {
        if (_startTurnVoiceClips.Count == 0) return; // 错误配置检查
        _audioSource.Stop();
        int index = Random.Range(0, _startGuardVoiceClips.Count);
        _audioSource.PlayOneShot(_startGuardVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机防御中语音
    /// </summary>
    public void PlayGuardVoice()
    {
        if (_guardVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _guardVoiceClips.Count);
        _audioSource.PlayOneShot(_guardVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机自我增益语音
    /// </summary>
    public void PlaySelfBuffVoice()
    {
        if (_selfBuffVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _selfBuffVoiceClips.Count);
        _audioSource.PlayOneShot(_selfBuffVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机道具使用语音
    /// </summary>
    public void PlayItemUseVoice()
    {
        if (_itemUseVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _itemUseVoiceClips.Count);
        _audioSource.PlayOneShot(_itemUseVoiceClips[index]);
    }
}
