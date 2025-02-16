using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "RangeRevealEffectData", menuName = "RPGMinesweeper/Effects/RangeRevealEffect")]
    public class RangeRevealEffectData : EffectData
    {
        public override IEffect CreateEffect()
        {
            return new RangeRevealEffect(Radius, Shape);
        }
    }
} 