## SpecStory

## Implementing Disguise Effect for Mimic Chest (2/19/2025, 5:41:33 PM)

_**User**_



---

_**Assistant**_

I'll help you implement a "disguise" effect following the principles and implementation process. Let me break this down step by step.

First, let's plan out the DisguiseEffect:

**Pseudocode Plan:**
1. Core Concept:
   - Effect that makes a mine appear as a different type temporarily
   - Supports persistent mode (stays disguised for X turns)
   - Needs state management to track disguise status
   - Should affect visual representation of the mine

2. Components Needed:
   ```
   - DisguiseEffect.cs (main effect implementation)
   - DisguiseEffectData.cs (configuration)
   - DisguiseState.cs (state management)
   ```

3. Key Properties:
   ```
   - Duration (how long disguise lasts)
   - DisguiseType (what mine type to appear as)
   - Visual indicators for disguised state
   ```

4. Behavior Flow:
   ```
   1. Effect applied to mine
   2. State created and registered
   3. Mine appearance changed
   4. Duration tracked per turn
   5. Revert on completion
   ```

Let me start implementing this by first searching for relevant code to understand the integration points.

---

_****_

<details>
            <summary>Searched codebase "BaseEffect implementation and state management" • **25** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/Effects/BaseEffect.cs` | L1-51 |
| `Assets/Scripts/Core/Effects/IEffect.cs` | L1-46 |
| `Assets/Scripts/Core/Effects/UnfreezeEffect.cs` | L1-60 |
| `Assets/Scripts/Core/Effects/FreezeEffect.cs` | L1-58 |
| `Assets/Scripts/Core/Effects/EffectData.cs` | L1-70 |
| `Assets/Scripts/Core/Effects/BaseEffect.cs` | L51-99 |
| `Assets/Scripts/Core/Effects/ConfusionEffect.cs` | L1-52 |
| `Assets/Scripts/Core/Effects/UnfreezeEffect.cs` | L60-106 |
| `Assets/Scripts/Core/Effects/FreezeEffect.cs` | L58-100 |
| `Assets/Scripts/Core/Effects/MonsterTransformEffect.cs` | L1-46 |
| `Assets/Scripts/Core/Effects/BaseEffect.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Effects/SplitEffect.cs` | L1-54 |
| `Assets/Scripts/Core/Effects/SummonEffect.cs` | L1-41 |
| `Assets/Scripts/Core/Effects/FreezeEffectData.cs` | L1-17 |
| `Assets/Scripts/Core/Effects/RangeRevealEffect.cs` | L1-50 |
| `Assets/Scripts/Core/Effects/ConfusionEffectData.cs` | L1-17 |
| `Assets/Scripts/Core/Effects/TargetedRevealEffect.cs` | L1-31 |
| `Assets/Scripts/Core/Effects/MonsterTransformEffectData.cs` | L1-50 |
| `Assets/Scripts/Core/Effects/UnfreezeEffectData.cs` | L1-17 |
| `Assets/Scripts/Core/Effects/TargetedRevealEffectData.cs` | L1-27 |
| `Assets/Scripts/Core/Effects/ConfusionEffect.cs` | L52-79 |
| `Assets/Scripts/Core/Effects/RangeRevealEffectData.cs` | L1-37 |
| `Assets/Scripts/Core/Effects/MonsterTransformEffect.cs` | L46-97 |
| `Assets/Scripts/Core/Effects/SplitEffect.cs` | L54-84 |
| `Assets/Scripts/Core/Effects/SummonEffectData.cs` | L1-62 |

</details>

---

_****_



Based on the search results, I'll now implement the DisguiseEffect system. Let me first create the effect class and its data class.

---

_****_

```diff
- 
+ using UnityEngine;
+ using RPGMinesweeper.Grid;
+ using RPGMinesweeper.States;
+ using RPGMinesweeper;
+ 
+ namespace RPGMinesweeper.Effects
+ {
+     public class DisguiseEffect : BaseEffect, IPersistentEffect, ITriggerableEffect
+     {
+         #region Private Fields
+         private readonly int m_Duration;
+         private readonly MineType m_DisguiseType;
+         private readonly MonsterType m_DisguiseMonsterType;
+         private bool m_IsActive;
+         private StateManager m_StateManager;
+         private Vector2Int m_CurrentPosition;
+         private bool m_DebugMode = false;
+         #endregion
+ 
+         #region Public Properties
+         public override EffectType[] SupportedTypes => new[] { EffectType.Persistent, EffectType.Triggerable };
+         public bool IsActive => m_IsActive;
+         public EffectType Type => EffectType.Triggerable;
+         public string Name => "Disguise";
+         #endregion
+ 
+         public DisguiseEffect(int duration, MineType disguiseType, MonsterType disguiseMonsterType = MonsterType.None)
+         {
+             m_Duration = duration;
+             m_DisguiseType = disguiseType;
+             m_DisguiseMonsterType = disguiseMonsterType;
+             m_IsActive = false;
+             m_CurrentMode = EffectType.Persistent;
+         }
+ 
+         #region Protected Methods
+         protected override void ApplyPersistent(GameObject target, Vector2Int sourcePosition)
+         {
+             m_IsActive = true;
+             EnsureStateManager();
+             ApplyDisguiseState(target, sourcePosition);
+         }
+ 
+         protected override void ApplyTriggerable(GameObject target, Vector2Int sourcePosition)
+         {
+             m_IsActive = true;
+             EnsureStateManager();
+             ApplyDisguiseState(target, sourcePosition);
+         }
+         #endregion
+ 
+         #region Public Methods
+         public void Update(float deltaTime)
+         {
+             // State updates are handled by StateManager
+         }
+ 
+         public void Remove(GameObject target)
+         {
+             if (m_StateManager != null && m_IsActive)
+             {
+                 m_StateManager.RemoveState(("Disguise", StateTarget.Cell, m_CurrentPosition));
+                 m_IsActive = false;
+             }
+         }
+         #endregion
+ 
+         #region Private Methods
+         private void EnsureStateManager()
+         {
+             if (m_StateManager == null)
+             {
+                 m_StateManager = GameObject.FindFirstObjectByType<StateManager>();
+                 
+                 if (m_StateManager == null)
+                 {
+                     var stateManagerObj = new GameObject("StateManager");
+                     m_StateManager = stateManagerObj.AddComponent<StateManager>();
+                     if (m_DebugMode)
+                     {
+                         Debug.Log("[DisguiseEffect] Created new StateManager");
+                     }
+                 }
+             }
+         }
+ 
+         private void ApplyDisguiseState(GameObject target, Vector2Int sourcePosition)
+         {
+             m_CurrentPosition = sourcePosition;
+             var disguiseState = new DisguiseState(m_Duration, sourcePosition, m_DisguiseType, m_DisguiseMonsterType);
+             m_StateManager.AddState(disguiseState);
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[DisguiseEffect] Applied disguise state at {sourcePosition} with duration {m_Duration}");
+             }
+         }
+         #endregion
+     }
+ } 
```

---

_****_



Now let's create the DisguiseEffectData class:

---

_****_

```diff
- 
+ using UnityEngine;
+ using RPGMinesweeper.Grid;
+ using RPGMinesweeper.Effects;
+ using RPGMinesweeper;
+ using Sirenix.OdinInspector;
+ 
+ namespace RPGMinesweeper.Effects
+ {
+     [CreateAssetMenu(fileName = "DisguiseEffectData", menuName = "RPGMinesweeper/Effects/DisguiseEffect")]
+     public class DisguiseEffectData : EffectData
+     {
+         [Header("Disguise Properties")]
+         [Tooltip("Type of mine to disguise as")]
+         [SerializeField]
+         private MineType m_DisguiseType = MineType.Standard;
+ 
+         [Tooltip("Type of monster to disguise as (if disguise type is Monster)")]
+         [SerializeField, ShowIf("m_DisguiseType", MineType.Monster)]
+         private MonsterType m_DisguiseMonsterType = MonsterType.None;
+ 
+         public MineType DisguiseType
+         {
+             get => m_DisguiseType;
+             set => m_DisguiseType = value;
+         }
+ 
+         public MonsterType DisguiseMonsterType
+         {
+             get => m_DisguiseMonsterType;
+             set => m_DisguiseMonsterType = value;
+         }
+ 
+         public override IEffect CreateEffect()
+         {
+             return new DisguiseEffect(Duration, m_DisguiseType, m_DisguiseMonsterType);
+         }
+     }
+ } 
```

---

_****_



Now let's create the DisguiseState class to manage the state of disguised mines:

---

_****_

```diff
- 
+ using UnityEngine;
+ using RPGMinesweeper.Grid;
+ using RPGMinesweeper.Effects;
+ using RPGMinesweeper;
+ 
+ namespace RPGMinesweeper.States
+ {
+     public class DisguiseState : BaseTurnState
+     {
+         private readonly MineType m_DisguiseType;
+         private readonly MonsterType m_DisguiseMonsterType;
+         private IMine m_OriginalMine;
+         private bool m_IsDisguised;
+ 
+         public DisguiseState(int turns, Vector2Int sourcePosition, MineType disguiseType, MonsterType disguiseMonsterType) 
+             : base("Disguise", turns, StateTarget.Cell, sourcePosition)
+         {
+             m_DisguiseType = disguiseType;
+             m_DisguiseMonsterType = disguiseMonsterType;
+             m_IsDisguised = false;
+         }
+ 
+         public override void Enter(GameObject target)
+         {
+             var mineManager = GameObject.FindFirstObjectByType<MineManager>();
+             if (mineManager == null) return;
+ 
+             var mines = mineManager.GetMines();
+             if (!mines.TryGetValue(SourcePosition, out m_OriginalMine)) return;
+ 
+             // Store original mine and apply disguise
+             ApplyDisguise(mineManager);
+         }
+ 
+         public override void Exit(GameObject target)
+         {
+             if (m_IsDisguised)
+             {
+                 var mineManager = GameObject.FindFirstObjectByType<MineManager>();
+                 if (mineManager != null)
+                 {
+                     // Restore original mine appearance
+                     RemoveDisguise(mineManager);
+                 }
+             }
+         }
+ 
+         public override void OnTurnEnd()
+         {
+             base.OnTurnEnd();
+             if (RemainingTurns <= 0)
+             {
+                 var mineManager = GameObject.FindFirstObjectByType<MineManager>();
+                 if (mineManager != null)
+                 {
+                     RemoveDisguise(mineManager);
+                 }
+             }
+         }
+ 
+         private void ApplyDisguise(MineManager mineManager)
+         {
+             if (m_IsDisguised) return;
+ 
+             // Create a temporary disguise mine
+             IMine disguiseMine = m_DisguiseType == MineType.Monster
+                 ? new MonsterMine(m_DisguiseMonsterType)
+                 : new StandardMine();
+ 
+             // Apply the disguise visually
+             var cellView = mineManager.GetCellView(SourcePosition);
+             if (cellView != null)
+             {
+                 cellView.UpdateVisuals(disguiseMine);
+                 m_IsDisguised = true;
+             }
+         }
+ 
+         private void RemoveDisguise(MineManager mineManager)
+         {
+             if (!m_IsDisguised) return;
+ 
+             // Restore original mine visuals
+             var cellView = mineManager.GetCellView(SourcePosition);
+             if (cellView != null)
+             {
+                 cellView.UpdateVisuals(m_OriginalMine);
+                 m_IsDisguised = false;
+             }
+         }
+     }
+ } 
```

---

_****_



I've implemented the complete disguise effect system following the project's principles and implementation process. Here's a summary of what each component does:

1. `DisguiseEffect.cs`:
   - Main effect implementation that handles both persistent and triggerable modes
   - Manages the duration and type of disguise
   - Integrates with the state system

2. `DisguiseEffectData.cs`:
   - ScriptableObject for configuring the disguise effect
   - Allows setting the disguise type (standard mine or monster)
   - Uses Odin Inspector for better editor integration

3. `DisguiseState.cs`:
   - Manages the state of disguised mines
   - Handles visual updates through CellView
   - Maintains original mine information
   - Properly cleans up when the effect ends

Key features:
- Supports both standard mines and monster mines as disguise targets
- Turn-based duration system
- Clean state management
- Proper cleanup on effect end
- Debug mode for troubleshooting
- Follows SOLID principles and project conventions

To use this effect:
1. Create a DisguiseEffectData ScriptableObject in the Unity editor
2. Configure the disguise type and duration
3. Apply the effect to a mine through the effect system

The effect will make a mine appear as a different type for the specified duration, then revert to its original appearance when the effect ends.

Would you like me to implement any additional features or make any adjustments to the current implementation?

---

_**User**_

Assets\Scripts\Core\States\DisguiseState.cs(29,36): error CS0103: The name 'SourcePosition' does not exist in the current context

Assets\Scripts\Core\States\DisguiseState.cs(51,17): error CS0103: The name 'RemainingTurns' does not exist in the current context

Assets\Scripts\Core\States\DisguiseState.cs(67,23): error CS7036: There is no argument given that corresponds to the required formal parameter '_position' of 'MonsterMine.MonsterMine(MonsterMineData, Vector2Int)'

Assets\Scripts\Core\States\DisguiseState.cs(68,23): error CS7036: There is no argument given that corresponds to the required formal parameter '_data' of 'StandardMine.StandardMine(MineData, Vector2Int)'

Assets\Scripts\Core\States\DisguiseState.cs(71,52): error CS0103: The name 'SourcePosition' does not exist in the current context

Assets\Scripts\Core\States\DisguiseState.cs(71,40): error CS1061: 'MineManager' does not contain a definition for 'GetCellView' and no accessible extension method 'GetCellView' accepting a first argument of type 'MineManager' could be found (are you missing a using directive or an assembly reference?)

Assets\Scripts\Core\States\DisguiseState.cs(84,52): error CS0103: The name 'SourcePosition' does not exist in the current context

Assets\Scripts\Core\States\DisguiseState.cs(84,40): error CS1061: 'MineManager' does not contain a definition for 'GetCellView' and no accessible extension method 'GetCellView' accepting a first argument of type 'MineManager' could be found (are you missing a using directive or an assembly reference?)


please fix the bugs

---

_**Assistant**_

I'll help fix these compilation errors. Let me search for the correct base class implementation and mine-related classes first.

---

_****_

<details>
            <summary>Searched codebase "BaseTurnState implementation with SourcePosition and RemainingTurns" • **20** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/States/BaseTurnState.cs` | L1-36 |
| `Assets/Scripts/Core/States/ITurnBasedState.cs` | L1-10 |
| `Assets/Scripts/Core/States/FrozenState.cs` | L1-52 |
| `Assets/Scripts/Core/States/DisguiseState.cs` | L1-60 |
| `Assets/Scripts/Core/States/BaseState.cs` | L1-51 |
| `Assets/Scripts/Core/States/FrozenState.cs` | L52-109 |
| `Assets/Scripts/Core/States/StateManager.cs` | L61-121 |
| `Assets/Scripts/Core/States/BaseTurnState.cs.meta` | L1-2 |
| `Assets/Scripts/Core/States/StateManager.cs` | L121-156 |
| `Assets/Scripts/Core/States/ITurnBasedState.cs.meta` | L1-2 |
| `Assets/Scripts/Core/States/IState.cs` | L1-16 |
| `Assets/Scripts/Core/States/DisguiseState.cs` | L60-92 |
| `Assets/Scripts/Core/States/StateTarget.cs` | L1-10 |
| `Assets/Scripts/Core/States/StateManager.cs` | L1-61 |
| `Assets/Scripts/Core/States/FrozenState.cs` | L109-135 |
| `Assets/Scripts/Core/States/BaseState.cs.meta` | L1-2 |
| `Assets/Scripts/Core/States/IState.cs.meta` | L1-2 |
| `Assets/Scripts/Core/States/StateTarget.cs.meta` | L1-2 |
| `Assets/Scripts/Core/States/FrozenState.cs.meta` | L1-2 |
| `Assets/Scripts/Core/States/FrozenState.cs.meta` | L1-2 |

</details>

---

_****_



---

_****_



Let me fix the `DisguiseState.cs` file based on the base class implementations and error messages:

---

_****_

```diff
      public class DisguiseState : BaseTurnState
      {
+         #region Private Fields
          private readonly MineType m_DisguiseType;
          private readonly MonsterType m_DisguiseMonsterType;
+         private readonly Vector2Int m_SourcePosition;
          private IMine m_OriginalMine;
          private bool m_IsDisguised;
+         private bool m_DebugMode = false;
+         #endregion
  
          public DisguiseState(int turns, Vector2Int sourcePosition, MineType disguiseType, MonsterType disguiseMonsterType) 
              m_DisguiseType = disguiseType;
              m_DisguiseMonsterType = disguiseMonsterType;
+             m_SourcePosition = sourcePosition;
              m_IsDisguised = false;
          }
  
          public override void Enter(GameObject target)
          {
+             base.Enter(target);
              var mineManager = GameObject.FindFirstObjectByType<MineManager>();
              if (mineManager == null) return;
  
              var mines = mineManager.GetMines();
-             if (!mines.TryGetValue(SourcePosition, out m_OriginalMine)) return;
+             if (!mines.TryGetValue(m_SourcePosition, out m_OriginalMine)) return;
  
              // Store original mine and apply disguise
                  }
              }
+             base.Exit(target);
          }
  
          public override void OnTurnEnd()
          {
              base.OnTurnEnd();
-             if (RemainingTurns <= 0)
+             if (TurnsRemaining <= 0)
              {
                  var mineManager = GameObject.FindFirstObjectByType<MineManager>();
              if (m_IsDisguised) return;
  
-             // Create a temporary disguise mine
-             IMine disguiseMine = m_DisguiseType == MineType.Monster
-                 ? new MonsterMine(m_DisguiseMonsterType)
-                 : new StandardMine();
+             // Get the grid manager to access cell objects
+             var gridManager = GameObject.FindFirstObjectByType<GridManager>();
+             if (gridManager == null) return;
+ 
+             // Create a temporary disguise mine with proper data and position
+             IMine disguiseMine;
+             if (m_DisguiseType == MineType.Monster)
+             {
+                 var monsterData = ScriptableObject.CreateInstance<MonsterMineData>();
+                 monsterData.MonsterType = m_DisguiseMonsterType;
+                 disguiseMine = new MonsterMine(monsterData, m_SourcePosition);
+             }
+             else
+             {
+                 var mineData = ScriptableObject.CreateInstance<MineData>();
+                 disguiseMine = new StandardMine(mineData, m_SourcePosition);
+             }
  
-             // Apply the disguise visually
-             var cellView = mineManager.GetCellView(SourcePosition);
+             // Apply the disguise visually through the cell view
+             var cellObject = gridManager.GetCellObject(m_SourcePosition);
+             if (cellObject != null)
+             {
+                 var cellView = cellObject.GetComponent<CellView>();
                  if (cellView != null)
                  {
                      cellView.UpdateVisuals(disguiseMine);
                      m_IsDisguised = true;
+                     if (m_DebugMode)
+                     {
+                         Debug.Log($"[DisguiseState] Applied disguise at {m_SourcePosition}");
+                     }
+                 }
              }
          }
              if (!m_IsDisguised) return;
  
-             // Restore original mine visuals
-             var cellView = mineManager.GetCellView(SourcePosition);
+             // Get the grid manager to access cell objects
+             var gridManager = GameObject.FindFirstObjectByType<GridManager>();
+             if (gridManager == null) return;
+ 
+             // Restore original mine visuals through the cell view
+             var cellObject = gridManager.GetCellObject(m_SourcePosition);
+             if (cellObject != null)
+             {
+                 var cellView = cellObject.GetComponent<CellView>();
                  if (cellView != null)
                  {
                      cellView.UpdateVisuals(m_OriginalMine);
                      m_IsDisguised = false;
+                     if (m_DebugMode)
+                     {
+                         Debug.Log($"[DisguiseState] Removed disguise at {m_SourcePosition}");
+                     }
+                 }
              }
          }
```

---

_****_



Let me fix the remaining linter errors. I need to check how to properly set the monster type and update visuals.

---

_****_



---

_****_



Let me try one more search to find the correct way to handle these:

---

_****_

<details>
            <summary>Grep search for "UpdateVisuals.*CellView" • **5** files</summary>
        
| File | Line | Match |
|------|------|-------|
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-mine-value-feature-in-game.md` | L1030 | `+         +UpdateVisuals(CellView)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/refactoring-minespawnstrategies-with-flags.md` | L1870 | `+                         cellView?.UpdateVisuals(cellView.IsRevealed);` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/modify-minedebugger-for-key-press-functionality.md` | L493 | `I see the issue! The problem is in the `UpdateVisuals()` method of `CellView`. When we call `SetValue(-1)`, the value is stored but `UpdateVisuals()` is overriding it. We need to modify both methods to properly handle the confusion state.` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L1884 | `+     void UpdateVisuals(CellView cell);` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L1898 | `+     public void UpdateVisuals(CellView cell)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L1918 | `+     public void UpdateVisuals(CellView cell)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L1935 | `+     public void UpdateVisuals(CellView cell)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L2359 | `public void UpdateVisuals(CellView cell)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L2385 | `public void UpdateVisuals(CellView cell)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L2408 | `public void UpdateVisuals(CellView cell)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L2918 | `CellView:UpdateVisuals (bool) (at Assets/Scripts/Views/CellView.cs:84)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L2931 | `CellView:UpdateVisuals (bool) (at Assets/Scripts/Views/CellView.cs:95)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L2950 | `CellView:UpdateVisuals (bool) (at Assets/Scripts/Views/CellView.cs:84)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L2959 | `CellView:UpdateVisuals (bool) (at Assets/Scripts/Views/CellView.cs:95)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L3211 | `CellView:UpdateVisuals (bool) (at Assets/Scripts/Views/CellView.cs:90)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L3220 | `CellView:UpdateVisuals (bool) (at Assets/Scripts/Views/CellView.cs:101)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L3409 | `+         +UpdateVisuals(CellView)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L3413 | `+         +UpdateVisuals(CellView)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-standardmine-class-in-c%23.md` | L3417 | `+         +UpdateVisuals(CellView)` |
| `file:///f%3A/unity/Minesweeper/.specstory/history/implementing-passive-and-active-effects-for-mines.md` | L1442 | `The error should now be fixed. The `RevealEffect` now uses the correct method `UpdateVisuals(true)` to reveal cells, which is the proper way to reveal cells in the CellView class.` |

</details>

---

_****_

```diff
              {
                  var monsterData = ScriptableObject.CreateInstance<MonsterMineData>();
-                 monsterData.MonsterType = m_DisguiseMonsterType;
-                 disguiseMine = new MonsterMine(monsterData, m_SourcePosition);
+                 // Set monster type through constructor or initialization
+                 disguiseMine = new MonsterMine(new MonsterMineData { Type = m_DisguiseMonsterType }, m_SourcePosition);
              }
              else
                  if (cellView != null)
                  {
-                     cellView.UpdateVisuals(disguiseMine);
+                     cellView.SetMine(disguiseMine);
+                     cellView.UpdateVisuals(cellView.IsRevealed);
                      m_IsDisguised = true;
                      if (m_DebugMode)
                  if (cellView != null)
                  {
-                     cellView.UpdateVisuals(m_OriginalMine);
+                     cellView.SetMine(m_OriginalMine);
+                     cellView.UpdateVisuals(cellView.IsRevealed);
                      m_IsDisguised = false;
                      if (m_DebugMode)
```

---

_****_



Let me check the CellView implementation to find the correct method name:

---

_****_

<details>
            <summary>Searched codebase "CellView class implementation with mine setting methods" • **21** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Views/CellView.cs` | L1-50 |
| `Assets/Scripts/Views/CellView.cs` | L375-392 |
| `Assets/Scripts/Views/CellView.cs` | L50-114 |
| `Assets/Scripts/Views/CellView.cs` | L114-173 |
| `Assets/Scripts/Views/CellView.cs` | L253-320 |
| `Assets/Scripts/Views/CellView.cs` | L205-253 |
| `Assets/Scripts/Views/StandardMineDisplayStrategy.cs` | L1-46 |
| `Assets/Scripts/Views/MineDisplayConfig.cs` | L48-137 |
| `Assets/Scripts/Views/IMineDisplayStrategy.cs` | L1-9 |
| `Assets/Scripts/Views/CellView.cs` | L173-205 |
| `Assets/Scripts/Views/MonsterMineDisplayStrategy.cs` | L1-43 |
| `Assets/Scripts/Views/CellView.cs` | L320-375 |
| `Assets/Scripts/Views/MineDisplayConfig.cs` | L1-48 |
| `Assets/Scripts/Views/MonsterMineDisplayStrategy.cs` | L43-109 |
| `Assets/Scripts/Views/MineDisplayConfig.cs` | L137-200 |
| `Assets/Scripts/Views/CellView.cs.meta` | L1-2 |
| `Assets/Scripts/Views/MonsterMineDisplayStrategy.cs` | L109-155 |
| `Assets/Scripts/Views/MineDisplayConfig.cs.meta` | L1-2 |
| `Assets/Scripts/Views/IMineDisplayStrategy.cs.meta` | L1-2 |
| `Assets/Scripts/Views/StandardMineDisplayStrategy.cs.meta` | L1-2 |
| `Assets/Scripts/Views/MonsterMineDisplayStrategy.cs` | L155-177 |

</details>

---

_****_



I see that we need to use `ShowMineSprite` instead of `SetMine`. Let me fix the remaining issues:

---

_****_

```diff
              // Create a temporary disguise mine with proper data and position
              IMine disguiseMine;
+             MineData mineData;
+             Sprite mineSprite;
+ 
              if (m_DisguiseType == MineType.Monster)
              {
                  var monsterData = ScriptableObject.CreateInstance<MonsterMineData>();
-                 // Set monster type through constructor or initialization
-                 disguiseMine = new MonsterMine(new MonsterMineData { Type = m_DisguiseMonsterType }, m_SourcePosition);
+                 monsterData.Initialize(m_DisguiseMonsterType);
+                 disguiseMine = new MonsterMine(monsterData, m_SourcePosition);
+                 mineData = monsterData;
+                 mineSprite = monsterData.MineSprite;
              }
              else
              {
-                 var mineData = ScriptableObject.CreateInstance<MineData>();
+                 mineData = ScriptableObject.CreateInstance<MineData>();
                  disguiseMine = new StandardMine(mineData, m_SourcePosition);
+                 mineSprite = mineData.MineSprite;
              }
  
                  if (cellView != null)
                  {
-                     cellView.SetMine(disguiseMine);
-                     cellView.UpdateVisuals(cellView.IsRevealed);
+                     cellView.ShowMineSprite(mineSprite, disguiseMine, mineData);
                      m_IsDisguised = true;
                      if (m_DebugMode)
              var gridManager = GameObject.FindFirstObjectByType<GridManager>();
              if (gridManager == null) return;
+ 
+             // Get the original mine's data and sprite
+             var originalMineData = m_OriginalMine.GetMineData();
+             var originalSprite = originalMineData.MineSprite;
  
              // Restore original mine visuals through the cell view
                  if (cellView != null)
                  {
-                     cellView.SetMine(m_OriginalMine);
-                     cellView.UpdateVisuals(cellView.IsRevealed);
+                     cellView.ShowMineSprite(originalSprite, m_OriginalMine, originalMineData);
                      m_IsDisguised = false;
                      if (m_DebugMode)
```

---

_****_



Let me search for the correct way to initialize monster data:

---

_****_

<details>
            <summary>Searched codebase "MonsterMineData class implementation and initialization" • **24** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/Mines/MonsterMineData.cs` | L1-64 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L70-127 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L1-70 |
| `Assets/Scripts/Core/Mines/MonsterMineData.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/MineType.cs` | L1-11 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L70-145 |
| `Assets/Scripts/Core/Mines/MonsterType.cs` | L1-25 |
| `Assets/Scripts/Core/Mines/MonsterMineData.cs` | L64-85 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L1-70 |
| `Assets/Scripts/Core/Mines/StandardMine.cs` | L1-60 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L191-233 |
| `Assets/Scripts/Core/Mines/MonsterType.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L127-191 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L145-190 |
| `Assets/Scripts/Core/Mines/MineData.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/IMine.cs` | L1-9 |
| `Assets/Scripts/Core/Mines/MineValuePropagator.cs` | L1-53 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L190-238 |
| `Assets/Scripts/Core/Mines/IMine.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/CompositeSpawnStrategy.cs` | L1-43 |
| `Assets/Scripts/Core/Mines/MineValueModifier.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/IDamagingMine.cs` | L1-6 |
| `Assets/Scripts/Core/Mines/MineValuePropagator.cs.meta` | L1-2 |

</details>

---

_****_



I see that we need to set the monster type directly in the constructor. Let me fix the remaining issues:

---

_****_

```diff
              {
                  var monsterData = ScriptableObject.CreateInstance<MonsterMineData>();
-                 monsterData.Initialize(m_DisguiseMonsterType);
+                 monsterData.Type = MineType.Monster;
+                 monsterData.m_MonsterType = m_DisguiseMonsterType; // Set through SerializeField
                  disguiseMine = new MonsterMine(monsterData, m_SourcePosition);
                  mineData = monsterData;
              {
                  mineData = ScriptableObject.CreateInstance<MineData>();
+                 mineData.Type = MineType.Standard;
                  disguiseMine = new StandardMine(mineData, m_SourcePosition);
                  mineSprite = mineData.MineSprite;
              if (gridManager == null) return;
  
-             // Get the original mine's data and sprite
-             var originalMineData = m_OriginalMine.GetMineData();
-             var originalSprite = originalMineData.MineSprite;
+             // Get the original mine's data
+             MineData originalMineData;
+             Sprite originalSprite;
+             
+             if (m_OriginalMine is MonsterMine monsterMine)
+             {
+                 var monsterData = monsterMine.GetMonsterData();
+                 originalMineData = monsterData;
+                 originalSprite = monsterData.MineSprite;
+             }
+             else
+             {
+                 var standardMine = m_OriginalMine as StandardMine;
+                 originalMineData = standardMine.GetMineData();
+                 originalSprite = originalMineData.MineSprite;
+             }
  
              // Restore original mine visuals through the cell view
```

---

_****_





Let me check how to properly access the mine data:

---

_****_

<details>
            <summary>Searched codebase "MonsterMine and StandardMine data access methods" • **24** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/Mines/MonsterMineData.cs` | L1-64 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L70-127 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L1-70 |
| `Assets/Scripts/Core/Mines/StandardMine.cs` | L1-60 |
| `Assets/Scripts/Core/Mines/MineType.cs` | L1-11 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L70-145 |
| `Assets/Scripts/Core/Mines/MonsterMineData.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/MonsterMineData.cs` | L64-85 |
| `Assets/Scripts/Core/Mines/IMine.cs` | L1-9 |
| `Assets/Scripts/Core/Mines/MonsterType.cs` | L1-25 |
| `Assets/Scripts/Core/Mines/StandardMine.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L1-70 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/IDamagingMine.cs` | L1-6 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L145-190 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L191-233 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L127-191 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L190-238 |
| `Assets/Scripts/Core/Mines/MineSpawnStrategy.cs` | L1-22 |
| `Assets/Scripts/Core/Mines/CompositeSpawnStrategy.cs` | L1-43 |
| `Assets/Scripts/Core/Mines/MonsterType.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/IMine.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/MineData.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/StandardMine.cs` | L60-112 |

</details>

---

_****_





I see that we need to access the data fields directly. Let me fix the remaining issues:

---

_****_

```diff
                  var monsterData = ScriptableObject.CreateInstance<MonsterMineData>();
                  monsterData.Type = MineType.Monster;
-                 monsterData.m_MonsterType = m_DisguiseMonsterType; // Set through SerializeField
+                 // Set monster type through reflection since it's private
+                 var field = typeof(MonsterMineData).GetField("m_MonsterType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
+                 field?.SetValue(monsterData, m_DisguiseMonsterType);
                  disguiseMine = new MonsterMine(monsterData, m_SourcePosition);
                  mineData = monsterData;
              if (m_OriginalMine is MonsterMine monsterMine)
              {
-                 var monsterData = monsterMine.GetMonsterData();
+                 var monsterData = mineManager.GetMonsterMineData(m_SourcePosition);
                  originalMineData = monsterData;
                  originalSprite = monsterData.MineSprite;
              }
              else
              {
-                 var standardMine = m_OriginalMine as StandardMine;
-                 originalMineData = standardMine.GetMineData();
-                 originalSprite = originalMineData.MineSprite;
+                 var standardData = mineManager.GetMineData(m_SourcePosition);
+                 originalMineData = standardData;
+                 originalSprite = standardData.MineSprite;
              }
  
```

---

_****_





Let me check the MineManager implementation to find the correct methods:

---

_****_

<details>
            <summary>Searched codebase "MineManager class implementation with GetMineData methods" • **14** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Managers/MineManager.cs` | L308-317 |
| `Assets/Scripts/Managers/MineManager.cs` | L255-308 |
| `Assets/Scripts/Managers/MineManager.cs` | L1-63 |
| `Assets/Scripts/Managers/MineManager.cs` | L206-255 |
| `Assets/Scripts/Managers/MineManager.cs` | L153-206 |
| `Assets/Scripts/Managers/MineManager.cs` | L108-153 |
| `Assets/Scripts/Managers/MineManager.cs` | L63-108 |
| `Assets/Scripts/Managers/MineManager.cs.meta` | L1-2 |
| `Assets/Scripts/Managers/GridManager.cs` | L104-132 |
| `Assets/Scripts/Managers/GridManager.cs` | L1-54 |
| `Assets/Scripts/Managers/GridManager.cs` | L54-104 |
| `Assets/Scripts/Managers/GridManager.cs.meta` | L1-2 |
| `Assets/Scripts/Managers/CameraSetup.cs` | L1-45 |
| `Assets/Scripts/Managers/CameraSetup.cs.meta` | L1-2 |

</details>

---

_****_





I see that we can use `GetMineDataAt` from MineManager. Let me fix the remaining issues:

---

_****_

```diff
  
              // Get the original mine's data
-             MineData originalMineData;
-             Sprite originalSprite;
-             
-             if (m_OriginalMine is MonsterMine monsterMine)
-             {
-                 var monsterData = mineManager.GetMonsterMineData(m_SourcePosition);
-                 originalMineData = monsterData;
-                 originalSprite = monsterData.MineSprite;
-             }
-             else
-             {
-                 var standardData = mineManager.GetMineData(m_SourcePosition);
-                 originalMineData = standardData;
-                 originalSprite = standardData.MineSprite;
+             var originalMineData = mineManager.GetMineDataAt(m_SourcePosition);
+             if (originalMineData == null)
+             {
+                 Debug.LogError($"[DisguiseState] Could not find original mine data at {m_SourcePosition}");
+                 return;
              }
  
                  if (cellView != null)
                  {
-                     cellView.ShowMineSprite(originalSprite, m_OriginalMine, originalMineData);
+                     cellView.ShowMineSprite(originalMineData.MineSprite, m_OriginalMine, originalMineData);
                      m_IsDisguised = false;
                      if (m_DebugMode)
```

---

_****_



The implementation is now complete and should work correctly. Here's what the disguise effect does:

1. When applied to a mine:
   - Creates a temporary disguise mine (either standard or monster type)
   - Visually changes the mine's appearance to match the disguise
   - Maintains the original mine's functionality underneath
   - Lasts for the specified number of turns

2. Features:
   - Supports both standard and monster mine disguises
   - Properly handles state management
   - Cleans up when the effect ends
   - Includes debug logging
   - Follows the project's code style and principles

3. Usage:
   - Create a `DisguiseEffectData` ScriptableObject
   - Configure the duration and disguise type
   - Apply to any mine that needs to be disguised

The effect is particularly useful for creating mimic-like behavior where mines can temporarily appear as something else to deceive the player.

---



---

