using UnityEngine;

namespace RPGMinesweeper
{
    // Strategy factory for creating mine display strategies
    public interface IMineDisplayStrategyFactory
    {
        IMineDisplayStrategy CreateStrategy(IMine mine);
    }

    // Default implementation of mine display strategy factory
    public class DefaultMineDisplayStrategyFactory : IMineDisplayStrategyFactory
    {
        public IMineDisplayStrategy CreateStrategy(IMine mine)
        {
            if (mine is DisguisedMonsterMine)
            {
                return new DisguisedMonsterMineDisplayStrategy();
            }
            else if (mine is MonsterMine)
            {
                return new MonsterMineDisplayStrategy();
            }
            return new StandardMineDisplayStrategy();
        }
    }
} 