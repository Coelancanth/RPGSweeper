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
        [Required, OnValueChanged("OnTemplateChanged")]
        [InlineEditor]
        public EffectData Template;

        [SerializeField, HideInInspector]
        private SerializableDictionary<string, bool> m_OverrideFlags = new();
        
        [SerializeField, HideInInspector]
        private SerializableDictionary<string, object> m_OverrideValues = new();

        // Cache for reflection results
        private static readonly Dictionary<Type, PropertyInfo[]> s_CachedProperties = new();
        private static readonly Dictionary<PropertyInfo, OverridablePropertyAttribute> s_CachedAttributes = new();
        
        private PropertyInfo[] GetCachedProperties(Type type)
        {
            if (!s_CachedProperties.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties();
                s_CachedProperties[type] = properties;
            }
            return properties;
        }
        
        private OverridablePropertyAttribute GetCachedAttribute(PropertyInfo property)
        {
            if (!s_CachedAttributes.TryGetValue(property, out var attribute))
            {
                attribute = property.GetCustomAttributes(typeof(OverridablePropertyAttribute), true)
                    .FirstOrDefault() as OverridablePropertyAttribute;
                s_CachedAttributes[property] = attribute;
            }
            return attribute;
        }

        // Common overrides that all effects might have
        [FoldoutGroup("Overrides")]
        [ShowInInspector, PropertyOrder(-1)]
        public bool OverrideShape
        {
            get => GetOverrideFlag(nameof(EffectData.Shape));
            set => SetOverrideFlag(nameof(EffectData.Shape), value);
        }

        [FoldoutGroup("Overrides"), ShowIf("OverrideShape")]
        [ShowInInspector, PropertyOrder(-1)]
        public GridShape Shape
        {
            get => GetOverrideValue<GridShape>(nameof(EffectData.Shape));
            set => SetOverrideValue(nameof(EffectData.Shape), value);
        }

        [FoldoutGroup("Overrides")]
        [ShowInInspector, PropertyOrder(-1)]
        public bool OverrideRadius
        {
            get => GetOverrideFlag(nameof(EffectData.Radius));
            set => SetOverrideFlag(nameof(EffectData.Radius), value);
        }

        [FoldoutGroup("Overrides"), ShowIf("OverrideRadius")]
        [ShowInInspector, PropertyOrder(-1)]
        public int Radius
        {
            get => GetOverrideValue<int>(nameof(EffectData.Radius));
            set => SetOverrideValue(nameof(EffectData.Radius), value);
        }

        [FoldoutGroup("Overrides")]
        [ShowInInspector, PropertyOrder(0)]
        [ListDrawerSettings(ShowIndexLabels = false, Expanded = true)]
        [ShowIf("HasCustomProperties")]
        [LabelText("Effect-Specific Overrides")]
        private List<CustomPropertyOverride> CustomProperties
        {
            get
            {
                if (Template == null) return new List<CustomPropertyOverride>();
                return GetCustomProperties();
            }
        }

        private List<CustomPropertyOverride> GetCustomProperties()
        {
            var properties = new List<CustomPropertyOverride>();
            if (Template == null) return properties;
            
            var type = Template.GetType();
            var propertyInfos = GetCachedProperties(type);
            
            foreach (var prop in propertyInfos)
            {
                var attr = GetCachedAttribute(prop);
                if (attr != null)
                {
                    properties.Add(new CustomPropertyOverride(
                        prop.Name,
                        prop.PropertyType,
                        GetOverrideFlag(prop.Name),
                        GetOverrideValue<object>(prop.Name),
                        (flag) => SetOverrideFlag(prop.Name, flag),
                        (value) => SetOverrideValue(prop.Name, value)
                    ));
                }
            }
            
            return properties;
        }

        private bool HasCustomProperties()
        {
            return Template != null && GetCustomProperties().Count > 0;
        }

        private bool GetOverrideFlag(string propertyName)
        {
            return m_OverrideFlags.TryGetValue(propertyName, out var flag) && flag;
        }

        private void SetOverrideFlag(string propertyName, bool value)
        {
            m_OverrideFlags[propertyName] = value;
            if (!value)
            {
                m_OverrideValues.Remove(propertyName);
            }
        }

        private T GetOverrideValue<T>(string propertyName)
        {
            if (m_OverrideValues.TryGetValue(propertyName, out var value))
            {
                return (T)value;
            }
            return default;
        }

        private void SetOverrideValue(string propertyName, object value)
        {
            m_OverrideValues[propertyName] = value;
        }

        private void OnTemplateChanged()
        {
            // Clear overrides that don't apply to the new template
            var validProperties = new HashSet<string> { nameof(EffectData.Shape), nameof(EffectData.Radius) };
            if (Template != null)
            {
                var type = Template.GetType();
                var propertyInfos = GetCachedProperties(type);
                
                foreach (var prop in propertyInfos)
                {
                    if (GetCachedAttribute(prop) != null)
                    {
                        validProperties.Add(prop.Name);
                    }
                }
            }

            // Remove invalid overrides
            var keysToRemove = m_OverrideFlags.Keys.Where(k => !validProperties.Contains(k)).ToList();
            foreach (var key in keysToRemove)
            {
                m_OverrideFlags.Remove(key);
                m_OverrideValues.Remove(key);
            }
        }

        public IEffect CreateEffect()
        {
            if (Template == null) return null;

            // Create a temporary EffectData with our overrides
            var effectData = ScriptableObject.CreateInstance(Template.GetType()) as EffectData;
            
            // Copy all properties from template
            var type = Template.GetType();
            var propertyInfos = GetCachedProperties(type);
            
            foreach (var prop in propertyInfos)
            {
                if (prop.CanWrite)
                {
                    var attr = GetCachedAttribute(prop);
                    if (attr != null)
                    {
                        var value = GetOverrideFlag(prop.Name) 
                            ? GetOverrideValue<object>(prop.Name) 
                            : prop.GetValue(Template);
                        prop.SetValue(effectData, value);
                    }
                    else
                    {
                        prop.SetValue(effectData, prop.GetValue(Template));
                    }
                }
            }

            // Handle common properties
            effectData.Shape = OverrideShape ? Shape : Template.Shape;
            effectData.Radius = OverrideRadius ? Radius : Template.Radius;
            effectData.TargetLayers = Template.TargetLayers;

            // Create the effect instance
            var effect = effectData.CreateEffect();
            
            // Destroy the temporary data
            if (Application.isPlaying)
                Destroy(effectData);
            else
                DestroyImmediate(effectData);
                
            return effect;
        }

        [Serializable]
        private class CustomPropertyOverride
        {
            [HorizontalGroup("Split")]
            [VerticalGroup("Split/Left"), LabelWidth(100)]
            [ShowInInspector]
            public string Name { get; private set; }

            [VerticalGroup("Split/Left")]
            [ShowInInspector]
            public bool Override
            {
                get => m_GetFlag();
                set => m_SetFlag(value);
            }

            [VerticalGroup("Split/Right"), ShowIf("Override")]
            [ShowInInspector]
            public object Value
            {
                get => m_GetValue();
                set => m_SetValue(value);
            }

            private readonly Type m_PropertyType;
            private readonly Func<bool> m_GetFlag;
            private readonly Action<bool> m_SetFlag;
            private readonly Func<object> m_GetValue;
            private readonly Action<object> m_SetValue;

            public CustomPropertyOverride(string name, Type propertyType, bool currentFlag, object currentValue, 
                Action<bool> setFlag, Action<object> setValue)
            {
                Name = name;
                m_PropertyType = propertyType;
                m_GetFlag = () => currentFlag;
                m_SetFlag = setFlag;
                m_GetValue = () => currentValue;
                m_SetValue = setValue;
            }
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