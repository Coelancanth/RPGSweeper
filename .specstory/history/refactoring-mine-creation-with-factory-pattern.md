## SpecStory

## Refactoring Mine Creation with Factory Pattern (2/19/2025, 10:32:27 PM)

_**User**_

Now I would like to refactor @MineManager.cs, please stick to SOLID principle, the first step is:

Extract Mine Creation Logic into a MineFactory Class

What to extract:
• The CreateMine method (the switch‐based mine instantiation)
Plan:
• Create a new class called MineFactory (or IMineFactory interface plus concrete implementation).
• Move the switch statement that selects between StandardMine and MonsterMine into a static or instance method, for example:
public static IMine CreateMine(MineData mineData, Vector2Int position)
• Update MineManager to call MineFactory.CreateMine instead of its internal CreateMine.


---

_**Assistant**_

I'll help you refactor the `MineManager.cs` to extract the mine creation logic into a separate `MineFactory` class following SOLID principles. Let me break this down step by step:

1. First, I'll create a new `MineFactory` class:

---

_****_

```diff
  using UnityEngine;
- 
- public static class MineFactory
+ using RPGMinesweeper;
+ 
+ namespace RPGMinesweeper.Factory
+ {
+     public interface IMineFactory
+     {
+         IMine CreateMine(MineData mineData, Vector2Int position);
+     }
+ 
+     public class MineFactory : IMineFactory
      {
-     public static IMine CreateMine(MineData mineData, Vector2Int position)
+         public IMine CreateMine(MineData mineData, Vector2Int position)
          {
-         switch (mineData.Type)
+             return mineData.Type switch
              {
-             case MineType.Standard:
-                 return new StandardMine(mineData, position);
-             case MineType.Monster:
+                 MineType.Standard => new StandardMine(mineData, position),
+                 MineType.Monster => CreateMonsterMine(mineData, position),
+                 _ => HandleUnknownMineType(mineData, position)
+             };
+         }
+ 
+         private IMine CreateMonsterMine(MineData mineData, Vector2Int position)
+         {
              if (mineData is MonsterMineData monsterData)
              {
                  return new MonsterMine(monsterData, position);
              }
-                 Debug.LogError("MineFactory: MineData for Monster type must be MonsterMineData!");
+             
+             Debug.LogError($"MineManager: MineData for Monster type must be MonsterMineData!");
              return new StandardMine(mineData, position);
-             default:
-                 Debug.LogWarning($"MineFactory: Mine type {mineData.Type} not implemented yet. Using StandardMine as fallback.");
+         }
+ 
+         private IMine HandleUnknownMineType(MineData mineData, Vector2Int position)
+         {
+             Debug.LogWarning($"Mine type {mineData.Type} not implemented yet. Using StandardMine as fallback.");
              return new StandardMine(mineData, position);
          }
```

---

_****_



Now, I'll update the `MineManager` to use the new `MineFactory`:

---

_****_

```diff
  using RPGMinesweeper;  // For MonsterType and CompositeSpawnStrategy
  using RPGMinesweeper.Grid;  // For GridShape
+ using RPGMinesweeper.Factory;  // For MineFactory
  
  [System.Serializable]
      private Dictionary<Vector2Int, MineData> m_MineDataMap = new Dictionary<Vector2Int, MineData>();
      private Dictionary<MineSpawnStrategyType, CompositeSpawnStrategy> m_SpawnStrategies;
+     private IMineFactory m_MineFactory;
  
      private void Awake()
      {
          InitializeSpawnStrategies();
+         m_MineFactory = new MineFactory();
      }
  
          }
  
-         // Create and add the new mine using the existing CreateMine method
-         IMine mine = CreateMine(mineData, position);
+         // Create and add the new mine using MineFactory
+         IMine mine = m_MineFactory.CreateMine(mineData, position);
          m_Mines[position] = mine;
          m_MineDataMap[position] = mineData;
                  Vector2Int position = GetSpawnPosition(spawnData.MineData);
                  
-                 IMine mine = CreateMine(spawnData.MineData, position);
+                 IMine mine = m_MineFactory.CreateMine(spawnData.MineData, position);
                  m_Mines.Add(position, mine);
                  m_MineDataMap.Add(position, spawnData.MineData);
          return strategy.GetSpawnPosition(m_GridManager, m_Mines);
      }
- 
-     private IMine CreateMine(MineData mineData, Vector2Int position)
-     {
-         switch (mineData.Type)
-         {
-             case MineType.Standard:
-                 return new StandardMine(mineData, position);
-             case MineType.Monster:
-                 if (mineData is MonsterMineData monsterData)
-                 {
-                     return new MonsterMine(monsterData, position);
-                 }
-                 Debug.LogError($"MineManager: MineData for Monster type must be MonsterMineData!");
-                 return new StandardMine(mineData, position);
-             default:
-                 Debug.LogWarning($"Mine type {mineData.Type} not implemented yet. Using StandardMine as fallback.");
-                 return new StandardMine(mineData, position);
-         }
-     }
  
      private void UpdateAllGridValues()
          var sourceMineData = m_MineDataMap[_sourcePosition];
  
-         // Create and place the new mine
-         IMine newMine = CreateMine(sourceMineData, _targetPosition);
+         // Create and place the new mine using MineFactory
+         IMine newMine = m_MineFactory.CreateMine(sourceMineData, _targetPosition);
          
          // Copy mine-type specific properties
```

---

_****_



I've completed the first step of the refactoring process by:

1. Creating a new `MineFactory.cs` file with:
   - An `IMineFactory` interface for dependency injection and better testability
   - A concrete `MineFactory` class implementing the interface
   - Extracted mine creation logic into separate methods for better organization
   - Used pattern matching with switch expression for cleaner code

2. Updated `MineManager.cs` to:
   - Add the `MineFactory` namespace
   - Add a private field for the `IMineFactory`
   - Initialize the factory in `Awake()`
   - Remove the old `CreateMine` method
   - Update all mine creation calls to use the factory

The changes follow SOLID principles by:
- Single Responsibility: Mine creation is now handled by a dedicated factory class
- Open/Closed: New mine types can be added by extending the factory without modifying existing code
- Liskov Substitution: The `IMineFactory` interface allows for different implementations
- Interface Segregation: The factory interface is focused on a single responsibility
- Dependency Inversion: `MineManager` depends on the `IMineFactory` abstraction

Would you like to proceed with any other refactoring steps?

---



---

