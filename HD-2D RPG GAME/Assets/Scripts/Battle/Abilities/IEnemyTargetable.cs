/// <summary>
/// IEnemyTargetable 接口定义了针对敌人的技能触发方法，
/// 用于表示技能同时需要对施法者和目标敌人生效（例如攻击、控制类技能）。
/// </summary>
public interface IEnemyTargetable
{
    /// <summary>
    /// 触发技能效果，针对指定的英雄和敌人执行技能逻辑。
    /// </summary>
    /// <param name="hero">技能的施法者或使用者</param>
    /// <param name="enemy">技能的目标敌人</param>
    public void Trigger(Hero hero, Enemy enemy);
}
