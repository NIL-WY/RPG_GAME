using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 音效处理器，用于播放界面交互时的提示音（高音与低音）
/// </summary>
[RequireComponent(typeof(AudioSource))] // 确保挂载该脚本的物体上有 AudioSource 组件
public class UISoundHandler : MonoBehaviour
{
    /// <summary>
    /// 较低频率的提示音效，例如错误、取消等场景使用
    /// </summary>
    [SerializeField] AudioClip _lowBeep;

    /// <summary>
    /// 较高频率的提示音效，例如确认、选择等场景使用
    /// </summary>
    [SerializeField] AudioClip _highBeep;

    /// <summary>
    /// 播放音效的音频源组件
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// 在 Awake 阶段初始化 AudioSource 引用
    /// </summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 播放低频率的提示音效（low beep）
    /// </summary>
    public void PlayLowBeep()
    {
        _audioSource.PlayOneShot(_lowBeep); // 使用一次性播放方式，不打断其他音效
    }

    /// <summary>
    /// 播放高频率的提示音效（high beep）
    /// </summary>
    public void PlayHighBeep()
    {
        _audioSource.PlayOneShot(_highBeep);
    }
}
