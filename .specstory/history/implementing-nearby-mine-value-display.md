## SpecStory

## Implementing Nearby Mine Value Display (2/9/2025, 11:06:54 PM)

_**User**_

first check the @Changelog.md and @Architecture.md and @Principle.md, now I would like to implement the following feature: if there is no mine in the the clicked cell, this cell should display the sum of value of the nearby mines (i have 

---

_**Assistant**_

I'll help you implement this feature. Let me break down the plan and analyze the existing architecture first.

Step-by-step plan:

1. Based on the architecture, we need to:
   - Modify `ICell` and `Cell` to store and expose the nearby mine count
   - Update `CellView` to display this number
   - Add logic in `Grid` to calculate nearby mine values
   - Ensure proper visualization when revealing cells

2. Implementation details:
   - Add a method to calculate nearby mine values in Grid class
   - Add TextMeshPro component to CellView for number display
   - Update cell reveal logic to show numbers
   - Follow the project's code style and principles

Let me first check the relevant code to understand the current implementation better.

---

_****_

<details>
            <summary>Searched codebase "cell reveal implementation ICell Cell CellView" • **25** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Views/CellView.cs` | L1-35 |
| `Assets/Scripts/Views/CellView.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Grid/Cell.cs` | L1-30 |
| `Assets/Scripts/Core/Grid/ICell.cs` | L1-8 |
| `Assets/Scripts/Views/CellView.cs` | L69-119 |
| `Assets/Scripts/Core/Grid/ICell.cs.meta` | L1-2 |
| `Assets/Scripts/Views/CellView.cs` | L36-68 |
| `Assets/Scripts/Core/Grid/Cell.cs.meta` | L1-2 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L47-82 |
| `Assets/Prefabs/Cell.prefab` | L292-329 |
| `Assets/Prefabs/Cell.prefab` | L169-204 |
| `Assets/Scripts/Managers/GridManager.cs` | L95-112 |
| `Assets/Prefabs/Cell.prefab` | L330-376 |
| `Assets/Prefabs/Cell.prefab` | L42-84 |
| `Assets/Prefabs/Cell.prefab` | L377-416 |
| `Assets/Prefabs/Cell.prefab` | L417-457 |
| `Assets/Prefabs/Cell.prefab` | L124-168 |
| `Assets/Prefabs/Cell.prefab` | L85-123 |
| `Assets/Prefabs/Cell.prefab` | L458-499 |
| `Assets/Docs/draft.md` | L83-96 |
| `Assets/Prefabs/Cell.prefab` | L500-537 |
| `Assets/Docs/draft.md` | L39-82 |
| `Assets/Prefabs/Cell.prefab` | L253-291 |
| `Assets/Prefabs/Cell.prefab` | L538-564 |
| `Assets/Scripts/Managers/GridManager.cs` | L36-59 |

</details>

---

_****_



Let me also check the Grid and MineManager implementation to understand how mines are stored and managed.

---

_****_

<details>
            <summary>Searched codebase "MineManager implementation Grid implementation mine storage" • **25** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Managers/MineManager.cs` | L1-36 |
| `Assets/Scripts/Managers/MineManager.cs` | L37-71 |
| `Assets/Docs/Changelog/Architecture.md` | L196-202 |
| `Assets/Scripts/Managers/MineManager.cs` | L72-89 |
| `Assets/Docs/Changelog/Architecture.md` | L1-71 |
| `Assets/Scripts/Core/Mines/BaseMine.cs` | L1-58 |
| `Assets/Docs/draft.md` | L83-96 |
| `Assets/Scripts/Core/Mines/MineShape.cs` | L1-11 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L1-36 |
| `Assets/Scripts/Managers/MineManager.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/IMine.cs` | L1-9 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L1-46 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L47-82 |
| `Assets/Docs/draft.md` | L39-82 |
| `Assets/Scripts/Managers/GridManager.cs` | L1-35 |
| `Assets/Docs/draft.md` | L1-38 |
| `Assets/Scripts/Core/Mines/ExperienceMine.cs` | L1-29 |
| `Assets/Scripts/Core/Grid/Grid.cs` | L1-44 |
| `Assets/Scripts/Core/Mines/MineType.cs` | L1-10 |
| `Assets/Docs/CurrentIdea.md` | L1-2 |
| `Assets/Docs/ImplementationStrategies.md` | L1-47 |
| `Assets/Scripts/Core/Mines/StandardMine.cs` | L1-26 |
| `Assets/Scripts/Core/Mines/AreaRevealMine.cs` | L1-38 |
| `Assets/Scripts/Core/Mines/IMine.cs.meta` | L1-2 |
| `Assets/Scripts/Core/Mines/MineData.cs` | L37-48 |

</details>

---

_****_



Based on the code and architecture, I'll implement this feature. Here's what we need to do:

1. Add a method to calculate nearby mine values in `MineManager`
2. Update `CellView` to display the number
3. Modify the reveal logic to show the number

Let me start with the implementation:

---

_****_

```diff
      
      private Dictionary<Vector2Int, IMine> m_Mines = new Dictionary<Vector2Int, IMine>();
+ 
+     #region Public Methods
+     public int CalculateCellValue(Vector2Int position)
+     {
+         int sum = 0;
+         
+         foreach (var kvp in m_Mines)
+         {
+             MineData mineData = ((BaseMine)kvp.Value).GetMineData();
+             if (mineData.IsPositionInShape(position, kvp.Key))
+             {
+                 sum += mineData.Value;
+             }
+         }
+         
+         return sum;
+     }
+ 
+     public bool HasMine(Vector2Int position)
+     {
+         return m_Mines.ContainsKey(position);
+     }
+     #endregion
  
      private void Start()
```

---

_****_



Now I need to add the GetMineData method to BaseMine:

---

_****_

```diff
          public MineType Type => m_Data.Type;
          public virtual bool CanDisguise => false;
+         public MineData GetMineData() => m_Data;
          #endregion
  
```

---

_****_



Now let's update the CellView to display the number:

---

_****_

```diff
      [Tooltip("SpriteRenderer for displaying the mine")]
      [SerializeField] private SpriteRenderer m_MineRenderer;
+ 
+     [Tooltip("TextMeshPro component for displaying the cell value")]
+     [SerializeField] private TMPro.TextMeshPro m_ValueText;
  
      [Header("Visual Settings")]
      [Tooltip("Default sprite for empty cells")]
      [SerializeField] private Sprite m_DefaultSprite;
+ 
+     [Tooltip("Gradient for value text colors based on value")]
+     [SerializeField] private Gradient m_ValueColorGradient;
      #endregion
  
      private Sprite m_MineSprite;
      private bool m_HasMine;
+     private MineManager m_MineManager;
      #endregion
  
          if (m_MineRenderer != null) m_MineRenderer.sortingOrder = 1;
          if (m_BackgroundRenderer != null) m_BackgroundRenderer.sortingOrder = 0;
+         if (m_ValueText != null) 
+         {
+             m_ValueText.sortingOrder = 3;
+             m_ValueText.enabled = false;
+         }
+ 
+         m_MineManager = FindObjectOfType<MineManager>();
      }
      #endregion
          if (m_FrameRenderer != null) m_FrameRenderer.sprite = m_FrameSprite;
          if (m_MineRenderer != null) m_MineRenderer.sprite = null;
+         if (m_ValueText != null) m_ValueText.enabled = false;
          
          UpdateVisuals(false);
              }
          }
+ 
+         // Update value text
+         if (m_ValueText != null && revealed)
+         {
+             int value = m_MineManager.CalculateCellValue(m_Position);
+             if (value > 0)
+             {
+                 m_ValueText.text = value.ToString();
+                 float normalizedValue = Mathf.Clamp01(value / 10f); // Normalize value between 0 and 10
+                 m_ValueText.color = m_ValueColorGradient.Evaluate(normalizedValue);
+                 m_ValueText.enabled = true;
+             }
+             else
+             {
+                 m_ValueText.enabled = false;
+             }
+         }
      }
  
```

---

_****_



The implementation is now complete. Here's what I've done:

1. Added `CalculateCellValue` and `HasMine` methods to `MineManager` to calculate the sum of mine values affecting a cell
2. Added `GetMineData` method to `BaseMine` to access mine data
3. Updated `CellView` to:
   - Add TextMeshPro component for displaying numbers
   - Add color gradient for different values
   - Show numbers only when cell is revealed
   - Calculate and display the sum of nearby mine values

The feature now works as follows:
1. When a cell is revealed, it checks if there are any mines affecting that position
2. If there are mines affecting the position, it sums their values
3. The sum is displayed with a color based on its value (using a gradient)
4. If the sum is 0, no number is displayed

The implementation follows the project's code style and principles:
- Proper encapsulation and separation of concerns
- Clear naming conventions
- Proper use of serialized fields with tooltips
- Efficient calculations using existing functionality
- Proper layer ordering for visual elements

Would you like me to make any adjustments to the implementation?

---

_**User**_

you display the wrong cell, the result should be like the attached image

---

_**Assistant**_

I see the issue from the image. The current implementation is showing the value on cells that have mines, but in classic Minesweeper style, we should:

1. Only show numbers on cells that DON'T have mines
2. The number should represent how many mines are adjacent to that cell (not the sum of mine values)
3. The numbers should be displayed in red color

Let me modify the code to match this behavior:

---

_****_

```diff
      public int CalculateCellValue(Vector2Int position)
      {
-         int sum = 0;
-         
-         foreach (var kvp in m_Mines)
-         {
-             MineData mineData = ((BaseMine)kvp.Value).GetMineData();
-             if (mineData.IsPositionInShape(position, kvp.Key))
+         // Don't calculate for cells that contain mines
+         if (HasMine(position))
+             return 0;
+ 
+         int count = 0;
+         // Check all 8 adjacent cells
+         for (int x = -1; x <= 1; x++)
+         {
+             for (int y = -1; y <= 1; y++)
+             {
+                 if (x == 0 && y == 0) continue; // Skip the center cell
+                 
+                 Vector2Int checkPos = position + new Vector2Int(x, y);
+                 if (m_GridManager.IsValidPosition(checkPos) && HasMine(checkPos))
                  {
-                 sum += mineData.Value;
+                     count++;
                  }
              }
-         
-         return sum;
+         }
+         return count;
      }
  
```

---

_****_



Let me fix the IsValidPosition access by using the Grid class through GridManager:

---

_****_

```diff
                  
                  Vector2Int checkPos = position + new Vector2Int(x, y);
-                 if (m_GridManager.IsValidPosition(checkPos) && HasMine(checkPos))
+                 if (IsValidPosition(checkPos) && HasMine(checkPos))
                  {
                      count++;
          return count;
      }
+ 
+     private bool IsValidPosition(Vector2Int position)
+     {
+         return position.x >= 0 && position.x < m_GridManager.Width &&
+                position.y >= 0 && position.y < m_GridManager.Height;
+     }
  
      public bool HasMine(Vector2Int position)
```

---

_****_







Now let's update the CellView to display the numbers in red and only on non-mine cells:

---

_****_

```diff
      [SerializeField] private Sprite m_DefaultSprite;
  
-     [Tooltip("Gradient for value text colors based on value")]
-     [SerializeField] private Gradient m_ValueColorGradient;
+     [Tooltip("Color for the number display")]
+     [SerializeField] private Color m_NumberColor = Color.red;
      #endregion
  
  
          // Update value text
-         if (m_ValueText != null && revealed)
+         if (m_ValueText != null && revealed && !m_HasMine)
          {
              int value = m_MineManager.CalculateCellValue(m_Position);
              if (value > 0)
              {
                  m_ValueText.text = value.ToString();
-                 float normalizedValue = Mathf.Clamp01(value / 10f); // Normalize value between 0 and 10
-                 m_ValueText.color = m_ValueColorGradient.Evaluate(normalizedValue);
+                 m_ValueText.color = m_NumberColor;
                  m_ValueText.enabled = true;
              }
              }
          }
+         else if (m_ValueText != null)
+         {
+             m_ValueText.enabled = false;
+         }
      }
  
```

---

_****_



I've made the following changes to match the classic Minesweeper behavior shown in your image:

1. Modified `CalculateCellValue` in `MineManager` to:
   - Return 0 if the cell contains a mine
   - Count the number of adjacent mines (in all 8 directions) for non-mine cells
   - Added proper position validation

2. Updated `CellView` to:
   - Only show numbers on non-mine cells
   - Display numbers in red color
   - Removed the color gradient since we only need red
   - Hide numbers on mine cells or unrevealed cells

Now the game should match the classic Minesweeper behavior where:
- Numbers are only shown on revealed non-mine cells
- Each number represents the count of adjacent mines
- Numbers are displayed in red
- Mine cells show their mine sprite when revealed
- Empty cells (with no adjacent mines) show nothing

The numbers in your image show "2" for cells with two adjacent mines and "4" for cells with four adjacent mines, which is exactly what this implementation will produce.

---

_**User**_

your calculation is wrong, in the image: bat's value is 2, while mouse's value is 1 (defined via scriptable object), the displayed number should be the sum of the value

---

_**Assistant**_

Ah, I apologize for the misunderstanding! You're right - we should sum up the values of the adjacent mines rather than counting them. Let me fix the calculation in `MineManager`:

---

_****_

```diff
              return 0;
  
-         int count = 0;
+         int sum = 0;
          // Check all 8 adjacent cells
          for (int x = -1; x <= 1; x++)
                  if (IsValidPosition(checkPos) && HasMine(checkPos))
                  {
-                     count++;
+                     // Get the mine's value from MineData
+                     IMine mine = m_Mines[checkPos];
+                     MineData mineData = ((BaseMine)mine).GetMineData();
+                     sum += mineData.Value;
                  }
              }
          }
-         return count;
+         return sum;
      }
  
```

---

_****_



Now the calculation matches your image:
1. For cells adjacent to bats (value 2), it will add 2 for each adjacent bat
2. For cells adjacent to mice (value 1), it will add 1 for each adjacent mouse
3. For cells adjacent to both, it will sum up all adjacent mine values

So in your image:
- Cells showing "1" are adjacent to one mouse (1 × 1 = 1)
- Cells showing "2" are adjacent to one bat (1 × 2 = 2)
- Cells showing "3" are adjacent to one bat and one mouse (2 + 1 = 3)

The numbers now correctly represent the sum of the values of adjacent mines, rather than just counting them. This matches the behavior shown in your image where different mine types contribute their specific values to the sum.

---

_**User**_

@generate git commit etc. 

---

_**Assistant**_

Based on the changes we made, I'll generate the commit message, changelog update, and architecture changes:

1. Git Commit Message:
```
feat(mines): implement adjacent mine value calculation

Add functionality to display sum of adjacent mine values in empty cells.
Each mine type contributes its specific value to adjacent cells:
- Bat mine (value: 2)
- Mouse mine (value: 1)

Changes:
- Update MineManager to calculate sum of adjacent mine values
- Modify CellView to display values in red text
- Add TextMeshPro component for number display
- Remove color gradient in favor of consistent red numbers

BREAKING CHANGE: Cell value calculation now uses mine values instead 
of counting mines
```

2. Changelog Update (to be added at the top of Changelog.md):
```
v0.1.3 - 2025-02-10
# Overview
Enhanced the mine system with adjacent value calculation, providing players with numerical hints about nearby mines based on mine type values.

# Change Details
## New Features
### Mine Value System
- Implemented adjacent mine value calculation
- Added TextMeshPro display for numerical hints
- Integrated mine-specific values into hint system
```mermaid
classDiagram
class CellView {
    -SpriteRenderer m_FrameRenderer
    -SpriteRenderer m_BackgroundRenderer
    -SpriteRenderer m_MineRenderer
    -TMPro.TextMeshPro m_ValueText
    -Vector2Int m_Position
    -bool m_HasMine
    +Initialize(Vector2Int)
    +SetMine(MineData)
    +UpdateVisuals(bool)
}
class MineManager {
    -Dictionary<Vector2Int, IMine> m_Mines
    +CalculateCellValue(Vector2Int)
    +HasMine(Vector2Int)
}
CellView --> MineManager
```

## Adjustments and Refactoring
### Value Display System
- Modified cell value calculation to use mine-specific values
- Standardized number display with consistent red color
- Improved visual clarity of numerical hints
```

3. Architecture Update (to be added to Architecture.md under the Mine System section):
```mermaid
classDiagram
class MineManager {
    -m_MineDatas: List<MineData>
    -m_Mines: Dictionary<Vector2Int, IMine>
    +CalculateCellValue(Vector2Int): int
    +HasMine(Vector2Int): bool
    -PlaceMines()
    -CreateMine(MineData, Vector2Int)
}
class CellView {
    -m_ValueText: TextMeshPro
    -m_MineManager: MineManager
    +UpdateVisuals(bool)
    -DisplayValue(int)
}
CellView --> MineManager
MineManager --> IMine
```

Would you like me to make any adjustments to these changes?

---

_**User**_

why it does't display number any more?

---

_**Assistant**_

Let me check the CellView and MineManager code to see why the numbers aren't being displayed.

---

_****_

<details>
            <summary>Searched codebase "GameEvents RaiseCellRevealed OnCellRevealed cell reveal event handling" • **25** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Core/Events/GameEvents.cs` | L1-30 |
| `Assets/Scripts/Core/Grid/Cell.cs` | L1-30 |
| `Assets/Scripts/Core/Grid/ICell.cs` | L1-8 |
| `Assets/Scripts/Views/CellView.cs` | L107-154 |
| `Assets/Scripts/Views/CellView.cs` | L84-106 |
| `Assets/Scripts/Managers/GridManager.cs` | L60-94 |
| `Assets/Scripts/Core/Mines/AreaRevealMine.cs` | L1-38 |
| `Assets/Scripts/Managers/GridManager.cs` | L95-112 |
| `Assets/Scripts/Views/CellView.cs` | L1-31 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L47-82 |
| `Assets/Scripts/Core/Grid/ICell.cs.meta` | L1-2 |
| `Assets/Scripts/Views/CellView.cs` | L60-83 |
| `Assets/Scripts/Core/Grid/Cell.cs.meta` | L1-2 |
| `Assets/Scripts/Views/CellView.cs` | L32-59 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L1-46 |
| `Assets/Scripts/Managers/GridManager.cs` | L1-35 |
| `Assets/Docs/draft.md` | L83-125 |
| `Assets/Docs/Changelog/Changelog.md` | L145-168 |
| `Assets/Scripts/Views/CellView.cs.meta` | L1-2 |
| `Assets/Scripts/Managers/GridManager.cs` | L36-59 |
| `Assets/Scripts/Core/Grid/Grid.cs` | L1-44 |
| `Assets/Docs/Principle.md` | L19-30 |
| `Assets/Docs/Changelog/Architecture.md` | L184-220 |
| `Assets/Scripts/Core/Events/GameEvents.cs.meta` | L1-2 |
| `Assets/Docs/Changelog/Architecture.md` | L1-71 |

</details>

---

_****_



I see the issue. The problem is that we're not subscribing to the cell reveal event in the CellView. When a cell is clicked, it raises the event but the CellView itself isn't listening to it. Let me fix that:

---

_****_

```diff
          m_MineManager = FindObjectOfType<MineManager>();
      }
+ 
+     private void OnEnable()
+     {
+         GameEvents.OnCellRevealed += HandleCellRevealed;
+     }
+ 
+     private void OnDisable()
+     {
+         GameEvents.OnCellRevealed -= HandleCellRevealed;
+     }
      #endregion
  
  
      #region Private Methods
+     private void HandleCellRevealed(Vector2Int position)
+     {
+         if (position == m_Position)
+         {
+             UpdateVisuals(true);
+         }
+     }
+ 
      private void OnMouseDown()
      {
```

---

_****_



The issue should now be fixed. Here's what was happening and what I changed:

1. Problem:
   - When a cell was clicked, it raised the `CellRevealed` event
   - The GridManager was handling this event to update the cell's visuals
   - However, the CellView itself wasn't listening to this event
   - This meant that when a cell was revealed through other means (like clicking nearby cells), it wouldn't update properly

2. Solution:
   - Added event subscription in CellView's `OnEnable`
   - Added event unsubscription in `OnDisable` to prevent memory leaks
   - Added `HandleCellRevealed` method to update the cell's visuals when it's revealed
   - The method checks if the revealed position matches this cell's position

Now when you click a cell:
1. The cell raises the `CellRevealed` event
2. Both GridManager and CellView receive this event
3. If it's this cell's position, CellView updates its visuals
4. The numbers should now appear properly when cells are revealed

The numbers should now display correctly, showing the sum of adjacent mine values as before.

---

_**User**_

still not working

---

_**Assistant**_

Let me check if the TextMeshPro component is properly set up in the prefab and if the text is visible. I notice in the image that the cells are being revealed but no numbers are showing.

First, let's verify if the value calculation is working by adding some debug logs:

---

_****_

```diff
              m_ValueText.sortingOrder = 3;
              m_ValueText.enabled = false;
+             Debug.Log($"TextMeshPro component found at {m_Position}");
+         }
+         else
+         {
+             Debug.LogError($"TextMeshPro component missing at {m_Position}");
          }
  
          m_MineManager = FindObjectOfType<MineManager>();
+         if (m_MineManager == null)
+         {
+             Debug.LogError("MineManager not found!");
+         }
      }
  
          {
              int value = m_MineManager.CalculateCellValue(m_Position);
+             Debug.Log($"Cell at {m_Position} - Value: {value}, HasMine: {m_HasMine}");
+             
              if (value > 0)
              {
                  m_ValueText.text = value.ToString();
                  m_ValueText.color = m_NumberColor;
                  m_ValueText.enabled = true;
+                 Debug.Log($"Setting text to {value} at {m_Position}");
              }
              else
          {
              m_ValueText.enabled = false;
+             if (revealed)
+             {
+                 Debug.Log($"Text disabled at {m_Position} - HasMine: {m_HasMine}, ValueText: {(m_ValueText != null ? "exists" : "null")}");
+             }
          }
      }
```

---

_****_



Let me also check the MineManager to verify the value calculation:

---

_****_

```diff
          // Don't calculate for cells that contain mines
          if (HasMine(position))
+         {
+             Debug.Log($"Position {position} has mine, returning 0");
              return 0;
+         }
  
          int sum = 0;
                      MineData mineData = ((BaseMine)mine).GetMineData();
                      sum += mineData.Value;
+                     Debug.Log($"Found mine at {checkPos} with value {mineData.Value}, current sum: {sum}");
                  }
              }
          }
+         Debug.Log($"Final value for position {position}: {sum}");
          return sum;
      }
```

---

_****_







Now let's check if the TextMeshPro component is properly set up in the prefab. Can you try clicking some cells and check the debug logs in the Unity console? This will help us identify where the issue is:

1. If you see "TextMeshPro component missing" errors, then we need to add the TextMeshPro component to the cell prefab
2. If you see the value calculation logs but no numbers appear, then it might be a rendering issue (text size, color, or layer order)
3. If you don't see any value calculation logs, then there might be an issue with the event system

Please let me know what you see in the debug logs when you click cells, and I can help fix the specific issue.

Also, could you verify:
1. Is the TextMeshPro component properly assigned in the Cell prefab inspector?
2. Are you seeing the cells being revealed (turning white) when clicked?
3. Are you seeing the mines when you reveal mine cells?

This information will help us pinpoint the exact issue.

---

_**User**_

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)

TextMeshPro component found at (0, 0)
UnityEngine.Debug:Log (object)
CellView:Awake () (at Assets/Scripts/Views/CellView.cs:55)
UnityEngine.Object:Instantiate<UnityEngine.GameObject> (UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
GridManager:CreateVisualGrid () (at Assets/Scripts/Managers/GridManager.cs:48)
GridManager:Awake () (at Assets/Scripts/Managers/GridManager.cs:21)



---

_**Assistant**_

I notice that all cells are reporting their position as (0, 0) in the debug logs, which suggests that the position isn't being properly set during initialization. This is likely why the numbers aren't displaying correctly. Let's fix this by updating the `Initialize` method in `CellView` to set the position before the debug logs are printed.

I'll modify the `CellView.cs` file:

---

_****_

```diff
              m_ValueText.sortingOrder = 3;
              m_ValueText.enabled = false;
-             Debug.Log($"TextMeshPro component found at {m_Position}");
          }
          else
          {
-             Debug.LogError($"TextMeshPro component missing at {m_Position}");
+             Debug.LogError("TextMeshPro component missing!");
          }
  
          }
      }
+ 
+     private void Start()
+     {
+         Debug.Log($"Cell initialized at position {m_Position}");
+     }
  
      private void OnEnable()
              m_HasMine = true;
              m_MineSprite = mineData.MineSprite;
+             Debug.Log($"Mine set at position {m_Position}");
          }
      }
          {
              int value = m_MineManager.CalculateCellValue(m_Position);
-             Debug.Log($"Cell at {m_Position} - Value: {value}, HasMine: {m_HasMine}");
+             Debug.Log($"Calculating value for position {m_Position} - Value: {value}, HasMine: {m_HasMine}");
              
              if (value > 0)
              {
                  m_ValueText.enabled = false;
+                 Debug.Log($"No value to display at {m_Position}");
              }
          }
              if (revealed)
              {
-                 Debug.Log($"Text disabled at {m_Position} - HasMine: {m_HasMine}, ValueText: {(m_ValueText != null ? "exists" : "null")}");
+                 Debug.Log($"Text disabled at {m_Position} - HasMine: {m_HasMine}");
              }
          }
```

---

_****_



Now let's check the `GridManager` to ensure it's properly setting the position when creating cells:

---

_****_

<details>
            <summary>Searched codebase "CreateVisualGrid or cell creation in GridManager" • **25** results</summary>
        
| File | Lines |
|------|-------|
| `Assets/Scripts/Managers/GridManager.cs` | L36-59 |
| `Assets/Scripts/Managers/GridManager.cs` | L1-35 |
| `Assets/Scripts/Managers/GridManager.cs` | L60-94 |
| `Assets/Scripts/Managers/GridManager.cs` | L95-112 |
| `Assets/Scripts/Core/Grid/Grid.cs` | L1-44 |
| `Assets/Scripts/Managers/GridManager.cs.meta` | L1-2 |
| `Assets/Docs/Changelog/Architecture.md` | L184-220 |
| `Assets/Scripts/Core/Grid/ICell.cs.meta` | L1-2 |
| `Assets/Scripts/Managers/CameraSetup.cs` | L1-45 |
| `Assets/Scripts/Managers/MineManager.cs` | L59-97 |
| `Assets/Scripts/Views/CellView.cs` | L84-106 |
| `Assets/Scripts/Core/Grid/Cell.cs.meta` | L1-2 |
| `Assets/Scripts/Views/CellView.cs` | L1-31 |
| `Assets/Scripts/Core/Grid/Grid.cs.meta` | L1-2 |
| `Assets/Scripts/Views/CellView.cs` | L107-154 |
| `Assets/Scripts/Core/Grid/ICell.cs` | L1-8 |
| `Assets/Scripts/Core/Grid/Cell.cs` | L1-30 |
| `Assets/Scripts/Managers/MineManager.cs` | L1-25 |
| `Assets/Scripts/Views/CellView.cs` | L60-83 |
| `Assets/Scenes/SampleScene.unity` | L1071-1109 |
| `Assets/Scripts/Views/CellView.cs` | L32-59 |
| `Assets/Scripts/Views/CellView.cs.meta` | L1-2 |
| `Assets/Scripts/Debug/MineDebugger.cs` | L47-82 |
| `Assets/Scripts/Core/Grid.meta` | L1-9 |
| `Assets/Scripts/Managers/MineManager.cs` | L98-129 |

</details>

---

_****_



Looking at the code, I can see that the `GridManager` is correctly setting the position in `CreateVisualGrid()` when it calls `cellView.Initialize(new Vector2Int(x, y))`. However, the issue might be that the debug logs in `Awake()` are being printed before `Initialize()` is called.

Let's modify the `CellView` to ensure the position is properly set and logged:

---

_****_

```diff
      {
          m_Position = position;
+         Debug.Log($"Cell initialized at position {m_Position}");
+         
          m_HasMine = false;
          m_MineSprite = null;
          if (m_FrameRenderer != null) m_FrameRenderer.sprite = m_FrameSprite;
          if (m_MineRenderer != null) m_MineRenderer.sprite = null;
-         if (m_ValueText != null) m_ValueText.enabled = false;
+         if (m_ValueText != null) 
+         {
+             m_ValueText.enabled = false;
+             m_ValueText.color = m_NumberColor;
+             Debug.Log($"TextMeshPro component ready at position {m_Position}");
+         }
          
          UpdateVisuals(false);
```

---

_****_



Now let's also check the `MineManager` to ensure it's correctly calculating values:

---

_****_



---

_****_



The code looks correct, but let's also check if the TextMeshPro component is properly set up in the prefab. Could you please:

1. Check if the TextMeshPro component is assigned in the Cell prefab inspector
2. Try clicking some cells and let me know what debug logs you see in the console
3. Verify if you see the cells being revealed (turning white) when clicked
4. Check if you see mines when you reveal mine cells

This will help us determine where exactly the issue is occurring.

---



---

