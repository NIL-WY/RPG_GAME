using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BattleSetupManager : MonoBehaviour
{
    public static BattleSetupManager Instance;
    private BattleConfig _pendingConfig;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持跨场景
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 在进入战斗前准备战斗配置（由 BattleTrigger 调用）
    /// </summary>
    public void PrepareBattle(BattleConfig config)
    {
        _pendingConfig = config;
    }

    /// <summary>
    /// 获取待使用的战斗配置，并清空当前缓存。
    /// </summary>
    public BattleConfig GetConfig()
    {
        BattleConfig config = _pendingConfig;
        _pendingConfig = null;
        return config;
    }

    /// <summary>
    /// 清空配置（可能在战斗结束时调用）
    /// </summary>
    public void ClearConfig()
    {
        _pendingConfig = null;
    }
}