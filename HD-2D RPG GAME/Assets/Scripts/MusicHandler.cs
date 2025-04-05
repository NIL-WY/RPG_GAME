using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理背景音乐播放的组件，支持带引子（Intro）和循环（Loop）部分的音乐播放逻辑。
/// </summary>
[RequireComponent(typeof(AudioSource))] // 确保挂载该脚本的物体上包含 AudioSource 组件
public class MusicHandler : MonoBehaviour
{
    /// <summary>
    /// 播放音乐引子部分的 AudioSource。
    /// </summary>
    [SerializeField] AudioSource _audioSourceIntro;

    /// <summary>
    /// 播放音乐循环部分的 AudioSource。
    /// </summary>
    [SerializeField] AudioSource _audioSourceLoop;

    /// <summary>
    /// 音乐的引子部分音频剪辑（先播放这段）。
    /// </summary>
    [SerializeField] AudioClip _songIntro;

    /// <summary>
    /// 音乐的循环部分音频剪辑（引子播放完后重复播放这段）。
    /// </summary>
    [SerializeField] AudioClip _songLoop;

    /// <summary>
    /// 初始化音乐播放设置，并开始播放音乐引子部分，随后自动切换到循环部分。
    /// </summary>
    private void Start()
    {
        // 设置引子 AudioSource 不循环、不自动播放
        _audioSourceIntro.loop = false;
        _audioSourceIntro.playOnAwake = false;

        // 设置循环 AudioSource 循环播放、不自动播放
        _audioSourceLoop.loop = true;
        _audioSourceLoop.playOnAwake = false;

        // 将音频剪辑赋值到各自的 AudioSource 上
        _audioSourceIntro.clip = _songIntro;
        _audioSourceLoop.clip = _songLoop;

        // 播放引子部分
        _audioSourceIntro.Play();

        // 延迟播放循环部分，时间为引子剪辑的时长
        _audioSourceLoop.PlayDelayed(_audioSourceIntro.clip.length);
    }
}
