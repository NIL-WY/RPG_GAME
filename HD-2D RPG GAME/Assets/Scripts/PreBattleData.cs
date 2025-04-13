using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PreBattleData
{
    // 记录玩家进入战斗前的位置
    public static Vector3 playerPosition;

    // 记录所有队伍成员的状态数据：Key 为角色名称（例如 "Fox" 或 "Dual"）
    // 这里记录为 (currentHP, currentMP, level, currentEXP, maxHP, maxMP)
    public static Dictionary<string, (float currentHP, float currentMP, int level, int currentEXP, float maxHP, float maxMP)> partyStates
        = new Dictionary<string, (float, float, int, int, float, float)>();
}
