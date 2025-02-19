## SpecStory

## Implementing Strategy Pattern for Mine Spawning (2/12/2025, 7:32:53 AM)

_**User**_

based on @Principle.md, I would like to implement a strategy pattern for handling how mines are spawned, the first one is the default random, the second one is spawned on the edge, the option is introduced via @MineData.cs 

---

_**Assistant**_

I'll help you implement a strategy pattern for mine spawning. Let's break this down step by step:

1. First, we'll create an interface for the spawn strategy
2. Create concrete implementations for random and edge spawning
3. Modify MineData to include spawn strategy selection
4. Implement the strategies

Let me search for any existing spawn-related code first:

---

_****_

<details>
            <summary>Searched codebase "mine spawn spawning placement" • **17** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Managers/MineManager.cs` | L123-168 |
| `Assets/Scripts/Core/Mines/StandardMine.cs` | L1-66 |
| `Assets/Scripts/Core/Mines/MineType.cs` | L1-10 |
| `Assets/Scripts/Managers/MineManager.cs` | L1-64 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L1-20 |
| `Assets/Scripts/Core/Mines/MineShapeHelper.cs` | L1-50 |
| `Assets/Scripts/Core/Mines/MineShape.cs` | L1-8 |
| `Assets/Scripts/Managers/MineManager.cs` | L64-123 |
| `Assets/Scripts/Core/Mines/MineShapeHelper.cs` | L50-91 |
| `Assets/Scripts/Core/Mines/MineValuePropagator.cs` | L59-93 |
| `Assets/Scripts/Core/Mines/IMine.cs` | L1-9 |
| `Assets/Scripts/Views/CellView.cs` | L1-61 |
| `Assets/Scripts/Core/Mines/MineValuePropagator.cs` | L1-59 |
| `Assets/Scripts/Views/CellView.cs` | L61-134 |
| `Assets/Scripts/Core/Mines/MineShapeHelper.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Events/GameEvents.cs` | L1-36 |
| `Assets/Scripts/Core/Mines/MineShape.cs.meta` | L1-2 |

</details>

---

_****_



Based on the codebase search, I can see the current implementation and will now implement the strategy pattern for mine spawning. Here's the step-by-step plan:

1. Create an interface for spawn strategies
2. Create concrete implementations for random and edge spawning
3. Modify MineData to include spawn strategy selection
4. Update MineManager to use the strategy pattern

Let's implement this following the principles from the document:

1. First, let's create the spawn strategy interface and enum:

---

_****_

```diff
- 
+ public interface IMineSpawnStrategy
+ {
+     Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines);
+ }
+ 
+ public enum MineSpawnStrategyType
+ {
+     Random,
+     Edge
+ } 
```

---

_****_



2. Now let's create the concrete strategy implementations:

---

_****_

```diff
- 
+ using UnityEngine;
+ using System.Collections.Generic;
+ 
+ public class RandomMineSpawnStrategy : IMineSpawnStrategy
+ {
+     public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
+     {
+         Vector2Int position;
+         do
+         {
+             position = new Vector2Int(
+                 Random.Range(0, gridManager.Width),
+                 Random.Range(0, gridManager.Height)
+             );
+         } while (existingMines.ContainsKey(position));
+ 
+         return position;
+     }
+ }
+ 
+ public class EdgeMineSpawnStrategy : IMineSpawnStrategy
+ {
+     public Vector2Int GetSpawnPosition(GridManager gridManager, Dictionary<Vector2Int, IMine> existingMines)
+     {
+         List<Vector2Int> edgePositions = new List<Vector2Int>();
+         
+         // Add top and bottom edges
+         for (int x = 0; x < gridManager.Width; x++)
+         {
+             edgePositions.Add(new Vector2Int(x, 0));
+             edgePositions.Add(new Vector2Int(x, gridManager.Height - 1));
+         }
+         
+         // Add left and right edges (excluding corners already added)
+         for (int y = 1; y < gridManager.Height - 1; y++)
+         {
+             edgePositions.Add(new Vector2Int(0, y));
+             edgePositions.Add(new Vector2Int(gridManager.Width - 1, y));
+         }
+         
+         // Filter out positions that already have mines
+         edgePositions.RemoveAll(pos => existingMines.ContainsKey(pos));
+         
+         // If no edge positions are available, fallback to random position
+         if (edgePositions.Count == 0)
+         {
+             return new RandomMineSpawnStrategy().GetSpawnPosition(gridManager, existingMines);
+         }
+         
+         // Return random edge position
+         return edgePositions[Random.Range(0, edgePositions.Count)];
+     }
+ } 
```

---

_****_



3. Now let's modify MineData to include the spawn strategy:

---

_****_

```diff
      public MineShape Shape;
      public int Radius;
+ 
+     [Header("Spawn Properties")]
+     public MineSpawnStrategyType SpawnStrategy = MineSpawnStrategyType.Random;
  
      [Header("Visual Properties")]
```

---

_****_



Let me fix the linter error by adding the correct using statement:

---

_****_



---

_****_







4. Finally, let's update the MineManager to use the strategy pattern:

---

_****_

```diff
      private Dictionary<Vector2Int, IMine> m_Mines = new Dictionary<Vector2Int, IMine>();
      private Dictionary<Vector2Int, MineData> m_MineDataMap = new Dictionary<Vector2Int, MineData>();
- 
-     #if UNITY_EDITOR
-     public IReadOnlyDictionary<Vector2Int, IMine> GetMines() => m_Mines;
-     #endif
- 
-     public bool HasMineAt(Vector2Int position) => m_Mines.ContainsKey(position);
+     private Dictionary<MineSpawnStrategyType, IMineSpawnStrategy> m_SpawnStrategies;
+ 
+     private void Awake()
+     {
+         InitializeSpawnStrategies();
+     }
  
-     public MineData GetMineDataAt(Vector2Int position)
+     private void InitializeSpawnStrategies()
      {
-         if (m_MineDataMap.TryGetValue(position, out MineData mineData))
+         m_SpawnStrategies = new Dictionary<MineSpawnStrategyType, IMineSpawnStrategy>
          {
-             return mineData;
-         }
-         return null;
+             { MineSpawnStrategyType.Random, new RandomMineSpawnStrategy() },
+             { MineSpawnStrategyType.Edge, new EdgeMineSpawnStrategy() }
+         };
      }
  
          if (m_Mines.TryGetValue(position, out IMine mine))
          {
-             
              // Show mine sprite
              if (m_MineDataMap.TryGetValue(position, out MineData mineData))
          {
              MineData randomMineData = m_MineDatas[Random.Range(0, m_MineDatas.Count)];
-             Vector2Int position = GetRandomEmptyPosition();
+             Vector2Int position = GetSpawnPosition(randomMineData);
              
              IMine mine = CreateMine(randomMineData, position);
      }
  
-     private Vector2Int GetRandomEmptyPosition()
+     private Vector2Int GetSpawnPosition(MineData mineData)
      {
-         Vector2Int position;
-         do
-         {
-             position = new Vector2Int(
-                 Random.Range(0, m_GridManager.Width),
-                 Random.Range(0, m_GridManager.Height)
-             );
-         } while (m_Mines.ContainsKey(position));
- 
-         return position;
+         if (m_SpawnStrategies.TryGetValue(mineData.SpawnStrategy, out IMineSpawnStrategy strategy))
+         {
+             return strategy.GetSpawnPosition(m_GridManager, m_Mines);
+         }
+         
+         Debug.LogWarning($"MineManager: No spawn strategy found for {mineData.SpawnStrategy}, falling back to random strategy");
+         return m_SpawnStrategies[MineSpawnStrategyType.Random].GetSpawnPosition(m_GridManager, m_Mines);
      }
  
          MineValuePropagator.PropagateValues(this, m_GridManager);
      }
+ 
+     #if UNITY_EDITOR
+     public IReadOnlyDictionary<Vector2Int, IMine> GetMines() => m_Mines;
+     #endif
+ 
+     public bool HasMineAt(Vector2Int position) => m_Mines.ContainsKey(position);
+ 
+     public MineData GetMineDataAt(Vector2Int position)
+     {
+         if (m_MineDataMap.TryGetValue(position, out MineData mineData))
+         {
+             return mineData;
+         }
+         return null;
+     }
  } 
```

---

_****_







Let me fix the linter errors by adding the correct using statement for Vector2Int:

---

_****_

```diff
+ using UnityEngine;
+ using System.Collections.Generic;
+ 
  public interface IMineSpawnStrategy
  {
```

---

_****_







Now I'll summarize what we've implemented:

1. Created a strategy pattern for mine spawning with:
   - `IMineSpawnStrategy` interface
   - `MineSpawnStrategyType` enum
   - Two concrete strategies: `RandomMineSpawnStrategy` and `EdgeMineSpawnStrategy`

2. Modified `MineData` to include spawn strategy selection with a default of Random

3. Updated `MineManager` to:
   - Initialize and store spawn strategies
   - Use the appropriate strategy based on mine data
   - Fall back to random strategy if needed

The implementation follows the principles from the document:
- Single Responsibility Principle: Each strategy class has one job
- Open/Closed Principle: New spawn strategies can be added without modifying existing code
- Interface Segregation: The interface is focused and minimal
- Dependency Inversion: High-level modules depend on abstractions

To use this in your game:
1. In the Unity Inspector, you can now select either Random or Edge spawn strategy for each mine type in your MineData assets
2. The mines will be placed according to their specified strategy during game initialization
3. If you want to add new spawn strategies in the future, just:
   - Add a new type to `MineSpawnStrategyType`
   - Create a new class implementing `IMineSpawnStrategy`
   - Add it to the strategies dictionary in `MineManager.InitializeSpawnStrategies()`

Would you like me to make any adjustments to this implementation?

---

_**User**_

@generate git commit etc. 

---

_**Assistant**_

I'll help generate the git commit, changelog entry, and architecture updates based on our implementation of the mine spawn strategy pattern.

1. First, let's create the git commit message following F_Commit.md format:

```
refactor: Implement strategy pattern for mine spawning

Why:
- Improve extensibility of mine placement logic
- Allow different mine types to have different spawn behaviors
- Follow SOLID principles for better maintainability

What:
- Created IMineSpawnStrategy interface for spawn behavior
- Implemented RandomMineSpawnStrategy and EdgeMineSpawnStrategy
- Added spawn strategy selection to MineData
- Updated MineManager to use strategy pattern
- Maintained backward compatibility with default random spawning
```

2. Now let's create the changelog entry to add to the top of Changelog.md:

---

_****_

```diff
+ # v0.1.5 - 2025-02-12 04:45:00
+ ## Overview
+ Implemented strategy pattern for mine spawning system, enhancing flexibility and extensibility of mine placement mechanics.
+ 
+ ## Change Details
+ ### Adjustments and Refactoring
+ #### Mine Spawn System
+ - Introduced strategy pattern for mine spawning
+ - Added random and edge spawn strategies
+ - Enhanced MineData with spawn strategy selection
+ ```mermaid
+ classDiagram
+     class IMineSpawnStrategy {
+         <<interface>>
+         +GetSpawnPosition(GridManager, Dictionary)
+     }
+     class RandomMineSpawnStrategy {
+         +GetSpawnPosition(GridManager, Dictionary)
+     }
+     class EdgeMineSpawnStrategy {
+         +GetSpawnPosition(GridManager, Dictionary)
+     }
+     class MineData {
+         +SpawnStrategy: MineSpawnStrategyType
+     }
+     class MineManager {
+         -m_SpawnStrategies: Dictionary
+         +GetSpawnPosition(MineData)
+     }
+     IMineSpawnStrategy <|.. RandomMineSpawnStrategy
+     IMineSpawnStrategy <|.. EdgeMineSpawnStrategy
+     MineManager --> IMineSpawnStrategy
+     MineData --> MineSpawnStrategyType
+ ```
+ 
+ ### Architecture Improvements
+ - Enhanced modularity of mine spawning system
+ - Improved extensibility for future spawn strategies
+ - Better separation of concerns in mine placement logic
+ 
  # v0.1.4 - 2025-02-12 04:17:50
  ## Overview
```

---

_****_



3. Finally, let's update the Architecture.md to include the new spawn strategy system:

---

_****_

```diff
  #CalculateDamage(Player)
  #ApplyEffect(EffectData)
+ }
+ class IMineSpawnStrategy {
+ <<interface>>
+ +GetSpawnPosition(GridManager, Dictionary)
+ }
+ class RandomMineSpawnStrategy {
+ +GetSpawnPosition(GridManager, Dictionary)
+ }
+ class EdgeMineSpawnStrategy {
+ +GetSpawnPosition(GridManager, Dictionary)
+ }
+ class MineData {
+ +Type: MineType
+ +Value: int
+ +Shape: MineShape
+ +SpawnStrategy: MineSpawnStrategyType
  }
  class HealingMine {
  class MineManager {
  -m_Mines: Dictionary
+ -m_SpawnStrategies: Dictionary
  +HandleMineRemoval(Vector2Int)
  +HasMineAt(Vector2Int)
+ -GetSpawnPosition(MineData)
  }
  IMine <|.. BaseMine
```

---

_****_







The changes have been made to implement the strategy pattern for mine spawning and update all relevant documentation. The implementation follows the SOLID principles and maintains good separation of concerns. The changes are well-documented in both the changelog and architecture documentation.

Would you like me to make any adjustments to these changes or explain any part in more detail?

---



---

