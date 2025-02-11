# RPG Minesweeper Architecture

## Core Systems

### Grid System

```mermaid
classDiagram
class ICell {
<<interface>>
+Position: Vector2Int
+IsRevealed: bool
+Reveal()
+Reset()
}
class Cell {
-m_Position: Vector2Int
-m_IsRevealed: bool
+Reveal()
+Reset()
}
class Grid {
-m_Cells: ICell[,]
-m_Width: int
-m_Height: int
+GetCell(Vector2Int)
+IsValidPosition(Vector2Int)
}
ICell <|.. Cell
Grid -- ICell
```

### Mine System
```mermaid
classDiagram
class IMine {
<<interface>>
+Type: MineType
+CanDisguise: bool
+OnTrigger(Player)
+OnDestroy()
}
class BaseMine {
#m_Data: MineData
#m_Position: Vector2Int
#m_IsDestroyed: bool
#CalculateDamage(Player)
#ApplyEffect(EffectData)
}
class HealingMine {
+OnTrigger(Player)
#ApplyEffect(EffectData)
}
class ExperienceMine {
+CanDisguise: bool
+OnTrigger(Player)
}
class MultiTriggerMine {
-m_TriggerCount: int
-m_RequiredTriggers: int
+OnTrigger(Player)
}
class AreaRevealMine {
+OnTrigger(Player)
-RevealArea()
}
class MineValuePropagator {
+PropagateValues(MineManager, GridManager)
-PropagateValueFromMine(Vector2Int, MineData)
}
class MineManager {
-m_Mines: Dictionary
+HandleMineRemoval(Vector2Int)
+HasMineAt(Vector2Int)
}
IMine <|.. BaseMine
BaseMine <|-- HealingMine
BaseMine <|-- ExperienceMine
BaseMine <|-- MultiTriggerMine
BaseMine <|-- AreaRevealMine
MineManager --> IMine
MineManager --> MineValuePropagator
```
### Player System
```mermaid
classDiagram
class Player {
-m_Stats: PlayerStats
-m_BaseMaxHP: int
-m_HPIncreasePerLevel: int
+TakeDamage(int)
+GainExperience(int)
}
class PlayerStats {
-m_CurrentHP: int
-m_MaxHP: int
-m_Experience: int
-m_Level: int
+ModifyHP(int)
+AddExperience(int)
+SetMaxHP(int)
+RestoreFullHP()
}
Player -- PlayerStats

### Event System
```mermaid
classDiagram
class GameEvents {
<<static>>
+OnCellRevealed: Action<Vector2Int>
+OnMineTriggered: Action<MineType>
+OnEffectApplied: Action<Vector2Int>
+OnExperienceGained: Action<int>
+OnMineRemovalAttempted: Action<Vector2Int>
}
```

### Visualization System
```mermaid
classDiagram
    class CellView {
        -m_BackgroundRenderer: SpriteRenderer
        -m_MineRenderer: SpriteRenderer
        -m_Position: Vector2Int
        -m_ValueText: TextMeshPro
        -m_CurrentState: ICellState
        +UpdateVisuals(bool)
        +ShowMineSprite(Sprite)
        +SetValue(int)
        -SetupRenderers()
        -SetState(ICellState)
    }
    class MineDebugger {
        -m_MineManager: MineManager
        -m_CellViews: Dictionary
        -m_IsDebugMode: bool
        +ToggleDebugVisuals()
    }
    class GridManager {
        -m_CellObjects: GameObject[,]
        +GetCellObject(Vector2Int)
    }
    class MineManager {
        -m_MineDataMap: Dictionary
        +HasMineAt(Vector2Int)
    }
    MineDebugger --> CellView
    GridManager --> CellView
    MineManager --> CellView

    note for CellView "Manages sprite renderer states\nEnsures proper visual transitions"
```

## Managers

### Grid Management
```mermaid
classDiagram
class GridManager {
-m_Grid: Grid
-m_CellObjects: GameObject[,]
+CreateVisualGrid()
+CenterGrid()
}
class CameraSetup {
-m_GridManager: GridManager
-SetupCamera()
}
CameraSetup --> GridManager
```

### Mine Management
```mermaid
classDiagram
class MineManager {
-m_MineDatas: List<MineData>
-m_Mines: Dictionary<Vector2Int, IMine>
-PlaceMines()
-CreateMine(MineData, Vector2Int)
}
```