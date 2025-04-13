using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public ItemData[] itemsInChest; // 宝箱内的物品（通过 Inspector 赋值）
    private bool _isOpened = false;
    public Animator chestAnimator; // 宝箱的 Animator 组件
    public string openAnimationTrigger = "Open"; // Animator 中开启动画的触发器名称

    public bool isOpened = false; // 宝箱是否已经被打开

    private bool isPlayerNearby = false; // 玩家是否靠近
    [Header("Particle Effect")]
    public GameObject particleEffectPrefab; // 粒子特效预制体
    public Transform particleSpawnPoint;    // 特效播放位置
    public float particleDuration = 2f;     // 特效持续时间

    private void Update()
    {
        // 检查玩家是否在交互范围内
        if (isPlayerNearby && !isOpened && Input.GetKeyDown(KeyCode.F))
        {
            OpenChest();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // 玩家进入触发区时，标记为可以交互
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 玩家离开触发区时，标记为不可交互
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    public void OpenChest()
    {
        if (!isOpened)
        {
            chestAnimator.SetTrigger(openAnimationTrigger);
            isOpened = true;
            if (particleEffectPrefab != null && particleSpawnPoint != null)
            {
                GameObject effect = Instantiate(particleEffectPrefab, particleSpawnPoint.position, Quaternion.identity);
                Destroy(effect, particleDuration);
            }
            // 禁用交互，防止重复开启
            this.GetComponent<Collider>().enabled = false;
        }
        if (_isOpened) return;
        _isOpened = true;

        foreach (var itemData in itemsInChest)
        {
            PartyManager.Instance.Inventory.AddItem(itemData);
        }
        Destroy(gameObject);
    }

}
