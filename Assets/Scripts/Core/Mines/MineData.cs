using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MineData", menuName = "RPGMinesweeper/MineData")]
public class MineData : ScriptableObject
{
    [Header("Basic Properties")]
    public MineType Type;
    public int Value;
    
    [Header("Shape Properties")]
    public MineShape Shape;
    public int Radius;

    [Header("Visual Properties")]
    public Sprite MineSprite;
    
    [Header("Effects")]
    public List<EffectData> Effects;
} 