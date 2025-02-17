using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;

[CreateAssetMenu(fileName = "UnfreezeEffectData", menuName = "RPGMinesweeper/Effects/Unfreeze Effect")]
public class UnfreezeEffectData : EffectData
{
    public override IEffect CreateEffect()
    {
        return new UnfreezeEffect(Radius, Shape);
    }
} 