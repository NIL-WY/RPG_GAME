using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleConfig
{
    [Header("英雄配置")]
    [Tooltip("需要上阵的英雄ID列表（1-2个）")]
    public string[] heroIDs;

    [Header("敌人配置")]
    [Tooltip("需要生成的敌人ID列表（1-3个）")]
    public string[] enemyIDs;

    [Tooltip("是否保留尸体（Hero专用）")]
    public bool keepHeroCorpse = true;
}
