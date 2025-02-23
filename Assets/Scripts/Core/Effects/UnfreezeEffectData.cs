using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "UnfreezeEffectData", menuName = "RPGMinesweeper/Effects/UnfreezeEffect")]
    public class UnfreezeEffectData : EffectData
    {

        public override IEffect CreateEffect()
        {
            var effect = new UnfreezeEffect(Radius, Shape);
            return effect;
        }
    }
} 