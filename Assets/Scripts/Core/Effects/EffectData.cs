using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "RPGMinesweeper/EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType Type;
    public float Duration;
    public float Magnitude;
}

public enum EffectType
{
    None,
    Reveal,
    Transform,
    Heal,
    SpawnItem
} 