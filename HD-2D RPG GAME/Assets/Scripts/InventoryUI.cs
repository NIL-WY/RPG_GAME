using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;         // 整个面板 (用来显示/隐藏)
    public Transform itemListParent;          // 用于生成道具项的父物体
    public GameObject itemEntryPrefab;        // 显示单个道具的 UI 预制体

    private List<GameObject> spawnedEntries = new();

    private void Awake()
    {
        // 注册监听事件
        Inventory.OnInventoryChanged += UpdateUI;
        inventoryPanel.SetActive(false); // 初始关闭
    }

    private void OnDestroy()
    {
        // 记得注销事件监听
        Inventory.OnInventoryChanged -= UpdateUI;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventoryUI();
        }
    }

    private void ToggleInventoryUI()
    {
        bool active = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(active);
        if (active)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        foreach (var entry in spawnedEntries)
        {
            Destroy(entry);
        }
        spawnedEntries.Clear();

        var inventory = PartyManager.Instance.Inventory;
        foreach (var item in inventory.Items)
        {
            GameObject entryGO = Instantiate(itemEntryPrefab, itemListParent);
            var texts = entryGO.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = item.Name;
            texts[1].text = "x" + item.Quantity;
            spawnedEntries.Add(entryGO);
        }
    }
}
