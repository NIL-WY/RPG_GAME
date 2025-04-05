using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器类，处理玩家的移动、动画状态以及奔跑切换等逻辑。
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 玩家移动的速度，当普通行走时为2f，奔跑时为5f。
    /// </summary>
    [SerializeField] private float speed = 2f;

    /// <summary>
    /// 玩家动画控制器，用于切换行走和奔跑动画。
    /// </summary>
    [SerializeField] private Animator playerani;

    /// <summary>
    /// 玩家控制输入类，封装玩家操作的输入读取。
    /// </summary>
    private PlayerControls playerControls;

    /// <summary>
    /// 刚体组件，用于实现物理移动。
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// 玩家移动的方向向量，归一化后用于计算实际移动。
    /// </summary>
    private Vector3 movement;

    /// <summary>
    /// 标识玩家当前是否处于行走状态。
    /// </summary>
    public bool isWalking;

    /// <summary>
    /// 标识玩家当前是否处于奔跑状态。
    /// </summary>
    public bool isRuning;

    /// <summary>
    /// 在脚本实例化时调用，初始化玩家输入控制实例。
    /// </summary>
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    /// <summary>
    /// 当脚本启用时调用，激活玩家输入控制。
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    /// <summary>
    /// 在游戏开始时调用，初始化刚体组件并设置初始状态。
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isRuning = false;
        isWalking = false;
    }

    /// <summary>
    /// 每帧调用，处理玩家输入、动画状态更新以及切换奔跑和行走的逻辑。
    /// </summary>
    void Update()
    {
        // 获取玩家在水平面上的输入，x表示左右方向，z表示上下方向
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        // 根据输入构造移动向量并归一化，避免对角线移动时速度过快
        movement = new Vector3(x, 0, z).normalized;

        // 如果没有输入，则将状态重置为不移动，并恢复行走速度
        if (movement == Vector3.zero)
        {
            speed = 2f;
            isWalking = false;
            isRuning = false;
            playerani.SetBool("isWalking", false);
            playerani.SetBool("isRuning", false);
        }
        else
        {
            // 设置动画控制器的输入参数，用于平滑过渡动画
            playerani.SetFloat("xinput", x);
            playerani.SetFloat("yinput", z);
            isWalking = true;
            playerani.SetBool("isWalking", true);
        }

        // 按下左Shift键时切换奔跑状态，且仅在有移动输入时有效
        if (Input.GetKeyDown(KeyCode.LeftShift) && movement != Vector3.zero)
        {
            if (isRuning == false)
            {
                isRuning = true;
                speed = 5f;
                playerani.SetBool("isRuning", true);
            }
            else if (isRuning == true)
            {
                isRuning = false;
                isWalking = true;
                speed = 2f;
                playerani.SetBool("isWalking", true);
                playerani.SetBool("isRuning", false);
            }
        }
    }

    /// <summary>
    /// 固定时间间隔调用，用于处理刚体的物理移动，保证移动的平滑性和物理一致性。
    /// </summary>
    private void FixedUpdate()
    {
        // 根据当前速度、时间间隔和移动方向更新刚体位置，实现物理层面的移动
        rb.MovePosition(transform.position + speed * Time.fixedDeltaTime * movement);
    }
}
