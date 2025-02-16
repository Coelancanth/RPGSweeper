using UnityEngine;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;

[CreateAssetMenu(fileName = "ConfusionEffectData", menuName = "RPGMinesweeper/Effects/ConfusionEffect")]
public class ConfusionEffectData : EffectData
{
    public override IEffect CreateEffect()
    {
        return new ConfusionEffect(Duration, Radius, Shape);
    }
} 