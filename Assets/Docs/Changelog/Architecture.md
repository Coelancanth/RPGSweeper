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
    +TargetType: EffectTargetType
    +Apply(GameObject, Vector2Int)
}
class IDurationalEffect {
    <<interface>>
    +Duration: float
    +Remove(GameObject, Vector2Int)
}
class IInstantEffect {
    <<interface>>
}
class ITickableEffect {
    <<interface>>
    +TickInterval: float
    +OnTick(GameObject, Vector2Int)
}
class EffectData {
    +Duration: float
    +Magnitude: float
    +Shape: GridShape
    +CreateEffect()
}
class ConfusionEffectData {
    +CreateEffect()
}
class TargetedRevealEffectData {
    -m_TargetMonsterType: MonsterType
    +CreateEffect()
}
class SummonEffectData {
    -m_MineType: MineType
    -m_MonsterType: MonsterType
    -m_Count: int
    +CreateEffect()
}
class SummonEffect {
    -m_Radius: float
    -m_Shape: GridShape
    -m_MineType: MineType
    -m_MonsterType: MonsterType
    -m_Count: int
    +Apply()
}
class MineData {
    -m_PassiveEffects: EffectInstance[]
    -m_ActiveEffects: EffectInstance[]
    +CreatePassiveEffects()
    +CreateActiveEffects()
}
class EffectInstance {
    +Template: EffectData
    +CreateEffect()
}
IEffect <|-- IDurationalEffect
IEffect <|-- IInstantEffect
IDurationalEffect <|-- ITickableEffect
EffectData <|-- ConfusionEffectData
EffectData <|-- TargetedRevealEffectData
EffectData <|-- SummonEffectData
IInstantEffect <|.. SummonEffect
MineData --> EffectInstance : Contains
EffectInstance --> EffectData : Uses template
note for EffectData "ScriptableObject-based\neffect configuration"
note for EffectInstance "Simple template reference\nfor effect creation"
note for MineData "Direct effect management\nwith ScriptableObjects"
note for SummonEffectData "Dynamic mine creation\nwith pattern support"
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