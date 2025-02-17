using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

[CreateAssetMenu(fileName = "FreezeEffectData", menuName = "RPGMinesweeper/Effects/Freeze Effect")]
public class FreezeEffectData : EffectData
{
    public override IEffect CreateEffect()
    {
        return new FreezeEffect(Duration, Radius, Shape);
    }
}