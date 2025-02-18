using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "FreezeEffectData", menuName = "RPGMinesweeper/Effects/FreezeEffect")]
    public class FreezeEffectData : EffectData
    {

        public override IEffect CreateEffect()
        {
            var effect = new FreezeEffect(Duration, Radius, Shape);
            return effect;
        }
    }
}