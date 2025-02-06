using UnityEngine;

//[System.Serializable]
[CreateAssetMenu(fileName = "CharacterUIConfig", menuName = "RPGMinesweeper/CharacterUIConfig")]
public class CharacterUIConfig : ScriptableObject
{
    [Header("HP Display")]
    [SerializeField] private string m_HPFormat = "HP: {0}/{1}";
    [SerializeField] private Color m_HPColor = Color.red;
    [Tooltip("Animation duration for HP changes")]
    [Range(0f, 1f)] private float m_HPAnimationDuration = 0.3f;

    [Header("Experience Display")]
    [SerializeField] private string m_EXPFormat = "EXP: {0}/{1}";
    [SerializeField] private Color m_EXPColor = Color.yellow;
    [Tooltip("Animation duration for EXP changes")]
    [Range(0f, 1f)] private float m_EXPAnimationDuration = 0.3f;

    [Header("Level Display")]
    [SerializeField] private string m_LevelFormat = "Level: {0}";
    [SerializeField] private Color m_LevelColor = Color.green;
    [Tooltip("Animation duration for level up")]
    [Range(0f, 1f)] private float m_LevelAnimationDuration = 0.5f;

    // Public accessors
    public string HPFormat => m_HPFormat;
    public string EXPFormat => m_EXPFormat;
    public string LevelFormat => m_LevelFormat;
    public Color HPColor => m_HPColor;
    public Color EXPColor => m_EXPColor;
    public Color LevelColor => m_LevelColor;
    public float HPAnimationDuration => m_HPAnimationDuration;
    public float EXPAnimationDuration => m_EXPAnimationDuration;
    public float LevelAnimationDuration => m_LevelAnimationDuration;
} 