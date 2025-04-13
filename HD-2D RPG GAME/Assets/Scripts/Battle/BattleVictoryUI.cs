using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleVictoryUI : MonoBehaviour
{
    [Tooltip("返回按钮")]
    public Button returnButton;

    void Start()
    {
        // 假设 BattleManager 为单例或可通过场景查找获取
        returnButton.onClick.AddListener(() =>
        {
            // 当点击返回按钮时调用 BattleManager 中的处理方法
            BattleManager battleManager = FindObjectOfType<BattleManager>();
            if (battleManager != null)
            {
                battleManager.OnVictoryButtonClicked();
            }
        });
    }
}
