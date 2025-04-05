using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物类型：飞鸟类怪物，继承自 Enemy。
/// 在生成时会随机在一定高度范围内进行 Y 轴偏移，使其离地漂浮。
/// </summary>
public class MonsterBird : Enemy
{
    // 起始位置最大 Y 轴偏移
    [SerializeField] float _startPositionOffsetMax = 3f;

    // 起始位置最小 Y 轴偏移
    [SerializeField] float _startPositionOffsetMin = 1f;

    /// <summary>
    /// 重写 Start 方法，在怪物初始化时给予 Y 轴上的随机高度偏移。
    /// </summary>
    protected override void Start()
    {
        // 在设定范围内的随机高度偏移
        transform.position += new Vector3(0f, Random.Range(_startPositionOffsetMin, _startPositionOffsetMax), 0f);

        // 调用基类 Enemy 的 Start 方法
        base.Start();
    }
}
