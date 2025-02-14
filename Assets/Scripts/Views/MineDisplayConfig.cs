using UnityEngine;

[System.Serializable]
public class MineDisplayConfig
{
    [Header("Text Positions")]
    [Tooltip("Position for HP display (usually above the sprite)")]
    public Vector3 HPPosition = new Vector3(0f, 0.4f, -0.1f);
    
    [Tooltip("Position for damage display (usually below the sprite)")]
    public Vector3 DamagePosition = new Vector3(0f, -0.2f, -0.1f);
    
    [Tooltip("Position for mine value display (usually to the right of the sprite)")]
    public Vector3 ValuePosition = new Vector3(0.3f, 0f, -0.1f);

    [Header("Text Colors")]
    public Color DefaultValueColor = Color.white;
    public Color EnragedColor = Color.red;
    public Color HPHighColor = Color.green;
    public Color HPLowColor = Color.red;
} 