/// <summary>
/// ISelfTargetable 接口定义了自我目标技能的触发方法，
/// 用于表示技能对自身生效（例如辅助、增益类技能）。
/// </summary>
public interface ISelfTargetable
{
    /// <summary>
    /// 触发技能效果，使指定的英雄受到该技能影响。
    /// </summary>
    /// <param name="hero">目标英雄，通常为施法者自身</param>
    public void Trigger(Hero hero);
}
