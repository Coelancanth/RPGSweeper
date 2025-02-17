using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

namespace RPGMinesweeper.Effects
{
    [CreateAssetMenu(fileName = "FreezeEffectData", menuName = "RPGMinesweeper/Effects/FreezeEffect")]
    public class FreezeEffectData : EffectData
    {
        [Header("Mode Settings")]
        [SerializeField, Tooltip("Whether this effect should be persistent or triggerable")]
        private EffectType m_Mode = EffectType.Persistent;

        public override IEffect CreateEffect()
        {
            var effect = new FreezeEffect(Duration, Radius, Shape);
            effect.SetMode(m_Mode);
            return effect;
        }
    }
}