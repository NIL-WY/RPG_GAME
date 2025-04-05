using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Billboard 类用于实现面向摄像机的效果，使对象始终面向主摄像机。
/// </summary>
public class Billboard : MonoBehaviour
{
    /// <summary>
    /// 主摄像机的 Transform，用于获取摄像机的朝向信息。
    /// </summary>
    public Transform CameraTransform;


    /// <summary>
    /// Start 方法在脚本实例启动时调用，初始化摄像机 Transform 
    /// </summary>
    void Start()
    {
        // 获取主摄像机的 Transform 组件
        CameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// LateUpdate 方法在每一帧的最后调用，保证所有摄像机移动完成后更新物体朝向，
    /// 使对象始终面向摄像机。
    /// </summary>
    void LateUpdate()
    {
        // 将当前对象的前方向设置为摄像机的前方向，实现始终面向摄像机效果
        transform.forward = CameraTransform.forward;
    }
}
