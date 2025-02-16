using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using RPGMinesweeper.Effects;
using RPGMinesweeper.Grid;
using RPGMinesweeper;  // For MonsterType
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

[Serializable]
public class SerializedKeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public SerializedKeyValuePair() { }
    
    public SerializedKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField]
    private List<SerializedKeyValuePair<TKey, TValue>> m_Entries = new();
    private Dictionary<TKey, TValue> m_Dictionary;

    public SerializableDictionary()
    {
        m_Dictionary = new Dictionary<TKey, TValue>();
    }

    private void RebuildDictionary()
    {
        if (m_Dictionary == null)
        {
            m_Dictionary = new Dictionary<TKey, TValue>();
        }
        m_Dictionary.Clear();
        
        if (m_Entries != null)
        {
            foreach (var entry in m_Entries)
            {
                if (entry.Key != null && !m_Dictionary.ContainsKey(entry.Key))
                {
                    m_Dictionary[entry.Key] = entry.Value;
                }
            }
        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (m_Dictionary == null)
        {
            RebuildDictionary();
        }
        return m_Dictionary.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (m_Dictionary == null)
            {
                RebuildDictionary();
            }
            return m_Dictionary[key];
        }
        set
        {
            if (m_Dictionary == null)
            {
                RebuildDictionary();
            }
            m_Dictionary[key] = value;
            
            // Update serialized entries
            var existingEntry = m_Entries.FirstOrDefault(e => e.Key.Equals(key));
            if (existingEntry != null)
            {
                existingEntry.Value = value;
            }
            else
            {
                m_Entries.Add(new SerializedKeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    public void Remove(TKey key)
    {
        if (m_Dictionary == null)
        {
            RebuildDictionary();
        }
        m_Dictionary.Remove(key);
        m_Entries.RemoveAll(e => e.Key.Equals(key));
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            if (m_Dictionary == null)
            {
                RebuildDictionary();
            }
            return m_Dictionary.Keys;
        }
    }
}

[CreateAssetMenu(fileName = "MineData", menuName = "RPGMinesweeper/MineData")]
public class MineData : SerializedScriptableObject
{
    [Serializable]
    public class EffectInstance
    {
        [Required]
        [InlineEditor]
        public EffectData Template;

        public IEffect CreateEffect()
        {
            return Template?.CreateEffect();
        }
    }

    [TitleGroup("Basic Properties")]
    [HorizontalGroup("Basic Properties/Split")]
    [VerticalGroup("Basic Properties/Split/Left"), LabelWidth(100)]
    public MineType Type;

    [VerticalGroup("Basic Properties/Split/Left")]
    [Tooltip("Display value shown on the grid")]
    public int Value;
    
    [BoxGroup("Area of Effect")]
    [HorizontalGroup("Area of Effect/Split")]
    [VerticalGroup("Area of Effect/Split/Left"), LabelWidth(100)]
    public GridShape Shape;

    [VerticalGroup("Area of Effect/Split/Left")]
    public int Radius;

    [FoldoutGroup("Spawn Settings")]
    public MineSpawnStrategyType SpawnStrategy = MineSpawnStrategyType.Random;

    [BoxGroup("Visual Properties")]
    [HorizontalGroup("Visual Properties/Split")]
    [VerticalGroup("Visual Properties/Split/Left"), LabelWidth(100)]
    [PreviewField(55)]
    public Sprite MineSprite;

    [FoldoutGroup("Visual Properties/Colors")]
    [Tooltip("Color of the displayed mine value text")]
    [ColorPalette]
    [SerializeField] private Color m_ValueColor = Color.white;

    [FoldoutGroup("Visual Properties/Colors")]
    [Tooltip("Color of the displayed mine value text when this is a mine cell")]
    [ColorPalette]
    [SerializeField] private Color m_MineValueColor = Color.yellow;

    public Color ValueColor => m_ValueColor;
    public Color MineValueColor => m_MineValueColor;
    
    [BoxGroup("Effects")]
    [TabGroup("Effects/Tabs", "Passive")]
    [Tooltip("Effects that are applied while the mine is active")]
    [ListDrawerSettings(ShowIndexLabels = true, Expanded = true)]
    [SerializeField] private EffectInstance[] m_PassiveEffects;

    [TabGroup("Effects/Tabs", "Active")]
    [Tooltip("Effects that are applied when the mine is destroyed")]
    [ListDrawerSettings(ShowIndexLabels = true, Expanded = true)]
    [SerializeField] private EffectInstance[] m_ActiveEffects;

    public IEffect[] CreatePassiveEffects()
    {
        if (m_PassiveEffects == null) return Array.Empty<IEffect>();
        var effects = new IEffect[m_PassiveEffects.Length];
        for (int i = 0; i < m_PassiveEffects.Length; i++)
        {
            effects[i] = m_PassiveEffects[i].CreateEffect();
        }
        return effects;
    }

    public IEffect[] CreateActiveEffects()
    {
        if (m_ActiveEffects == null) return Array.Empty<IEffect>();
        var effects = new IEffect[m_ActiveEffects.Length];
        for (int i = 0; i < m_ActiveEffects.Length; i++)
        {
            effects[i] = m_ActiveEffects[i].CreateEffect();
        }
        return effects;
    }

    public List<Vector2Int> GetAffectedPositions(Vector2Int center)
    {
        return GridShapeHelper.GetAffectedPositions(center, Shape, Radius);
    }

    public bool IsPositionAffected(Vector2Int position, Vector2Int center)
    {
        return GridShapeHelper.IsPositionAffected(position, center, Shape, Radius);
    }

    public (int value, Color color) GetValueDisplay()
    {
        return (Value, m_ValueColor);
    }
}

// Add this attribute to any properties in effect data classes that should be overridable
[AttributeUsage(AttributeTargets.Property)]
public class OverridablePropertyAttribute : Attribute
{
    public string DisplayName { get; }
    public OverridablePropertyAttribute(string displayName = null)
    {
        DisplayName = displayName;
    }
} 