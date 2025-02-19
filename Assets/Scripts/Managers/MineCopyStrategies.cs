using UnityEngine;

public interface IMineCopyStrategy
{
    void CopyMineProperties(IMine sourceMine, IMine targetMine);
}

public class StandardMineCopyStrategy : IMineCopyStrategy
{
    public void CopyMineProperties(IMine sourceMine, IMine targetMine)
    {
        // Standard mines have no extra properties to copy.
    }
}

public class MonsterMineCopyStrategy : IMineCopyStrategy
{
    public void CopyMineProperties(IMine sourceMine, IMine targetMine)
    {
        if (sourceMine is MonsterMine sourceMonster && targetMine is MonsterMine targetMonster)
        {
            targetMonster.CurrentHp = sourceMonster.CurrentHp;
        }
    }
}

public static class MineCopyStrategyFactory
{
    public static IMineCopyStrategy CreateStrategy(MineType type)
    {
        return type switch
        {
            MineType.Monster => new MonsterMineCopyStrategy(),
            MineType.Standard => new StandardMineCopyStrategy(),
            _ => new StandardMineCopyStrategy()
        };
    }
}
