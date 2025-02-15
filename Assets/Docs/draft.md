# Template-Based ScriptableObject Instantiation

## Problem Statement
When referencing ScriptableObjects (like `MineData` in `EffectData`), we often want to use the referenced object as a template rather than a direct reference. This allows creating new instances with customizable data while maintaining the base structure.

## Solution Approaches

### Approach 1: Using OnValidate (Recommended)
Unity's `OnValidate` method provides a simpler, more native approach to handle template instantiation in the editor:

```csharp
public class EffectData : ScriptableObject
{
    [Header("Mine Template")]
    [SerializeField] private MineData m_MineTemplate;
    [SerializeField] private MineData m_MineInstance;
    
    // Custom overrides
    [SerializeField] private int m_CustomValue;
    [SerializeField] private Color m_CustomValueColor;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_MineTemplate != null && m_MineInstance == null)
        {
            // Create new instance
            m_MineInstance = ScriptableObject.CreateInstance<MineData>();
            UnityEditor.AssetDatabase.AddObjectToAsset(m_MineInstance, this);
            
            // Copy template values
            m_MineInstance.Type = m_MineTemplate.Type;
            m_MineInstance.Shape = m_MineTemplate.Shape;
            m_MineInstance.Radius = m_MineTemplate.Radius;
            
            // Apply custom overrides
            m_MineInstance.Value = m_CustomValue;
            
            // Mark the asset dirty to ensure changes are saved
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    #endif
}
```

Benefits of this approach:
1. Simpler implementation - no wrapper class needed
2. Automatic handling in the Unity Editor
3. Native integration with Unity's asset system
4. Proper serialization of sub-assets
5. Better memory management as instances are properly tracked as sub-assets

### Approach 2: Wrapper Class (Alternative)

### 1. Using CreateInstance Method
Unity provides `ScriptableObject.CreateInstance<T>()` which can create new instances at runtime. We can combine this with a custom editor to automatically clone templates.

### 2. Implementation Example
Using `MineData` and `EffectData` as examples:

```csharp
[Serializable]
public class TemplatedMineData
{
    [SerializeField] private MineData m_Template;
    [SerializeField] private MineData m_Instance;
    
    // Custom properties that override template values
    [SerializeField] private int m_CustomValue;
    [SerializeField] private Color m_CustomValueColor;
    
    public MineData Instance => m_Instance;
    
    public void Initialize()
    {
        if (m_Template != null && m_Instance == null)
        {
            // Create new instance from template
            m_Instance = ScriptableObject.CreateInstance<MineData>();
            // Copy base values
            m_Instance.Type = m_Template.Type;
            m_Instance.Shape = m_Template.Shape;
            m_Instance.Radius = m_Template.Radius;
            // Apply custom overrides
            m_Instance.Value = m_CustomValue;
            // ... other property copying
        }
    }
}
```

### 3. Usage in EffectData
```csharp
public class EffectData : ScriptableObject
{
    [Header("Mine Template")]
    [SerializeField] private TemplatedMineData m_MineTemplate;
    
    private void OnEnable()
    {
        m_MineTemplate.Initialize();
    }
}
```

## Comparison of Approaches

### OnValidate Approach
Pros:
- Simpler implementation
- Better integration with Unity's asset system
- Automatic cleanup of sub-assets
- Works seamlessly in the editor

Cons:
- Only works in editor (but that's typically when we need it)
- Less flexible for runtime modifications

### Wrapper Class Approach
Pros:
- More flexible for runtime use
- Better encapsulation of template logic
- Can be extended more easily

Cons:
- More complex implementation
- Requires manual cleanup
- Needs additional serialization handling

## Recommendation
Use the OnValidate approach unless you specifically need runtime template instantiation. It's simpler, more maintainable, and better integrated with Unity's systems.

## Benefits
1. Maintains SOLID principles:
   - Single Responsibility: Template handling is encapsulated
   - Open/Closed: New template types can be added without modifying existing code
   - Dependency Inversion: Works with abstractions rather than concrete implementations

2. Follows Unity best practices:
   - Proper serialization support
   - Inspector-friendly design
   - Clean separation of template and instance data

## Implementation Notes
1. Consider using `[CreateAssetMenu]` for template creation
2. Implement proper cleanup in editor scripts
3. Handle template updates appropriately
4. Consider adding validation to ensure template integrity

## Considerations
1. Memory management: Instances should be properly cleaned up
2. Asset references: Ensure proper handling of sub-assets
3. Scene serialization: Consider how instances persist between play sessions
4. Editor tooling: May need custom editor scripts for better UX
