using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "ConfusionEffectData", menuName = "RPGMinesweeper/Effects/ConfusionEffect")]
    public class ConfusionEffectData : EffectData
    {

        public override IEffect CreateEffect()
        {
            var effect = new ConfusionEffect(Duration, Radius, Shape);
            return effect;
        }
    }
} 