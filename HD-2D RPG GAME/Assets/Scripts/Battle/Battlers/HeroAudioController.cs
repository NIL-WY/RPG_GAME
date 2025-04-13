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
    public void PlayAttackVoice()
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放攻击语音！");
            return;
        }
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
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放特殊攻击语音！");
            return;
        }
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
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放受伤语音！");
            return;
        }
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
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放回合开始语音！");
            return;
        }
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
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放闪避语音！");
            return;
        }
        if (_evadeVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _evadeVoiceClips.Count);
        _audioSource.PlayOneShot(_evadeVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机防御中语音
    /// </summary>
    public void PlayGuardVoice()
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放防御中语音！");
            return;
        }
        if (_guardVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _guardVoiceClips.Count);
        _audioSource.PlayOneShot(_guardVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机防御开始语音
    /// </summary>
    public void PlayStartGuardVoice()
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放开始防御语音！");
            return;
        }
        // 修正：检查 _startGuardVoiceClips 列表是否为空
        if (_startGuardVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _startGuardVoiceClips.Count);
        _audioSource.PlayOneShot(_startGuardVoiceClips[index]);
    }

    /// <summary>
    /// 播放随机自我增益语音
    /// </summary>
    public void PlaySelfBuffVoice()
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放自我增益语音！");
            return;
        }
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
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource已被销毁，无法播放道具使用语音！");
            return;
        }
        if (_itemUseVoiceClips.Count == 0) return;
        _audioSource.Stop();
        int index = Random.Range(0, _itemUseVoiceClips.Count);
        _audioSource.PlayOneShot(_itemUseVoiceClips[index]);
    }
}
