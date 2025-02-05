using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MineData", menuName = "RPGMinesweeper/MineData")]
public class MineData : ScriptableObject
{
    public MineType Type;
    public int Damage;
    public float TriggerRadius;
    public List<EffectData> Effects;
} 