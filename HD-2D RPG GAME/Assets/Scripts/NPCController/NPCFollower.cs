using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollower : MonoBehaviour
{
    [Tooltip("跟随目标，一般为玩家")]
    public Transform target;

    [Tooltip("跟随时保持的最小距离")]
    public float followDistance = 2.0f;

    [Tooltip("默认移动速度（备用）")]
    public float defaultSpeed = 2.0f;

    // 实时移动速度，同步自玩家
    private float moveSpeed;

    [Tooltip("NPC的Sprite子对象上的Animator组件")]
    public Animator spriteAnimator;

    // 保存玩家的控制器引用
    private PlayerController playerController;

    void Start()
    {
        // 如果没有手动指定目标，则自动查找带有 "Player" 标签的对象
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }
        if (target != null)
        {
            playerController = target.GetComponent<PlayerController>();
        }
        // 如果没有手动指定 Animator，则尝试获取子物体上的 Animator
        if (spriteAnimator == null)
        {
            spriteAnimator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (target == null) return;

        // 从玩家控制器同步移动速度（假设玩家奔跑时为5f，行走时为2f）
        if (playerController != null)
        {
            moveSpeed = playerController.isRuning ? 5f : 2f;
        }
        else
        {
            moveSpeed = defaultSpeed;
        }

        // 计算与目标的距离
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > followDistance)
        {
            // 计算朝向玩家的方向并移动
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 更新动画器参数，让 NPC 播放行走或奔跑动画
            if (spriteAnimator != null)
            {
                // 将方向转换为局部坐标，用于动画参数
                Vector3 localDir = transform.InverseTransformDirection(direction);
                spriteAnimator.SetFloat("xinput", localDir.x);
                spriteAnimator.SetFloat("yinput", localDir.z);

                // 同步动画状态：行走和奔跑
                spriteAnimator.SetBool("isWalking", true);
                spriteAnimator.SetBool("isRunning", playerController != null && playerController.isRuning);
            }
        }
        else
        {
            // 当 NPC 足够接近目标时停止移动并关闭动画
            if (spriteAnimator != null)
            {
                spriteAnimator.SetBool("isWalking", false);
                spriteAnimator.SetBool("isRunning", false);
            }
        }
    }
}
