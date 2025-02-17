using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "ConfusionEffectData", menuName = "RPGMinesweeper/Effects/ConfusionEffect")]
    public class ConfusionEffectData : EffectData
    {
        [Header("Mode Settings")]
        [SerializeField, Tooltip("Whether this effect should be persistent or triggerable")]
        private EffectType m_Mode = EffectType.Persistent;

        public override IEffect CreateEffect()
        {
            var effect = new ConfusionEffect(Duration, Radius, Shape);
            effect.SetMode(m_Mode);
            return effect;
        }
    }
} 