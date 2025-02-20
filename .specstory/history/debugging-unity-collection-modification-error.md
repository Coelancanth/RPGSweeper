## SpecStory

## Debugging Unity Collection Modification Error (2/20/2025, 4:51:34 PM)

_**User**_

please fix the bug, 

Advancing to round 12...
UnityEngine.Debug:Log (object)
MineDebugger:AdvanceRound () (at Assets/Scripts/Debug/MineDebugger.cs:229)
MineDebugger:Update () (at Assets/Scripts/Debug/MineDebugger.cs:98)

InvalidOperationException: Collection was modified; enumeration operation may not execute.
System.Collections.Generic.Dictionary`2+Enumerator[TKey,TValue].MoveNext () (at <ed969b0e627d471da4848289f9c322df>:0)
RPGMinesweeper.States.StateManager.OnTurnEnd () (at Assets/Scripts/Core/States/StateManager.cs:89)
RPGMinesweeper.States.StateManager.HandleRoundAdvanced () (at Assets/Scripts/Core/States/StateManager.cs:126)
GameEvents.RaiseRoundAdvanced () (at Assets/Scripts/Core/Events/GameEvents.cs:53)
MineDebugger.AdvanceRound () (at Assets/Scripts/Debug/MineDebugger.cs:230)
MineDebugger.Update () (at Assets/Scripts/Debug/MineDebugger.cs:98)



---

_**Assistant**_

Let me help track down and fix this bug. The error indicates we're getting a `Collection was modified` exception during enumeration, which typically happens when trying to modify a collection while iterating over it.

Let me search for the relevant code first.

---

_****_

<details>
            <summary>Searched codebase "StateManager OnTurnEnd HandleRoundAdvanced" • **23** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/States/StateManager.cs` | L61-121 |
| `.specstory/history/integrating-frozenstate-with-turn-system.md` | L412-468 |
| `Assets/Scripts/Core/TurnSystem/PlayerTurn.cs` | L56-68 |
| `Assets/Scripts/Core/States/StateManager.cs` | L121-156 |
| `Assets/Scripts/Core/States/StateManager.cs` | L1-61 |
| `Assets/Scripts/Core/States/BaseTurnState.cs` | L1-36 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L1-64 |
| `.specstory/history/integrating-frozenstate-with-turn-system.md` | L364-412 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L64-130 |
| `.specstory/history/turn-management-system-integration-and-debugging.md` | L369-413 |
| `Assets/Scripts/Core/States/ITurnBasedState.cs` | L1-10 |
| `.specstory/history/turn-management-system-integration-and-debugging.md` | L321-369 |
| `Assets/Scripts/Core/TurnSystem/TurnManager.cs` | L130-174 |
| `Assets/Scripts/Core/States/TeleportState.cs` | L51-104 |
| `.specstory/history/integrating-frozenstate-with-turn-system.md` | L69-130 |
| `.specstory/history/fixing-teleport-effect-bug-in-game.md` | L339-405 |
| `.specstory/history/turn-management-system-integration-and-debugging.md` | L615-678 |
| `Assets/Scripts/Core/TurnSystem/PlayerTurn.cs` | L1-56 |
| `Assets/Scripts/Core/States/FrozenState.cs` | L52-109 |
| `Assets/Scripts/Core/States/BaseState.cs` | L1-51 |
| `.specstory/history/refactoring-freezeeffect-for-better-state-management.md` | L731-781 |
| `.specstory/history/turn-management-system-integration-and-debugging.md` | L794-849 |
| `.specstory/history/fixing-teleport-effect-bug-in-game.md` | L295-339 |

</details>

---

_****_



I see the issue. The error occurs because we're trying to modify a collection while iterating over it in the `StateManager.OnTurnEnd()` method. Let's fix this by making the following changes:

1. First, let's read the current implementation of the `StateManager` to confirm our understanding.

---

_****_

Read file: Assets/Scripts/Core/States/StateManager.cs

---

_****_



I see the issue. The problem is that we're modifying the collection while iterating over it in the `OnTurnEnd` method. Even though we're using a separate list for expired states, the issue might be occurring because some state's `OnTurnEnd` method is trying to modify the collection.

Let's fix this by:
1. Creating a safe copy of the states to process
2. Adding more debug logging to track what's happening
3. Making the state processing more robust

Here's the fix:

---

_****_

```diff
              }
  
+             // Create a safe copy of the states to process
+             var statesToProcess = m_ActiveStates.ToList();
              var expiredStates = new List<(string Name, StateTarget Target, object TargetId)>();
              
-             foreach (var kvp in m_ActiveStates)
-             {
-                 if (kvp.Value is ITurnBasedState turnState)
-                 {
-                     turnState.OnTurnEnd();
-                     if (turnState.IsExpired)
-                     {
-                         expiredStates.Add(kvp.Key);
-                         if (m_DebugMode)
-                         {
-                             Debug.Log($"[StateManager] State {turnState.Name} at {turnState.TargetId} has expired");
-                         }
-                     }
-                 }
-             }
- 
+             try
+             {
+                 foreach (var kvp in statesToProcess)
+                 {
+                     if (kvp.Value is ITurnBasedState turnState)
+                     {
+                         try
+                         {
+                             if (m_DebugMode)
+                             {
+                                 Debug.Log($"[StateManager] Processing turn end for state {turnState.Name} at {turnState.TargetId}");
+                             }
+                             
+                             turnState.OnTurnEnd();
+                             
+                             if (turnState.IsExpired)
+                             {
+                                 expiredStates.Add(kvp.Key);
+                                 if (m_DebugMode)
+                                 {
+                                     Debug.Log($"[StateManager] State {turnState.Name} at {turnState.TargetId} has expired");
+                                 }
+                             }
+                         }
+                         catch (System.Exception e)
+                         {
+                             Debug.LogError($"[StateManager] Error processing turn end for state {turnState.Name}: {e}");
+                             // Add to expired states to remove problematic state
+                             expiredStates.Add(kvp.Key);
+                         }
+                     }
+                 }
+             }
+             catch (System.Exception e)
+             {
+                 Debug.LogError($"[StateManager] Error during turn end processing: {e}");
+             }
+ 
+             // Remove expired states
              foreach (var key in expiredStates)
              {
-                 RemoveState(key);
+                 try
+                 {
+                     if (m_DebugMode)
+                     {
+                         Debug.Log($"[StateManager] Removing expired state {key.Name}");
+                     }
+                     RemoveState(key);
+                 }
+                 catch (System.Exception e)
+                 {
+                     Debug.LogError($"[StateManager] Error removing expired state {key.Name}: {e}");
+                 }
              }
          }
```

---

_****_



The changes I've made:

1. Created a safe copy of the states to process using `ToList()` to prevent collection modification issues
2. Added try-catch blocks around state processing to handle errors gracefully
3. Added more detailed debug logging to track the flow of state processing
4. Made the state removal process more robust with error handling
5. Added logging for state removal to better track what's happening

This should fix the collection modification exception by:
1. Preventing direct modification of the collection during iteration
2. Handling any errors that occur during state processing
3. Ensuring problematic states are removed safely

The error was occurring because something was trying to modify the `m_ActiveStates` collection while we were iterating over it. By creating a safe copy first and adding proper error handling, we should now be able to process the states safely.

Let me know if you'd like me to make any additional changes or if you need any clarification about the fix.

---



---

