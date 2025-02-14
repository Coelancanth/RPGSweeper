using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MineDisplayConfig", menuName = "RPGMinesweeper/MineDisplayConfig")]
public class MineDisplayConfig : ScriptableObject
{
    public event Action OnConfigChanged;

    [Header("Monster Mine Text Positions")]
    [Tooltip("Position for HP display (usually above the sprite)")]
    [SerializeField] private Vector3 m_HPPosition = new Vector3(0f, 0.4f, -0.1f);
    
    [Tooltip("Position for monster's damage value (usually below the sprite)")]
    [SerializeField] private Vector3 m_DamagePosition = new Vector3(0f, -0.2f, -0.1f);
    
    [Tooltip("Position for monster's mine value (usually to the right of the sprite)")]
    [SerializeField] private Vector3 m_MonsterMineValuePosition = new Vector3(0.3f, 0f, -0.1f);

    [Header("Standard Mine Text Position")]
    [Tooltip("Position for standard mine's value (usually centered)")]
    [SerializeField] private Vector3 m_StandardMineValuePosition = new Vector3(0f, 0f, -0.1f);

    [Header("Empty Cell Text Position")]
    [Tooltip("Position for empty cell's value (surrounding mine count)")]
    [SerializeField] private Vector3 m_EmptyCellValuePosition = new Vector3(0f, 0f, -0.1f);

    [Header("Sprite Position")]
    [Tooltip("Position offset for mine sprite")]
    [SerializeField] private Vector3 m_MineOffset = new Vector3(0f, 0.2f, -0.1f);

    [Header("Text Colors")]
    [Tooltip("Default color for empty cell values (surrounding mine count)")]
    [SerializeField] private Color m_DefaultValueColor = Color.white;
    
    [Tooltip("Color for monster's base damage value")]
    [SerializeField] private Color m_MonsterPowerColor = new Color(1f, 0.6f, 0f); // Orange
    
    [Tooltip("Color when monster is enraged")]
    [SerializeField] private Color m_EnragedColor = Color.red;
    
    [Tooltip("Color for monster's HP when at full health")]
    [SerializeField] private Color m_HPHighColor = Color.green;
    
    [Tooltip("Color for monster's HP when at low health")]
    [SerializeField] private Color m_HPLowColor = Color.red;

    private void OnValidate()
    {
        // Notify listeners when values are changed in inspector
        OnConfigChanged?.Invoke();
    }

    // Properties for Monster Mine
    public Vector3 HPPosition
    {
        get => m_HPPosition;
        set
        {
            if (m_HPPosition != value)
            {
                m_HPPosition = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    public Vector3 DamagePosition
    {
        get => m_DamagePosition;
        set
        {
            if (m_DamagePosition != value)
            {
                m_DamagePosition = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    public Vector3 MonsterMineValuePosition
    {
        get => m_MonsterMineValuePosition;
        set
        {
            if (m_MonsterMineValuePosition != value)
            {
                m_MonsterMineValuePosition = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    // Property for Standard Mine
    public Vector3 StandardMineValuePosition
    {
        get => m_StandardMineValuePosition;
        set
        {
            if (m_StandardMineValuePosition != value)
            {
                m_StandardMineValuePosition = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    // Property for Empty Cell
    public Vector3 EmptyCellValuePosition
    {
        get => m_EmptyCellValuePosition;
        set
        {
            if (m_EmptyCellValuePosition != value)
            {
                m_EmptyCellValuePosition = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    // Property for Sprite
    public Vector3 MineOffset
    {
        get => m_MineOffset;
        set
        {
            if (m_MineOffset != value)
            {
                m_MineOffset = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    // Color Properties
    public Color DefaultValueColor
    {
        get => m_DefaultValueColor;
        set
        {
            if (m_DefaultValueColor != value)
            {
                m_DefaultValueColor = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    public Color MonsterPowerColor
    {
        get => m_MonsterPowerColor;
        set
        {
            if (m_MonsterPowerColor != value)
            {
                m_MonsterPowerColor = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    public Color EnragedColor
    {
        get => m_EnragedColor;
        set
        {
            if (m_EnragedColor != value)
            {
                m_EnragedColor = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    public Color HPHighColor
    {
        get => m_HPHighColor;
        set
        {
            if (m_HPHighColor != value)
            {
                m_HPHighColor = value;
                OnConfigChanged?.Invoke();
            }
        }
    }

    public Color HPLowColor
    {
        get => m_HPLowColor;
        set
        {
            if (m_HPLowColor != value)
            {
                m_HPLowColor = value;
                OnConfigChanged?.Invoke();
            }
        }
    }
} 