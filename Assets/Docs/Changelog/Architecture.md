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
class MineData {
+Type: MineType
+Damage: int
+TriggerRadius: float
+Effects: List~EffectData~
-m_MineSprite: Sprite
+MineSprite: Sprite
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
IMine <|.. BaseMine
BaseMine <|-- HealingMine
BaseMine <|-- ExperienceMine
BaseMine <|-- MultiTriggerMine
BaseMine <|-- AreaRevealMine
BaseMine --> MineData

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

### Player System
```mermaid
classDiagram
class Player {
-m_Stats: PlayerStats
-m_BaseMaxHP: int
-m_HPIncreasePerLevel: int
+Stats: PlayerStats
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
```

### UI System
```mermaid
classDiagram
class CharacterUIConfig {
+HPFormat: string
+EXPFormat: string
+LevelFormat: string
+HPColor: Color
+EXPColor: Color
+LevelColor: Color
+HPAnimationDuration: float
+EXPAnimationDuration: float
+LevelAnimationDuration: float
}
class CharacterUIView {
-m_Config: CharacterUIConfig
-m_PlayerStats: PlayerStats
-m_HPText: TextMeshProUGUI
-m_EXPText: TextMeshProUGUI
-m_LevelText: TextMeshProUGUI
+InitializeUI()
+UpdateHP(int)
+UpdateExperience(int)
+UpdateLevel(int)
}
CharacterUIView --> CharacterUIConfig
CharacterUIView --> PlayerStats
CharacterUIView --> TextMeshProUGUI
```

### Event System
mermaid
classDiagram
class GameEvents {
<<static>>
+OnCellRevealed: Action<Vector2Int>
+OnMineTriggered: Action<MineType>
+OnEffectApplied: Action<Vector2Int>
+OnExperienceGained: Action<int>
}
```

### Visualization System
```mermaid
classDiagram
class CellView {
-m_FrameRenderer: SpriteRenderer
-m_BackgroundRenderer: SpriteRenderer
-m_MineRenderer: SpriteRenderer
-m_Position: Vector2Int
-m_MineSprite: Sprite
-m_HasMine: bool
+Initialize(Vector2Int)
+SetMine(MineData)
+UpdateVisuals(bool)
+ApplyEffect()
}
class MineDebugger {
-m_MineHighlightColor: Color
-m_DebugKeyCode: KeyCode
-m_AllCells: CellView[]
-m_IsDebugVisible: bool
+ToggleMineVisibility()
}
MineDebugger --> CellView
CellView --> MineData
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