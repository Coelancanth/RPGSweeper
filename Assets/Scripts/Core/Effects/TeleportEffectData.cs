using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "TeleportEffectData", menuName = "RPGMinesweeper/Effects/TeleportEffect")]
    public class TeleportEffectData : EffectData
    {
        public override IEffect CreateEffect()
        {
            var effect = new TeleportEffect(Duration, Radius, Shape);
            return effect;
        }
    }
} 