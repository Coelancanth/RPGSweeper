using UnityEngine;
using RPGMinesweeper;

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

public class DisguisedMonsterMineCopyStrategy : IMineCopyStrategy
{
    public void CopyMineProperties(IMine sourceMine, IMine targetMine)
    {
        // First copy monster properties
        new MonsterMineCopyStrategy().CopyMineProperties(sourceMine, targetMine);
        
        // Then copy disguise-specific properties
        if (sourceMine is DisguisedMonsterMine sourceDisguised && targetMine is DisguisedMonsterMine targetDisguised)
        {
            // If needed in future, copy disguised-specific properties here
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
            MineType.DisguisedMonster => new DisguisedMonsterMineCopyStrategy(),
            MineType.Standard => new StandardMineCopyStrategy(),
            _ => new StandardMineCopyStrategy()
        };
    }
}
