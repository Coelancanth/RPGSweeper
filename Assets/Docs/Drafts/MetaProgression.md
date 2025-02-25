# Meta-Progression Framework Design

## Overview

The Meta-Progression Framework is designed to manage persistent progression between game sessions, allowing players to unlock new features, upgrades, and content as they play. This framework follows SOLID principles and uses a facade pattern to integrate cleanly with the existing game systems.

## Design Principles

1. **Single Responsibility Principle**: Each component has a clear, focused responsibility
2. **Open/Closed Principle**: Extensible to new progression types without modifying existing code
3. **Liskov Substitution Principle**: Subtypes can be used in place of their parent types
4. **Interface Segregation Principle**: Clients only depend on interfaces they use
5. **Dependency Inversion Principle**: High-level modules depend on abstractions, not concrete implementations

## Core Components

### 1. MetaProgressionManager

The central facade that provides a unified interface to the meta-progression system.

```csharp
public class MetaProgressionManager : MonoBehaviour
{
    [SerializeField] private PlayerComponent m_PlayerComponent;
    
    private IMetaProgressionDataProvider m_DataProvider;
    private Dictionary<string, IProgressionSystem> m_ProgressionSystems = new Dictionary<string, IProgressionSystem>();
    
    private void Awake()
    {
        m_DataProvider = new JsonMetaProgressionDataProvider();
        InitializeSystems();
    }
    
    private void InitializeSystems()
    {
        // Register progression systems
        RegisterSystem(new ResourceProgressionSystem(m_DataProvider));
        RegisterSystem(new UnlockablesProgressionSystem(m_DataProvider));
        RegisterSystem(new ClassProgressionSystem(m_DataProvider));
        RegisterSystem(new BuildingProgressionSystem(m_DataProvider));
    }
    
    private void RegisterSystem(IProgressionSystem system)
    {
        m_ProgressionSystems[system.SystemID] = system;
    }
    
    public void SaveProgress()
    {
        foreach (var system in m_ProgressionSystems.Values)
        {
            system.SaveProgress();
        }
    }
    
    public T GetSystem<T>() where T : class, IProgressionSystem
    {
        foreach (var system in m_ProgressionSystems.Values)
        {
            if (system is T typedSystem)
            {
                return typedSystem;
            }
        }
        return null;
    }
}
```

### 2. IProgressionSystem

The base interface for all progression systems.

```csharp
public interface IProgressionSystem
{
    string SystemID { get; }
    void Initialize();
    void SaveProgress();
    void ResetProgress();
}
```

### 3. Progression Data Model

```csharp
[Serializable]
public class MetaProgressionData
{
    public ResourceData Resources = new ResourceData();
    public UnlockablesData Unlockables = new UnlockablesData();
    public ClassData Classes = new ClassData();
    public BuildingData Buildings = new BuildingData();
}

[Serializable]
public class ResourceData
{
    public int Gold = 0;
    public int Gems = 0;
    public Dictionary<string, int> Materials = new Dictionary<string, int>();
}

[Serializable]
public class UnlockablesData
{
    public HashSet<string> UnlockedItems = new HashSet<string>();
    public HashSet<string> UnlockedAbilities = new HashSet<string>();
    public HashSet<string> UnlockedMaps = new HashSet<string>();
}

[Serializable]
public class ClassData
{
    public Dictionary<string, int> ClassLevels = new Dictionary<string, int>();
    public string ActiveClass = "Default";
}

[Serializable]
public class BuildingData
{
    public Dictionary<string, int> BuildingLevels = new Dictionary<string, int>();
    public Dictionary<string, bool> BuildingUnlocked = new Dictionary<string, bool>();
}
```

### 4. Data Provider

```csharp
public interface IMetaProgressionDataProvider
{
    MetaProgressionData LoadData();
    void SaveData(MetaProgressionData data);
    void ResetData();
    T GetSystemData<T>(string systemId) where T : class;
    void SaveSystemData<T>(string systemId, T data) where T : class;
}

public class JsonMetaProgressionDataProvider : IMetaProgressionDataProvider
{
    private const string SAVE_FILE_NAME = "meta_progression.json";
    private MetaProgressionData m_CachedData;
    
    public MetaProgressionData LoadData()
    {
        if (m_CachedData != null)
            return m_CachedData;
            
        string filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                m_CachedData = JsonUtility.FromJson<MetaProgressionData>(json);
                return m_CachedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading meta progression data: {e.Message}");
                m_CachedData = new MetaProgressionData();
                return m_CachedData;
            }
        }
        
        m_CachedData = new MetaProgressionData();
        return m_CachedData;
    }
    
    public void SaveData(MetaProgressionData data)
    {
        m_CachedData = data;
        string filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }
    
    public void ResetData()
    {
        m_CachedData = new MetaProgressionData();
        SaveData(m_CachedData);
    }
    
    public T GetSystemData<T>(string systemId) where T : class
    {
        var data = LoadData();
        
        // Return the appropriate system data based on systemId
        switch(systemId)
        {
            case "Resources": return data.Resources as T;
            case "Unlockables": return data.Unlockables as T;
            case "Classes": return data.Classes as T;
            case "Buildings": return data.Buildings as T;
            default: return null;
        }
    }
    
    public void SaveSystemData<T>(string systemId, T systemData) where T : class
    {
        var data = LoadData();
        
        // Update the appropriate system data based on systemId
        switch(systemId)
        {
            case "Resources":
                if (systemData is ResourceData resourceData)
                    data.Resources = resourceData;
                break;
            case "Unlockables":
                if (systemData is UnlockablesData unlockablesData)
                    data.Unlockables = unlockablesData;
                break;
            case "Classes":
                if (systemData is ClassData classData)
                    data.Classes = classData;
                break;
            case "Buildings":
                if (systemData is BuildingData buildingData)
                    data.Buildings = buildingData;
                break;
        }
        
        SaveData(data);
    }
}
```

## Specific Progression Systems

### 1. Resource Progression System

Manages currencies and materials that persist between game sessions.

```csharp
public class ResourceProgressionSystem : IProgressionSystem
{
    private readonly IMetaProgressionDataProvider m_DataProvider;
    private ResourceData m_ResourceData;
    
    public string SystemID => "Resources";
    
    public ResourceProgressionSystem(IMetaProgressionDataProvider dataProvider)
    {
        m_DataProvider = dataProvider;
        Initialize();
    }
    
    public void Initialize()
    {
        m_ResourceData = m_DataProvider.GetSystemData<ResourceData>(SystemID) ?? new ResourceData();
    }
    
    public void SaveProgress()
    {
        m_DataProvider.SaveSystemData(SystemID, m_ResourceData);
    }
    
    public void ResetProgress()
    {
        m_ResourceData = new ResourceData();
        SaveProgress();
    }
    
    public int GetCurrency(string currencyType)
    {
        switch(currencyType.ToLower())
        {
            case "gold": return m_ResourceData.Gold;
            case "gems": return m_ResourceData.Gems;
            default: 
                if (m_ResourceData.Materials.TryGetValue(currencyType, out int amount))
                    return amount;
                return 0;
        }
    }
    
    public void AddCurrency(string currencyType, int amount)
    {
        if (amount <= 0) return;
        
        switch(currencyType.ToLower())
        {
            case "gold":
                m_ResourceData.Gold += amount;
                break;
            case "gems":
                m_ResourceData.Gems += amount;
                break;
            default:
                if (!m_ResourceData.Materials.ContainsKey(currencyType))
                    m_ResourceData.Materials[currencyType] = 0;
                m_ResourceData.Materials[currencyType] += amount;
                break;
        }
        
        // Notify listeners about currency change
        GameEvents.RaiseCurrencyChanged(currencyType, GetCurrency(currencyType));
    }
    
    public bool SpendCurrency(string currencyType, int amount)
    {
        if (amount <= 0) return true;
        
        int currentAmount = GetCurrency(currencyType);
        if (currentAmount < amount) return false;
        
        switch(currencyType.ToLower())
        {
            case "gold":
                m_ResourceData.Gold -= amount;
                break;
            case "gems":
                m_ResourceData.Gems -= amount;
                break;
            default:
                if (m_ResourceData.Materials.ContainsKey(currencyType))
                    m_ResourceData.Materials[currencyType] -= amount;
                break;
        }
        
        // Notify listeners about currency change
        GameEvents.RaiseCurrencyChanged(currencyType, GetCurrency(currencyType));
        return true;
    }
}
```

### 2. Unlockables Progression System

Manages items, abilities, maps, and other content that can be unlocked.

```csharp
public class UnlockablesProgressionSystem : IProgressionSystem
{
    private readonly IMetaProgressionDataProvider m_DataProvider;
    private UnlockablesData m_UnlockablesData;
    
    public string SystemID => "Unlockables";
    
    public UnlockablesProgressionSystem(IMetaProgressionDataProvider dataProvider)
    {
        m_DataProvider = dataProvider;
        Initialize();
    }
    
    public void Initialize()
    {
        m_UnlockablesData = m_DataProvider.GetSystemData<UnlockablesData>(SystemID) ?? new UnlockablesData();
    }
    
    public void SaveProgress()
    {
        m_DataProvider.SaveSystemData(SystemID, m_UnlockablesData);
    }
    
    public void ResetProgress()
    {
        m_UnlockablesData = new UnlockablesData();
        SaveProgress();
    }
    
    public bool IsUnlocked(string itemId, UnlockableType type)
    {
        switch(type)
        {
            case UnlockableType.Item:
                return m_UnlockablesData.UnlockedItems.Contains(itemId);
            case UnlockableType.Ability:
                return m_UnlockablesData.UnlockedAbilities.Contains(itemId);
            case UnlockableType.Map:
                return m_UnlockablesData.UnlockedMaps.Contains(itemId);
            default:
                return false;
        }
    }
    
    public void Unlock(string itemId, UnlockableType type)
    {
        bool wasUnlocked = false;
        
        switch(type)
        {
            case UnlockableType.Item:
                wasUnlocked = m_UnlockablesData.UnlockedItems.Contains(itemId);
                m_UnlockablesData.UnlockedItems.Add(itemId);
                break;
            case UnlockableType.Ability:
                wasUnlocked = m_UnlockablesData.UnlockedAbilities.Contains(itemId);
                m_UnlockablesData.UnlockedAbilities.Add(itemId);
                break;
            case UnlockableType.Map:
                wasUnlocked = m_UnlockablesData.UnlockedMaps.Contains(itemId);
                m_UnlockablesData.UnlockedMaps.Add(itemId);
                break;
        }
        
        if (!wasUnlocked)
        {
            // Notify listeners about unlocked item
            GameEvents.RaiseItemUnlocked(itemId, type);
        }
    }
    
    public HashSet<string> GetAllUnlocked(UnlockableType type)
    {
        switch(type)
        {
            case UnlockableType.Item:
                return new HashSet<string>(m_UnlockablesData.UnlockedItems);
            case UnlockableType.Ability:
                return new HashSet<string>(m_UnlockablesData.UnlockedAbilities);
            case UnlockableType.Map:
                return new HashSet<string>(m_UnlockablesData.UnlockedMaps);
            default:
                return new HashSet<string>();
        }
    }
}

public enum UnlockableType
{
    Item,
    Ability,
    Map
}
```

### 3. Class Progression System

Manages character classes, their levels and abilities.

```csharp
public class ClassProgressionSystem : IProgressionSystem
{
    private readonly IMetaProgressionDataProvider m_DataProvider;
    private ClassData m_ClassData;
    
    public string SystemID => "Classes";
    
    public ClassProgressionSystem(IMetaProgressionDataProvider dataProvider)
    {
        m_DataProvider = dataProvider;
        Initialize();
    }
    
    public void Initialize()
    {
        m_ClassData = m_DataProvider.GetSystemData<ClassData>(SystemID) ?? new ClassData();
    }
    
    public void SaveProgress()
    {
        m_DataProvider.SaveSystemData(SystemID, m_ClassData);
    }
    
    public void ResetProgress()
    {
        m_ClassData = new ClassData();
        SaveProgress();
    }
    
    public string GetActiveClass()
    {
        return m_ClassData.ActiveClass;
    }
    
    public void SetActiveClass(string className)
    {
        if (!m_ClassData.ClassLevels.ContainsKey(className))
        {
            // New class, initialize at level 1
            m_ClassData.ClassLevels[className] = 1;
        }
        
        bool changed = m_ClassData.ActiveClass != className;
        m_ClassData.ActiveClass = className;
        
        if (changed)
        {
            // Notify listeners about class change
            GameEvents.RaiseActiveClassChanged(className, GetClassLevel(className));
        }
    }
    
    public int GetClassLevel(string className)
    {
        if (m_ClassData.ClassLevels.TryGetValue(className, out int level))
            return level;
        return 0;
    }
    
    public void IncreaseClassLevel(string className)
    {
        if (!m_ClassData.ClassLevels.ContainsKey(className))
            m_ClassData.ClassLevels[className] = 0;
            
        m_ClassData.ClassLevels[className]++;
        
        // Notify listeners about class level up
        GameEvents.RaiseClassLevelUp(className, m_ClassData.ClassLevels[className]);
    }
    
    public Dictionary<string, int> GetAllClassLevels()
    {
        return new Dictionary<string, int>(m_ClassData.ClassLevels);
    }
}
```

### 4. Building Progression System

Manages base building and structures.

```csharp
public class BuildingProgressionSystem : IProgressionSystem
{
    private readonly IMetaProgressionDataProvider m_DataProvider;
    private BuildingData m_BuildingData;
    
    public string SystemID => "Buildings";
    
    public BuildingProgressionSystem(IMetaProgressionDataProvider dataProvider)
    {
        m_DataProvider = dataProvider;
        Initialize();
    }
    
    public void Initialize()
    {
        m_BuildingData = m_DataProvider.GetSystemData<BuildingData>(SystemID) ?? new BuildingData();
    }
    
    public void SaveProgress()
    {
        m_DataProvider.SaveSystemData(SystemID, m_BuildingData);
    }
    
    public void ResetProgress()
    {
        m_BuildingData = new BuildingData();
        SaveProgress();
    }
    
    public bool IsBuildingUnlocked(string buildingId)
    {
        if (m_BuildingData.BuildingUnlocked.TryGetValue(buildingId, out bool unlocked))
            return unlocked;
        return false;
    }
    
    public void UnlockBuilding(string buildingId)
    {
        bool wasUnlocked = IsBuildingUnlocked(buildingId);
        m_BuildingData.BuildingUnlocked[buildingId] = true;
        
        if (!wasUnlocked)
        {
            // Initialize building at level 1
            m_BuildingData.BuildingLevels[buildingId] = 1;
            
            // Notify listeners about building unlocked
            GameEvents.RaiseBuildingUnlocked(buildingId);
        }
    }
    
    public int GetBuildingLevel(string buildingId)
    {
        if (!IsBuildingUnlocked(buildingId))
            return 0;
            
        if (m_BuildingData.BuildingLevels.TryGetValue(buildingId, out int level))
            return level;
        return 0;
    }
    
    public bool UpgradeBuilding(string buildingId)
    {
        if (!IsBuildingUnlocked(buildingId))
            return false;
            
        if (!m_BuildingData.BuildingLevels.ContainsKey(buildingId))
            m_BuildingData.BuildingLevels[buildingId] = 1;
        else
            m_BuildingData.BuildingLevels[buildingId]++;
            
        // Notify listeners about building upgraded
        GameEvents.RaiseBuildingUpgraded(buildingId, m_BuildingData.BuildingLevels[buildingId]);
        return true;
    }
    
    public Dictionary<string, int> GetAllBuildings()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        
        foreach (var buildingId in m_BuildingData.BuildingUnlocked.Keys)
        {
            if (m_BuildingData.BuildingUnlocked[buildingId])
                result[buildingId] = GetBuildingLevel(buildingId);
        }
        
        return result;
    }
}
```

## Integration with Existing Systems

### GameEvents Extensions

To facilitate communication between the meta-progression systems and the rest of the game, we'll extend the existing `GameEvents` class:

```csharp
// Add these to the existing GameEvents class
public static event Action<string, int> OnCurrencyChanged;
public static event Action<string, UnlockableType> OnItemUnlocked;
public static event Action<string, int> OnActiveClassChanged;
public static event Action<string, int> OnClassLevelUp;
public static event Action<string> OnBuildingUnlocked;
public static event Action<string, int> OnBuildingUpgraded;

public static void RaiseCurrencyChanged(string currencyType, int newAmount)
{
    OnCurrencyChanged?.Invoke(currencyType, newAmount);
}

public static void RaiseItemUnlocked(string itemId, UnlockableType type)
{
    OnItemUnlocked?.Invoke(itemId, type);
}

public static void RaiseActiveClassChanged(string className, int level)
{
    OnActiveClassChanged?.Invoke(className, level);
}

public static void RaiseClassLevelUp(string className, int newLevel)
{
    OnClassLevelUp?.Invoke(className, newLevel);
}

public static void RaiseBuildingUnlocked(string buildingId)
{
    OnBuildingUnlocked?.Invoke(buildingId);
}

public static void RaiseBuildingUpgraded(string buildingId, int newLevel)
{
    OnBuildingUpgraded?.Invoke(buildingId, newLevel);
}
```

### Reward System

To connect the game mechanics with meta-progression:

```csharp
public class RewardSystem : MonoBehaviour
{
    private MetaProgressionManager m_MetaProgressionManager;
    
    private void Awake()
    {
        m_MetaProgressionManager = GetComponent<MetaProgressionManager>();
    }
    
    private void OnEnable()
    {
        // Subscribe to relevant game events
        GameEvents.OnMineDefeated += HandleMineDefeated;
        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from game events
        GameEvents.OnMineDefeated -= HandleMineDefeated;
        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
    }
    
    private void HandleMineDefeated(Vector2Int position, MineType type, MonsterType? monsterType)
    {
        // Award resources based on mine/monster type
        var resourceSystem = m_MetaProgressionManager.GetSystem<ResourceProgressionSystem>();
        
        if (monsterType.HasValue)
        {
            switch (monsterType.Value)
            {
                case MonsterType.Goblin:
                    resourceSystem.AddCurrency("gold", Random.Range(1, 5));
                    break;
                case MonsterType.Orc:
                    resourceSystem.AddCurrency("gold", Random.Range(3, 8));
                    break;
                case MonsterType.Dragon:
                    resourceSystem.AddCurrency("gold", Random.Range(10, 20));
                    resourceSystem.AddCurrency("gems", Random.Range(1, 3));
                    break;
                // Add more monster types and rewards as needed
            }
        }
        else
        {
            // Basic reward for non-monster mines
            resourceSystem.AddCurrency("gold", 1);
        }
    }
    
    private void HandleLevelCompleted(int level, bool success, float completionTime)
    {
        if (!success) return;
        
        var resourceSystem = m_MetaProgressionManager.GetSystem<ResourceProgressionSystem>();
        var unlockablesSystem = m_MetaProgressionManager.GetSystem<UnlockablesProgressionSystem>();
        var classSystem = m_MetaProgressionManager.GetSystem<ClassProgressionSystem>();
        
        // Base rewards for completing a level
        int goldReward = 10 * level;
        int gemsReward = Mathf.FloorToInt(level / 3f);
        
        resourceSystem.AddCurrency("gold", goldReward);
        resourceSystem.AddCurrency("gems", gemsReward);
        
        // Potentially unlock new content based on level
        if (level >= 5 && !unlockablesSystem.IsUnlocked("advanced_maps", UnlockableType.Map))
        {
            unlockablesSystem.Unlock("advanced_maps", UnlockableType.Map);
        }
        
        if (level >= 10 && !unlockablesSystem.IsUnlocked("warrior_class", UnlockableType.Ability))
        {
            unlockablesSystem.Unlock("warrior_class", UnlockableType.Ability);
        }
        
        // Give experience to current class
        string activeClass = classSystem.GetActiveClass();
        if (level % 3 == 0)
        {
            classSystem.IncreaseClassLevel(activeClass);
        }
        
        // Save all progress after awarding rewards
        m_MetaProgressionManager.SaveProgress();
    }
}
```

## Usage Example

Here's an example of how to use the meta-progression system in the game:

```csharp
// In a shop or upgrade UI
public class ShopController : MonoBehaviour
{
    [SerializeField] private MetaProgressionManager m_ProgressionManager;
    
    public void PurchaseItem(ShopItemData itemData)
    {
        var resourceSystem = m_ProgressionManager.GetSystem<ResourceProgressionSystem>();
        var unlockablesSystem = m_ProgressionManager.GetSystem<UnlockablesProgressionSystem>();
        
        // Check if player has enough currency
        if (resourceSystem.SpendCurrency(itemData.CurrencyType, itemData.Price))
        {
            // Unlock the item
            unlockablesSystem.Unlock(itemData.ItemID, itemData.Type);
            
            // Apply any immediate effects
            ApplyItemEffects(itemData);
            
            // Save progress
            m_ProgressionManager.SaveProgress();
        }
        else
        {
            // Show "not enough currency" message
            Debug.Log("Not enough " + itemData.CurrencyType);
        }
    }
    
    private void ApplyItemEffects(ShopItemData itemData)
    {
        // Implement immediate effects of purchased item
        switch (itemData.ItemID)
        {
            case "health_upgrade":
                // Increase player max health
                break;
            case "mine_detector":
                // Enable mine detection ability
                break;
            case "extra_lives":
                // Add extra lives
                break;
        }
    }
}
```

## Conclusion

This meta-progression framework provides a flexible and extensible foundation for implementing persistent progression in the game. It follows SOLID principles and uses a facade pattern to provide a clean interface to the rest of the game.

The framework can be easily extended with new progression systems without modifying existing code, and each component has a clear responsibility. Integration with the existing game systems is accomplished through GameEvents and dedicated connectors like the RewardSystem.

As the game evolves, new progression systems, unlockables, and rewards can be added to enhance the player's meta-progression experience.