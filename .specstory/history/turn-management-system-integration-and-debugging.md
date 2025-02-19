## SpecStory

## Turn Management System Integration and Debugging (2/18/2025, 7:55:31 PM)

_**User**_

@Turn Management System Modifications, make sure that you follow @Principle.md, now I would like to integrate interaction with the turn manager, i.e. when I clicked to reveal a cell, it should count as one action, you should also add necessary debug log info to help me trace the data flow

---

_**Assistant**_

I'll help you integrate the interaction system with the turn manager following the principles from the documentation. Let me analyze and plan the changes needed.

Step-by-step plan:
1. Review current interaction flow in TurnManager.cs
2. Add debug logging for turn management and interaction
3. Ensure proper event handling between interaction and turns
4. Follow SOLID principles and coding conventions

First, let me check the interaction handling code:

---

_****_

<details>
            <summary>Searched codebase "class InteractionHandler or class InputManager implementation for cell interaction" • **24** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/Input/InteractionHandler.cs` | L1-39 |
| `Assets/Scripts/Core/Input/InteractionHandler.cs` | L39-67 |
| `Assets/Scripts/Core/Input/InputManager.cs` | L1-60 |
| `Assets/Scripts/Core/Input/InputManager.cs` | L60-104 |
| `Assets/Docs/Changelog/Architecture.md` | L1-160 |
| `Assets/Scripts/Views/CellView.cs` | L1-53 |
| `Assets/Scripts/Views/CellView.cs` | L309-371 |
| `Assets/Scripts/Core/Input/IInteractable.cs` | L1-11 |
| `Assets/Docs/Changelog/Changelog.md` | L1-81 |
| `Assets/Scripts/Managers/MineManager.cs` | L110-144 |
| `Assets/Scripts/Managers/GridManager.cs` | L104-132 |
| `Assets/Scripts/Managers/GridManager.cs` | L54-104 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L29-104 |
| `Assets/Scripts/Core/Grid/Cell.cs` | L1-30 |
| `Assets/Scripts/Managers/GridManager.cs` | L1-54 |
| `Assets/Scripts/Views/CellView.cs` | L53-113 |
| `Assets/Scripts/Managers/MineManager.cs` | L144-180 |
| `Assets/Scripts/Core/States/FrozenState.cs` | L1-61 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L55-121 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L136-174 |
| `Assets/Scripts/Core/Grid/ICell.cs` | L1-8 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L1-55 |
| `Assets/Scripts/Core/Mines/MineValuePropagator.cs` | L1-53 |
| `Assets/Scripts/Views/CellView.cs` | L238-309 |

</details>

---

_****_



Based on the code review, I see that we already have a good foundation for integrating the interaction system with the turn manager. Let me outline the current flow and propose enhancements:

Current Flow:
1. `InputManager` detects clicks and raises `OnCellClicked`
2. `InteractionHandler` receives click and calls `OnInteract()` on the `CellView`
3. `CellView.OnInteract()` raises `GameEvents.RaiseCellRevealed`
4. `TurnManager` listens to `GameEvents.OnCellRevealed` and queues a `PlayerTurn`

Let's enhance the debug logging to better trace this flow and ensure proper turn integration:

1. First, let's add more debug logging to the TurnManager:

---

_****_

```diff
                  if (m_DebugMode)
                  {
-                     //Debug.Log($"[TurnManager] Processing turns. Current: {m_CurrentTurn?.GetType().Name ?? "None"}, Pending: {m_PendingTurns.Count}");
+                     Debug.Log($"[TurnManager] Processing turns. Current: {m_CurrentTurn?.GetType().Name ?? "None"}, Pending: {m_PendingTurns.Count}");
                  }
  
          private void HandleCellRevealed(Vector2Int position)
          {
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[TurnManager] Cell revealed at {position}, queueing PlayerTurn");
+             }
              // Queue player turn when cell is revealed
              QueueTurn(new PlayerTurn(position));
```

---

_****_



2. Let's enhance the debug logging in CellView to better trace the interaction flow:

---

_****_

```diff
          if (CanInteract)
          {
+             Debug.Log($"[CellView] Revealing cell at {m_Position}, CanInteract: {CanInteract}, IsFrozen: {m_IsFrozen}, IsRevealed: {m_IsRevealed}");
              GameEvents.RaiseCellRevealed(m_Position);
          }
          else if (m_IsRevealed && m_HasMine)
          {
+             Debug.Log($"[CellView] Interacting with revealed mine at {m_Position}, IsCollectable: {(m_CurrentMine as MonsterMine)?.IsCollectable}");
              // For monster mines, process interaction on every click
              var monsterMine = m_CurrentMine as MonsterMine;
              }
          }
+         else
+         {
+             Debug.Log($"[CellView] Cannot interact with cell at {m_Position}, CanInteract: {CanInteract}, IsFrozen: {m_IsFrozen}, IsRevealed: {m_IsRevealed}");
+         }
      }
  
```

---

_****_



3. Finally, let's enhance the debug logging in InteractionHandler:

---

_****_



---

_****_



I notice the InteractionHandler already has good debug logging. Now let's verify the complete flow:

1. When a cell is clicked:
   - `InputManager` detects the click and raises `OnCellClicked`
   - `InteractionHandler` receives the click and logs "[InteractionHandler] Handling click at position {x,y}"
   - If cell is found, logs "[InteractionHandler] Found cell object: Cell_{x}_{y}"
   - Logs "[InteractionHandler] Found IInteractable, CanInteract: {true/false}"

2. When the cell is interacted with:
   - `CellView` logs "[CellView] Revealing cell at {position}" with detailed state
   - If cell can't be interacted with, logs reason
   - If cell has a mine, logs mine interaction details

3. When the turn is processed:
   - `TurnManager` logs "[TurnManager] Cell revealed at {position}, queueing PlayerTurn"
   - Logs "[TurnManager] Processing turns" with current and pending turn info
   - Logs "[TurnManager] Starting turn" and "[TurnManager] Ending turn" for each turn

This enhanced logging will help trace the complete flow from click to turn processing. The integration is already properly set up where:
1. Each cell reveal counts as one action (one PlayerTurn)
2. The turn system manages the flow of actions
3. Debug logs help trace the entire process

You can now enable debug mode on the TurnManager, InteractionHandler, and other components to see the complete flow of actions when clicking cells. The logs will show:
1. When a cell is clicked
2. Whether it can be interacted with
3. When the interaction triggers a turn
4. How the turn is processed

Is there anything specific about the turn integration you'd like me to explain or enhance further?

---

_**User**_

one of the problem in @TurnManager.cs is the processTurns() is in Update(), which will update each frame, which violets the concept of "turn" (that's the reason why I comment it out)

---

_**Assistant**_

Ah, you've identified a key issue. You're absolutely right - having `ProcessTurns()` in `Update()` violates the turn-based concept since it's being checked every frame. Let's fix this by making the turn processing event-driven instead of frame-based.

Here's how we should modify the TurnManager:

---

_****_

```diff
              GameEvents.OnEffectApplied -= HandleEffectApplied;
          }
- 
-         private void Update()
-         {
-             ProcessTurns();
-         }
          #endregion
  
          #region Public Methods
          public void QueueTurn(ITurn turn)
          {
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[TurnManager] Queueing turn: {turn.GetType().Name}, Current queue size: {m_PendingTurns.Count}");
+             }
+             
              m_PendingTurns.Enqueue(turn);
-             if (!m_IsProcessingTurns)
+             ProcessTurns();
+         }
+ 
+         public void CompleteTurn()
+         {
+             if (m_CurrentTurn != null)
+             {
+                 if (m_DebugMode)
                  {
+                     Debug.Log($"[TurnManager] Completing current turn: {m_CurrentTurn.GetType().Name}");
+                 }
+                 EndCurrentTurn();
                  ProcessTurns();
              }
                  if (m_DebugMode)
                  {
-                     //Debug.Log($"[TurnManager] Processing turns. Current: {m_CurrentTurn?.GetType().Name ?? "None"}, Pending: {m_PendingTurns.Count}");
-                 }
- 
-                 // Process current turn if it exists
-                 if (m_CurrentTurn != null)
-                 {
-                     m_CurrentTurn.Update();
-                     if (m_CurrentTurn.IsComplete)
-                     {
-                         EndCurrentTurn();
-                     }
+                     Debug.Log($"[TurnManager] Processing turns. Current: {m_CurrentTurn?.GetType().Name ?? "None"}, Pending: {m_PendingTurns.Count}");
                  }
  
```

---

_****_



Key changes made to fix the turn-based system:

1. Removed `Update()` method that was continuously checking turns
2. Removed automatic turn completion check from `ProcessTurns()`
3. Added a new `CompleteTurn()` method that explicitly ends the current turn
4. Made turn processing purely event-driven
5. Enhanced debug logging for better turn flow tracking

The main changes are:

1. **Event-Driven Processing**: Instead of checking every frame, turns are now processed only when:
   - A new turn is queued (`QueueTurn`)
   - A turn is explicitly completed (`CompleteTurn`)

2. **Manual Turn Completion**: Turns must now be explicitly completed by calling `CompleteTurn()` rather than being automatically checked in `Update()`

3. **Simplified Turn Flow**:
   ```
   QueueTurn -> ProcessTurns -> StartNextTurn -> (Turn executes) -> CompleteTurn -> ProcessTurns
   ```

This change means that:
1. Each turn is now properly discrete and controlled
2. Turns won't automatically advance without explicit completion
3. The system is more predictable and follows true turn-based logic
4. Better performance as we're not checking every frame

To use this system, turns should now call `TurnManager.CompleteTurn()` when they are finished, rather than relying on the `Update` loop to check their completion status. This gives better control over when turns end and maintains the true turn-based nature of the system.

Would you like me to also show how to modify the turn implementations (PlayerTurn, ReactionTurn, etc.) to work with this new system?

---

_**User**_

I also need a counter to trace the current turn, you should also add the respective debug info

---

_**Assistant**_

I'll add a turn counter and enhance the debug information to track turn progression. Here's the modified TurnManager:

---

_****_

```diff
          public static event System.Action<ITurn> OnTurnStarted;
          public static event System.Action<ITurn> OnTurnEnded;
+         public static event System.Action<int> OnTurnCountChanged;
          #endregion
  
          private ITurn m_CurrentTurn;
          private bool m_IsProcessingTurns;
+         private int m_TurnCount;
          [SerializeField] private bool m_DebugMode;
          #endregion
+ 
+         #region Public Properties
+         public int CurrentTurn => m_TurnCount;
+         public ITurn ActiveTurn => m_CurrentTurn;
+         public int PendingTurnsCount => m_PendingTurns.Count;
+         #endregion
  
          #region Unity Lifecycle
          private void Awake()
          {
+             m_TurnCount = 0;
              GameEvents.OnCellRevealed += HandleCellRevealed;
              GameEvents.OnMineTriggered += HandleMineTriggered;
              GameEvents.OnEffectApplied += HandleEffectApplied;
+             
+             if (m_DebugMode)
+             {
+                 Debug.Log("[TurnManager] Initialized. Turn count: 0");
+             }
          }
  
              if (m_DebugMode)
              {
-                 Debug.Log($"[TurnManager] Queueing turn: {turn.GetType().Name}, Current queue size: {m_PendingTurns.Count}");
+                 Debug.Log($"[TurnManager] Queueing turn: {turn.GetType().Name}, Current turn: {m_TurnCount}, Queue size: {m_PendingTurns.Count}");
              }
              
                  if (m_DebugMode)
                  {
-                     Debug.Log($"[TurnManager] Completing current turn: {m_CurrentTurn.GetType().Name}");
+                     Debug.Log($"[TurnManager] Completing turn {m_TurnCount}: {m_CurrentTurn.GetType().Name}");
                  }
                  EndCurrentTurn();
                  ProcessTurns();
              }
          }
+ 
+         public void ResetTurnCount()
+         {
+             m_TurnCount = 0;
+             if (m_DebugMode)
+             {
+                 Debug.Log("[TurnManager] Turn count reset to 0");
+             }
+             OnTurnCountChanged?.Invoke(m_TurnCount);
+         }
          #endregion
  
                  if (m_DebugMode)
                  {
-                     Debug.Log($"[TurnManager] Processing turns. Current: {m_CurrentTurn?.GetType().Name ?? "None"}, Pending: {m_PendingTurns.Count}");
+                     Debug.Log($"[TurnManager] Processing turns. Turn {m_TurnCount} | Current: {m_CurrentTurn?.GetType().Name ?? "None"} | Pending: {m_PendingTurns.Count}");
                  }
  
              if (m_PendingTurns.Count == 0) return;
  
+             m_TurnCount++;
              m_CurrentTurn = m_PendingTurns.Dequeue();
              m_CurrentTurn.Begin();
+             
              if (m_DebugMode)
              {
-                 Debug.Log($"[TurnManager] Starting turn: {m_CurrentTurn.GetType().Name}");
+                 Debug.Log($"[TurnManager] Starting turn {m_TurnCount}: {m_CurrentTurn.GetType().Name} | Pending turns: {m_PendingTurns.Count}");
              }
+             
              OnTurnStarted?.Invoke(m_CurrentTurn);
+             OnTurnCountChanged?.Invoke(m_TurnCount);
          }
  
              if (m_DebugMode)
              {
-                 Debug.Log($"[TurnManager] Ending turn: {m_CurrentTurn.GetType().Name}");
+                 Debug.Log($"[TurnManager] Ending turn {m_TurnCount}: {m_CurrentTurn.GetType().Name} | Total turns completed: {m_TurnCount}");
              }
+             
              m_CurrentTurn.End();
              OnTurnEnded?.Invoke(m_CurrentTurn);
              if (m_DebugMode)
              {
-                 Debug.Log($"[TurnManager] Cell revealed at {position}, queueing PlayerTurn");
+                 Debug.Log($"[TurnManager] Turn {m_TurnCount}: Cell revealed at {position}, queueing PlayerTurn");
              }
              // Queue player turn when cell is revealed
          private void HandleMineTriggered(MineType type)
          {
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[TurnManager] Turn {m_TurnCount}: Mine of type {type} triggered, queueing ReactionTurn");
+             }
              // Queue reaction turn when mine is triggered
              QueueTurn(new ReactionTurn(type));
          private void HandleEffectApplied(Vector2Int position)
          {
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[TurnManager] Turn {m_TurnCount}: Effect applied at {position}, queueing EffectTurn");
+             }
              // Queue effect turn when effect is applied
              QueueTurn(new EffectTurn(position));
```

---

_****_



Key changes made:

1. **Added Turn Counter**:
   - Added `m_TurnCount` field to track current turn number
   - Increments in `StartNextTurn()`
   - Added `ResetTurnCount()` method for game resets

2. **Added Public Properties**:
   - `CurrentTurn`: Get current turn number
   - `ActiveTurn`: Get currently active turn
   - `PendingTurnsCount`: Get number of pending turns

3. **Added New Event**:
   - `OnTurnCountChanged`: Notifies listeners when turn count changes

4. **Enhanced Debug Logging**:
   - Added turn count to all debug messages
   - Added more detailed state information
   - Added initialization logging
   - Added turn completion status

5. **Improved Status Reporting**:
   - All debug messages now include turn number
   - Added pending turns count to relevant messages
   - Added total turns completed to end turn message

The debug output now shows a clear progression of turns:
```
[TurnManager] Initialized. Turn count: 0
[TurnManager] Turn 0: Cell revealed at (2,3), queueing PlayerTurn
[TurnManager] Queueing turn: PlayerTurn, Current turn: 0, Queue size: 0
[TurnManager] Processing turns. Turn 0 | Current: None | Pending: 1
[TurnManager] Starting turn 1: PlayerTurn | Pending turns: 0
...
[TurnManager] Ending turn 1: PlayerTurn | Total turns completed: 1
```

This makes it much easier to:
1. Track the progression of turns
2. Debug turn-based issues
3. Monitor game state
4. Verify turn completion
5. Track different types of turns (Player, Reaction, Effect)

Would you like me to add any additional debug information or make any adjustments to the turn counting system?

---

_**User**_

[CellView] Revealing cell at (12, 9), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Revealing cell at (12, 10), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Cannot interact with cell at (12, 10), CanInteract: False, IsFrozen: False, IsRevealed: True
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:352)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Revealing cell at (12, 11), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Revealing cell at (11, 11), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Revealing cell at (11, 12), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Revealing cell at (12, 12), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

So current no "turn" triggered 

---

_**Assistant**_

Ah, I see the issue. The cell interaction is working, but the turn system isn't being triggered because we're missing the connection between `GameEvents.RaiseCellRevealed` and the turn system. Let me check the GameEvents system:

---

_****_

<details>
            <summary>Searched codebase "class GameEvents or static GameEvents implementation" • **20** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/Events/GameEvents.cs` | L1-55 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L1-55 |
| `Assets/Scripts/Core/States/StateManager.cs` | L1-56 |
| `Assets/Scripts/Core/TurnSystem/PlayerTurn.cs` | L1-43 |
| `Assets/Scripts/Core/Grid/Cell.cs` | L1-30 |
| `Assets/Docs/Changelog/Architecture.md` | L160-202 |
| `Assets/Scripts/Core/Events/GameEvents.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Input/InputManager.cs` | L1-60 |
| `Assets/Scripts/Managers/GridManager.cs` | L54-104 |
| `Assets/Scripts/Core/Player/PlayerComponent.cs` | L1-58 |
| `Assets/Scripts/Core/Player/Player.cs` | L1-48 |
| `Assets/Scripts/Managers/MineManager.cs` | L65-110 |
| `Assets/Plugins/Sirenix/Assemblies/Sirenix.Utilities.Editor.xml` | L1330-1365 |
| `Assets/Plugins/Sirenix/Assemblies/Sirenix.Utilities.Editor.xml` | L1182-1232 |
| `Assets/Scripts/Tests/MineAdditionTest.cs` | L1-54 |
| `Assets/Scripts/Views/CellView.cs` | L309-371 |
| `Assets/Plugins/Sirenix/Assemblies/Sirenix.Utilities.Editor.xml` | L1305-1330 |
| `Assets/Scripts/Core/Effects/BaseEffect.cs` | L1-51 |
| `Assets/Scripts/Core/Mines/MonsterMine.cs` | L1-83 |
| `ProjectSettings/UnityConnectSettings.asset` | L1-37 |

</details>

---

_****_



Let me check the PlayerTurn implementation:

---

_****_

<details>
            <summary>Searched codebase "class PlayerTurn implementation or ITurn interface" • **20** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/TurnSystem/ITurn.cs` | L1-12 |
| `Assets/Scripts/Core/TurnSystem/PlayerTurn.cs` | L1-43 |
| `Assets/Scripts/Core/States/ITurnBasedState.cs` | L1-10 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L1-55 |
| `Assets/Scripts/Core/States/BaseTurnState.cs` | L1-36 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L55-121 |
| `Assets/Scripts/Core/TurnSystem/EffectTurn.cs` | L1-47 |
| `Assets/Scripts/Core/TurnSystem/ReactionTurn.cs` | L1-47 |
| `Assets/Scripts/Core/TurnSystem/ITurn.cs.meta` | L1-2 |
| `Assets/Scripts/Core/States/StateManager.cs` | L1-56 |
| `Assets/Scripts/Core/States/BaseState.cs` | L1-51 |
| `Assets/Scripts/Core/Mines/IMine.cs` | L1-9 |
| `Assets/Scripts/Core/States/StateManager.cs` | L56-109 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L121-133 |
| `Assets/Scripts/Core/States/FrozenState.cs` | L1-61 |
| `Assets/Scripts/Core/TurnSystem/PlayerTurn.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Grid/ICell.cs` | L1-8 |
| `Assets/Scripts/Core/Input/IInteractable.cs` | L1-11 |
| `Assets/Scripts/Core/States/IState.cs` | L1-16 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L29-104 |

</details>

---

_****_



I see the issue. The problem is in the PlayerTurn implementation. Currently, it's not properly completing the turn. Let's modify the PlayerTurn class:

---

_****_

```diff
          private readonly Vector2Int m_Position;
          private bool m_IsComplete;
+         private bool m_DebugMode = true; // Enable debug logging for player turns
          #endregion
  
          {
              m_Position = position;
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[PlayerTurn] Created new turn for position {position}");
+             }
          }
          #endregion
          public void Begin()
          {
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[PlayerTurn] Beginning turn at position {m_Position}");
+             }
              // Player turn begins immediately when cell is revealed
-             // No additional setup needed
+             m_IsComplete = false;
          }
  
          public void Update()
          {
-             // Player turn completes immediately after cell reveal
+             if (!m_IsComplete)
+             {
+                 if (m_DebugMode)
+                 {
+                     Debug.Log($"[PlayerTurn] Completing turn at position {m_Position}");
+                 }
                  m_IsComplete = true;
+                 // Find TurnManager and complete the turn
+                 var turnManager = GameObject.FindFirstObjectByType<TurnManager>();
+                 if (turnManager != null)
+                 {
+                     turnManager.CompleteTurn();
+                 }
+             }
          }
  
          public void End()
          {
+             if (m_DebugMode)
+             {
+                 Debug.Log($"[PlayerTurn] Ending turn at position {m_Position}");
+             }
              // Notify that player's action is complete
              GameEvents.RaiseRoundAdvanced();
```

---

_****_



Key changes made:

1. Added debug logging to track turn flow
2. Fixed turn completion logic:
   - Initialize `m_IsComplete` to false in `Begin()`
   - Properly complete the turn in `Update()`
   - Call `TurnManager.CompleteTurn()` when the turn is done

The flow should now be:
1. Cell is clicked -> `GameEvents.RaiseCellRevealed`
2. TurnManager creates and queues a new PlayerTurn
3. PlayerTurn begins (logs "Beginning turn")
4. PlayerTurn updates and completes itself (logs "Completing turn")
5. TurnManager processes the completion and ends the turn (logs "Ending turn")

You should now see the turn progression in the debug logs:
```
[CellView] Revealing cell at (x,y)...
[PlayerTurn] Created new turn for position (x,y)
[TurnManager] Queueing turn: PlayerTurn...
[PlayerTurn] Beginning turn at position (x,y)
[PlayerTurn] Completing turn at position (x,y)
[TurnManager] Completing turn...
[PlayerTurn] Ending turn at position (x,y)
```

This should fix the issue where turns weren't being triggered. The key was making sure the PlayerTurn properly completes itself and notifies the TurnManager.

---

_**User**_

[CellView] Revealing cell at (12, 12), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[PlayerTurn] Created new turn for position (12, 12)
UnityEngine.Debug:Log (object)
RPGMinesweeper.TurnSystem.PlayerTurn:.ctor (UnityEngine.Vector2Int) (at Assets/Scripts/Core/TurnSystem/PlayerTurn.cs:23)
RPGMinesweeper.TurnSystem.TurnManager:HandleCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/TurnSystem/TurnManager.cs:150)
GameEvents:RaiseCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Events/GameEvents.cs:18)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:323)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[PlayerTurn] Beginning turn at position (12, 12)
UnityEngine.Debug:Log (object)
RPGMinesweeper.TurnSystem.PlayerTurn:Begin () (at Assets/Scripts/Core/TurnSystem/PlayerTurn.cs:33)
RPGMinesweeper.TurnSystem.TurnManager:StartNextTurn () (at Assets/Scripts/Core/TurnSystem/TurnManager.cs:118)
RPGMinesweeper.TurnSystem.TurnManager:ProcessTurns () (at Assets/Scripts/Core/TurnSystem/TurnManager.cs:103)
RPGMinesweeper.TurnSystem.TurnManager:QueueTurn (RPGMinesweeper.TurnSystem.ITurn) (at Assets/Scripts/Core/TurnSystem/TurnManager.cs:60)
RPGMinesweeper.TurnSystem.TurnManager:HandleCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/TurnSystem/TurnManager.cs:150)
GameEvents:RaiseCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Events/GameEvents.cs:18)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:323)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Revealing cell at (12, 11), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[PlayerTurn] Created new turn for position (12, 11)
UnityEngine.Debug:Log (object)
RPGMinesweeper.TurnSystem.PlayerTurn:.ctor (UnityEngine.Vector2Int) (at Assets/Scripts/Core/TurnSystem/PlayerTurn.cs:23)
RPGMinesweeper.TurnSystem.TurnManager:HandleCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/TurnSystem/TurnManager.cs:150)
GameEvents:RaiseCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Events/GameEvents.cs:18)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:323)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Revealing cell at (12, 10), CanInteract: True, IsFrozen: False, IsRevealed: False
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:322)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[PlayerTurn] Created new turn for position (12, 10)
UnityEngine.Debug:Log (object)
RPGMinesweeper.TurnSystem.PlayerTurn:.ctor (UnityEngine.Vector2Int) (at Assets/Scripts/Core/TurnSystem/PlayerTurn.cs:23)
RPGMinesweeper.TurnSystem.TurnManager:HandleCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/TurnSystem/TurnManager.cs:150)
GameEvents:RaiseCellRevealed (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Events/GameEvents.cs:18)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:323)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)

[CellView] Interacting with revealed mine at (12, 10), IsCollectable: True
UnityEngine.Debug:Log (object)
CellView:OnInteract () (at Assets/Scripts/Views/CellView.cs:327)
RPGMinesweeper.Input.InteractionHandler:HandleCellClicked (UnityEngine.Vector2Int) (at Assets/Scripts/Core/Input/InteractionHandler.cs:53)
RPGMinesweeper.Input.InputManager:HandleMouseClick () (at Assets/Scripts/Core/Input/InputManager.cs:85)
RPGMinesweeper.Input.InputManager:Update () (at Assets/Scripts/Core/Input/InputManager.cs:48)



seems no turn "completed", and please also add turn counter

---

_**Assistant**_

Ah, I see the issue. The problem is that we're relying on `Update()` to complete the turn, but we're not calling `Update()` anymore since we removed it from the TurnManager. Let's modify both the PlayerTurn and TurnManager to fix this:

1. First, let's modify the PlayerTurn to complete immediately:

---

_****_

```diff
          private bool m_IsComplete;
          private bool m_DebugMode = true; // Enable debug logging for player turns
+         private TurnManager m_TurnManager;
          #endregion
  
          {
              m_Position = position;
+             m_TurnManager = GameObject.FindFirstObjectByType<TurnManager>();
              if (m_DebugMode)
              {
              if (m_DebugMode)
              {
-                 Debug.Log($"[PlayerTurn] Beginning turn at position {m_Position}");
+                 Debug.Log($"[PlayerTurn] Beginning turn at position {m_Position}, Turn #{m_TurnManager?.CurrentTurn ?? 0}");
              }
-             // Player turn begins immediately when cell is revealed
-             m_IsComplete = false;
+             
+             // Player turn completes immediately when it begins
+             m_IsComplete = true;
+             
+             // Complete the turn right away
+             if (m_TurnManager != null)
+             {
+                 m_TurnManager.CompleteTurn();
+             }
+             else if (m_DebugMode)
+             {
+                 Debug.LogError("[PlayerTurn] Could not find TurnManager!");
+             }
          }
  
          public void Update()
          {
-             if (!m_IsComplete)
-             {
-                 if (m_DebugMode)
-                 {
-                     Debug.Log($"[PlayerTurn] Completing turn at position {m_Position}");
-                 }
-                 m_IsComplete = true;
-                 // Find TurnManager and complete the turn
-                 var turnManager = GameObject.FindFirstObjectByType<TurnManager>();
-                 if (turnManager != null)
-                 {
-                     turnManager.CompleteTurn();
-                 }
-             }
+             // Not used since we complete in Begin()
          }
  
              if (m_DebugMode)
              {
-                 Debug.Log($"[PlayerTurn] Ending turn at position {m_Position}");
+                 Debug.Log($"[PlayerTurn] Ending turn at position {m_Position}, Turn #{m_TurnManager?.CurrentTurn ?? 0}");
              }
              // Notify that player's action is complete
```

---

_****_



2. Now let's enhance the TurnManager's debug logging to better show turn counts:

---

_****_

```diff
              if (m_DebugMode)
              {
-                 Debug.Log($"[TurnManager] Queueing turn: {turn.GetType().Name}, Current turn: {m_TurnCount}, Queue size: {m_PendingTurns.Count}");
+                 Debug.Log($"[TurnManager] Turn {m_TurnCount}: Queueing {turn.GetType().Name}, Queue size: {m_PendingTurns.Count}");
              }
              
                  if (m_DebugMode)
                  {
-                     Debug.Log($"[TurnManager] Completing turn {m_TurnCount}: {m_CurrentTurn.GetType().Name}");
+                     Debug.Log($"[TurnManager] Turn {m_TurnCount}: Completing {m_CurrentTurn.GetType().Name}");
                  }
                  EndCurrentTurn();
                  if (m_DebugMode)
                  {
-                     Debug.Log($"[TurnManager] Processing turns. Turn {m_TurnCount} | Current: {m_CurrentTurn?.GetType().Name ?? "None"} | Pending: {m_PendingTurns.Count}");
+                     Debug.Log($"[TurnManager] Turn {m_TurnCount}: Processing turns | Current: {m_CurrentTurn?.GetType().Name ?? "None"} | Pending: {m_PendingTurns.Count}");
                  }
  
              if (m_DebugMode)
              {
-                 Debug.Log($"[TurnManager] Starting turn {m_TurnCount}: {m_CurrentTurn.GetType().Name} | Pending turns: {m_PendingTurns.Count}");
+                 Debug.Log($"[TurnManager] Turn {m_TurnCount}: Starting {m_CurrentTurn.GetType().Name} | Pending: {m_PendingTurns.Count}");
              }
              
              if (m_DebugMode)
              {
-                 Debug.Log($"[TurnManager] Ending turn {m_TurnCount}: {m_CurrentTurn.GetType().Name} | Total turns completed: {m_TurnCount}");
+                 Debug.Log($"[TurnManager] Turn {m_TurnCount}: Ending {m_CurrentTurn.GetType().Name}");
              }
              
```

---

_****_



Key changes made:

1. In PlayerTurn:
   - Cache TurnManager reference in constructor
   - Complete turn immediately in `Begin()` instead of waiting for `Update()`
   - Added turn counter to debug logs
   - Removed unnecessary `Update()` logic

2. In TurnManager:
   - Enhanced debug logging format for better readability
   - Consistently show turn numbers in all logs
   - Made log messages more concise and uniform

Now you should see a clear progression of turns:
```
[CellView] Revealing cell at (x,y)...
[TurnManager] Turn 0: Cell revealed at (x,y), queueing PlayerTurn
[TurnManager] Turn 0: Queueing PlayerTurn, Queue size: 0
[TurnManager] Turn 0: Processing turns | Current: None | Pending: 1
[TurnManager] Turn 1: Starting PlayerTurn | Pending: 0
[PlayerTurn] Beginning turn at position (x,y), Turn #1
[TurnManager] Turn 1: Completing PlayerTurn
[TurnManager] Turn 1: Ending PlayerTurn
```

The key changes are:
1. Turns complete immediately when they begin (no need for Update)
2. Clear turn counting in all debug messages
3. Proper turn progression tracking

Try it now and you should see the turns properly incrementing and completing.

---

_**User**_

based on @Commit (Diff of Working State), @generate git commit etc. 

---

_**Assistant**_

Let me help you generate a commit message and update the documentation based on the changes shown in the diff.

First, I'll analyze the changes:
1. Enhanced turn system with proper turn counting and debug logging
2. Improved turn completion logic in PlayerTurn
3. Removed Update() polling in TurnManager
4. Added debug logging throughout interaction flow

Here's the commit message following the format:

```
refactor: enhance turn system with event-driven flow and debug tracing

Improve turn system implementation with proper turn counting and debug tracing:
- Remove Update() polling in TurnManager for event-driven turn processing
- Add turn counter with OnTurnCountChanged event
- Enhance debug logging throughout interaction flow
- Improve PlayerTurn completion logic with immediate completion in Begin()

This change makes the turn system more predictable and easier to debug by
providing clear visibility into the turn progression and interaction flow.
```

Now I'll update the changelog. I'll add this entry at the top of the Changelog.md file:

---

_****_

```diff
+ # v0.1.39 - 2025-02-18 20:00:00
+ ## Overview
+ Enhanced the turn system with event-driven flow and improved debug tracing, making turn progression more predictable and easier to debug.
+ 
+ ## Change Details
+ ### Architecture Improvements
+ #### Turn System Enhancement
+ - Removed Update() polling in favor of event-driven turn processing
+ - Added turn counter with proper event notification
+ - Enhanced debug logging throughout interaction flow
+ - Improved turn completion logic in PlayerTurn
+ ```mermaid
+ classDiagram
+ class TurnManager {
+     -m_TurnCount: int
+     -m_PendingTurns: Queue~ITurn~
+     -m_CurrentTurn: ITurn
+     +CurrentTurn: int
+     +ActiveTurn: ITurn
+     +PendingTurnsCount: int
+     +CompleteTurn()
+     +QueueTurn(ITurn)
+ }
+ class PlayerTurn {
+     -m_Position: Vector2Int
+     -m_IsComplete: bool
+     +Begin()
+     +End()
+ }
+ class Events {
+     OnTurnStarted
+     OnTurnEnded
+     OnTurnCountChanged
+ }
+ TurnManager --> PlayerTurn : Manages
+ TurnManager --> Events : Raises
+ note for TurnManager "Event-driven turn processing\nwith debug tracing"
+ ```
+ 
+ ### Adjustments and Refactoring
+ #### Debug System Enhancement
+ - Added comprehensive debug logging in TurnManager
+ - Enhanced interaction flow tracing in CellView
+ - Improved turn state visibility
+ ```mermaid
+ classDiagram
+ class DebugFlow {
+     TurnCounting
+     InteractionTracing
+     StateLogging
+ }
+ class Implementation {
+     TurnProgression[counter]
+     CellInteraction[states]
+     TurnCompletion[events]
+ }
+ DebugFlow --> Implementation
+ note for DebugFlow "Clear visibility into\nturn progression"
+ ```
+ 
+ ### Optimizations
+ - Improved turn processing efficiency by removing polling
+ - Enhanced debugging capabilities
+ - Better turn state management
+ - Clearer interaction flow tracing
+ 
  # v0.1.38 - 2025-02-18 19:30:00
  ## Overview
```

---

_****_



Now I'll update the Architecture.md file to reflect the changes in the turn system:

---

_****_

```diff
  +OnMineRemovalAttempted: Action<Vector2Int>
  }
+ ```
+ 
+ ### Turn System
+ ```mermaid
+ classDiagram
+ class TurnManager {
+     -m_TurnCount: int
+     -m_PendingTurns: Queue~ITurn~
+     -m_CurrentTurn: ITurn
+     +CurrentTurn: int
+     +ActiveTurn: ITurn
+     +PendingTurnsCount: int
+     +QueueTurn(ITurn)
+     +CompleteTurn()
+     +ResetTurnCount()
+ }
+ class ITurn {
+     <<interface>>
+     +IsComplete: bool
+     +Begin()
+     +End()
+ }
+ class PlayerTurn {
+     -m_Position: Vector2Int
+     -m_IsComplete: bool
+     +Begin()
+     +End()
+ }
+ class ReactionTurn {
+     -m_MineType: MineType
+     -m_IsComplete: bool
+     +Begin()
+     +End()
+ }
+ class EffectTurn {
+     -m_Position: Vector2Int
+     -m_IsComplete: bool
+     +Begin()
+     +End()
+ }
+ ITurn <|.. PlayerTurn
+ ITurn <|.. ReactionTurn
+ ITurn <|.. EffectTurn
+ TurnManager --> ITurn : Manages
+ note for TurnManager "Event-driven turn processing\nwith debug tracing"
+ note for ITurn "Base interface for all\nturn types"
  ```
  
```

---

_****_



I've completed the documentation updates:

1. Created a commit message following the format in F_Commit.md
2. Added a new changelog entry at the top of Changelog.md
3. Updated Architecture.md with a new Turn System section

The changes reflect the improvements made to the turn system, including:
- Event-driven turn processing
- Turn counting and state management
- Enhanced debug logging
- Improved turn completion logic

The documentation now provides a clear overview of the turn system architecture and its recent enhancements.

---



---

