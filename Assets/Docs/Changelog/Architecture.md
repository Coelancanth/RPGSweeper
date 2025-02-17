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
class GridPositionType {
    <<enumeration>>
    None
    Random
    Edge
    Corner
    Center
    Source
    All
}
class GridShapeHelper {
    +GetAffectedPositions()
    +IsPositionAffected()
}
class GridManager {
    +Width: int
    +Height: int
    +IsValidPosition()
}
ICell <|.. Cell
Grid -- ICell
GridShapeHelper --> GridManager : Uses
GridShapeHelper --> GridPositionType : Uses
note for GridPositionType "Standardized position types\nfor grid-based operations"
note for GridShapeHelper "Handles position calculations\nand validation"
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
class MonsterMine {
+IsCollectable: bool
+CurrentHp: int
+HpPercentage: float
+OnTrigger(Player)
+OnDestroy()
}
class IMineSpawnStrategy {
<<interface>>
+GetSpawnPosition(GridManager, Dictionary)
}
class CompositeSpawnStrategy {
-m_Strategies: MineSpawnStrategyType
-m_StrategyMap: Dictionary
+GetSpawnPosition()
}
class MineSpawnStrategyType {
<<enumeration>>
None = 0
Random = 1
Edge = 2
Corner = 4
Center = 8
All = Random | Edge | Corner | Center
}
class MineData {
+Type: MineType
+Value: int
+Shape: MineShape
+SpawnStrategy: MineSpawnStrategyType
+Effects: EffectData[]
}
class MineManager {
-m_Mines: Dictionary
-m_SpawnStrategies: Dictionary
+HandleMineRemoval(Vector2Int)
+HandleMineAdd(Vector2Int, MineType, MonsterType?)
+HasMineAt(Vector2Int)
-GetSpawnPosition(MineData)
-FindMineDataByMonsterType(MonsterType)
}
IMine <|.. BaseMine
BaseMine <|-- MonsterMine
IMineSpawnStrategy <|.. CompositeSpawnStrategy
MineManager --> IMineSpawnStrategy
MineData --> MineSpawnStrategyType
CompositeSpawnStrategy --> MineSpawnStrategyType
note for MonsterMine "Two-phase removal:\n1. Combat to 0 HP\n2. Collection"
note for MineManager "Supports runtime mine addition\nwith type-specific handling"
note for CompositeSpawnStrategy "Handles multiple strategies\nwith flag composition"
```

### Player System
```mermaid
classDiagram
class Player {
    -m_Stats: PlayerStats
    -m_BaseMaxHP: int
    -m_HPIncreasePerLevel: int
    -m_CurrentShield: int
    +TakeDamage(int)
    +GainExperience(int)
    +AddShield(int)
    +RemoveShield(int)
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
class GameEvents {
    +OnShieldChanged: Action~int~
    +RaiseShieldChanged(int)
}
Player -- PlayerStats
Player --> GameEvents : Notifies shield changes

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
        -m_DisplayStrategy: IMineDisplayStrategy
        -m_DisplayConfig: MineDisplayConfig
        +UpdateVisuals(bool)
        +ShowMineSprite(Sprite, IMine, MineData)
        +SetValue(int, Color)
        +UpdateValueTextPosition()
    }
    class IMineDisplayStrategy {
        <<interface>>
        +SetupDisplay(GameObject, TextMeshPro)
        +UpdateDisplay(IMine, MineData, bool)
        +CleanupDisplay()
    }
    class MonsterMineDisplayStrategy {
        -m_StatsText: TextMeshPro
        -m_MineValueText: TextMeshPro
        -m_MonsterMine: MonsterMine
        +UpdateDisplay()
        -HandleHpChanged()
        -UpdateHPDisplay()
        -UpdateTextPositions()
    }
    class StandardMineDisplayStrategy {
        -m_ValueText: TextMeshPro
        +UpdateDisplay()
    }
    class MineDisplayConfig {
        +EmptyCellValuePosition: Vector3
        +StandardMineValuePosition: Vector3
        +MonsterMineValuePosition: Vector3
        +HPPosition: Vector3
        +DamagePosition: Vector3
        +DefaultValueColor: Color
        +MonsterPowerColor: Color
        +EnragedColor: Color
    }
    IMineDisplayStrategy <|.. MonsterMineDisplayStrategy
    IMineDisplayStrategy <|.. StandardMineDisplayStrategy
    CellView --> IMineDisplayStrategy
    CellView --> MineDisplayConfig
    MonsterMineDisplayStrategy --> MineDisplayConfig

    note for CellView "Handles collectable state\nand sprite transitions"
    note for IMineDisplayStrategy "Flexible display system\nfor different mine types"
    note for MineDisplayConfig "Distinct positions for\ndifferent cell types"
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

### Effect System
```mermaid
classDiagram
class IEffect {
<<interface>>
+Type: EffectType
+TargetType: EffectTargetType
+Duration: float
+Apply(GameObject, Vector2Int)
+Remove(GameObject, Vector2Int)
}
class IPassiveEffect {
<<interface>>
+OnTick(GameObject, Vector2Int)
}
class EffectTemplate {
-m_Template: EffectData
-m_Duration: float
-m_Magnitude: float
-m_Shape: GridShape
-m_Radius: int
-m_TargetMonsterType: MonsterType
+CreateInstance()
+OnValidate()
}
class EffectData {
+Type: EffectType
+Duration: float
+Magnitude: float
+Shape: GridShape
+CreateEffect()
}
class RangeRevealEffect {
-m_Radius: float
-m_Shape: GridShape
-m_TriggerPosition: Vector2Int?
-m_TriggerPositionType: GridPositionType
+Apply()
-GetEffectivePosition()
}
class SummonEffect {
-m_Radius: float
-m_Shape: GridShape
-m_MineType: MineType
-m_MonsterType: MonsterType
-m_Count: int
-m_TriggerPosition: Vector2Int?
-m_TriggerPositionType: GridPositionType
+Apply()
-GetEffectivePosition()
}
class ConfusionEffect {
-m_Shape: GridShape
-m_AffectedCells: HashSet
+Apply()
+Remove()
+OnTick()
}

[Update the Effect System section in Architecture.md with:]

class SplitEffect {
    -m_HealthModifier: float
    -m_SplitCount: int
    -CalculateNewHP()
    -ValidateHPRatio()
    +Apply(GameObject, Vector2Int)
}

IEffect <|-- IPassiveEffect
IPassiveEffect <|.. ConfusionEffect
IEffect <|.. RangeRevealEffect
IEffect <|.. SummonEffect
EffectTemplate --> EffectData : References
EffectData --> IEffect : Creates
note for RangeRevealEffect "Configurable trigger positions\nfor revealing cells"
note for SummonEffect "Flexible mine placement\nwith position selection"
note for SplitEffect "HP calculation: H * k / n\nwith ratio validation"
```

### Value Modification System
```mermaid
classDiagram
class MineValueModifier {
-s_ActiveEffects: Dictionary<Vector2Int, HashSet<IEffect>>
+RegisterEffect(Vector2Int, IEffect)
+UnregisterEffect(Vector2Int, IEffect)
+ModifyValue(Vector2Int, int)
+ModifyValueAndGetColor(Vector2Int, int)
+Clear()
}
class MineValuePropagator {
+PropagateValues(MineManager, GridManager)
-PropagateValueFromMine()
}
class IEffect {
<<interface>>
+Type: EffectType
+Apply()
+Remove()
}
MineValuePropagator --> MineValueModifier
IEffect --> MineValueModifier
note for MineValueModifier "Centralized effect and color management"
```