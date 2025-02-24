using UnityEngine;
using Sirenix.OdinInspector;
using RPGMinesweeper;
using RPGMinesweeper.Core.Mines.Spawning;
using System.Collections.Generic;

namespace RPGMinesweeper.Core.Mines
{
    [System.Serializable]
    public class MineTypeSpawnData
    {
        [FoldoutGroup("$GroupName")]
        [HorizontalGroup("$GroupName/Header")]
        [VerticalGroup("$GroupName/Header/Left"), LabelWidth(100)]
        [PreviewField(45)]
        [ShowInInspector, ReadOnly]
        public Sprite Preview => MineData?.MineSprite;

        [VerticalGroup("$GroupName/Header/Left")]
        [LabelText("$GetMineTypeName")]
        [Required]
        public MineData MineData;

        [FoldoutGroup("$GroupName")]
        [TextArea(1, 3)]
        [PropertyOrder(-1)]
        [LabelText("Description")]
        public string Description;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [HorizontalGroup("$GroupName/Spawn Settings/Split")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left"), LabelWidth(100)]
        [MinValue(1)]
        [Tooltip("Number of mines to spawn")]
        public int SpawnCount = 1;

        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [ValueDropdown("GetSpawnStrategies")]
        [OnValueChanged("OnSpawnStrategyChanged")]
        [Tooltip("Strategy to use when spawning this mine type")]
        public SpawnStrategyType SpawnStrategy = SpawnStrategyType.Random;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Surrounded")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [EnumToggleButtons]
        [OnValueChanged("OnTargetMineTypeChanged")]
        [Tooltip("The type of mine this mine should be surrounded by")]
        public MineType TargetMineType = MineType.Monster;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Surrounded && TargetMineType == MineType.Monster")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [EnumToggleButtons]
        [Tooltip("The specific type of monster to be surrounded by")]
        public MonsterType TargetMonsterType = MonsterType.None;

        // Symmetric Strategy Settings
        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Symmetric")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [EnumToggleButtons]
        [OnValueChanged("OnSymmetryDirectionChanged")]
        [Tooltip("The direction of symmetry")]
        public SymmetryDirection SymmetryDirection = SymmetryDirection.AroundVerticalLine;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Symmetric")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [Range(0, 1)]
        [OnValueChanged("OnSymmetryLinePositionChanged")]
        [InfoBox("$GetSymmetryLineInfo", InfoMessageType.None)]
        [Tooltip("Position of the symmetry line (0-1 range)")]
        public float SymmetryLinePosition = 0.5f;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Symmetric")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [MinValue(0)]
        [Tooltip("Minimum distance from the symmetry line")]
        public int MinDistanceToLine = 0;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Symmetric")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [MinValue(0)]
        [Tooltip("Maximum distance from the symmetry line (0 for no limit)")]
        public int MaxDistanceToLine = 0;

        // Adjacent Strategy Settings
        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Adjacent")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [EnumToggleButtons]
        [Tooltip("The primary direction for adjacent mines")]
        public AdjacentDirection AdjacencyDirection = AdjacentDirection.Vertical;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Adjacent || SpawnStrategy == SpawnStrategyType.Neighbor")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [MinValue(1)]
        [Tooltip("Maximum distance between adjacent mines")]
        public int MaxDistance = 1;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Adjacent || SpawnStrategy == SpawnStrategyType.Neighbor")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [ToggleLeft]
        [Tooltip("Allow diagonal adjacency between mines")]
        public bool AllowDiagonal = true;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Adjacent || SpawnStrategy == SpawnStrategyType.Neighbor")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [MinValue(1)]
        [Tooltip("Minimum number of mines in a cluster")]
        public int MinClusterSize = 2;

        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Adjacent || SpawnStrategy == SpawnStrategyType.Neighbor")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [MinValue(1)]
        [Tooltip("Maximum number of mines in a cluster")]
        public int MaxClusterSize = 4;

        // Neighbor Strategy Settings
        [FoldoutGroup("$GroupName/Spawn Settings", expanded: true)]
        [ShowIf("@SpawnStrategy == SpawnStrategyType.Neighbor")]
        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [TableList(ShowIndexLabels = true)]
        [Tooltip("Define valid neighbor types and their facing directions")]
        public List<NeighborTypeSettings> NeighborTypes = new List<NeighborTypeSettings>();

        [VerticalGroup("$GroupName/Spawn Settings/Split/Left")]
        [ToggleLeft]
        [LabelText("Enabled")]
        [Tooltip("When disabled, this mine type will not be spawned")]
        public bool IsEnabled = true;

        [System.Serializable]
        public class NeighborTypeSettings
        {
            [Required]
            [HorizontalGroup("Row")]
            [VerticalGroup("Row/Left")]
            [LabelWidth(100)]
            public MineData NeighborMineData;

            [HorizontalGroup("Row")]
            [VerticalGroup("Row/Right")]
            [LabelWidth(100)]
            public FacingDirection FacingDirection = FacingDirection.Right;

            [HorizontalGroup("Row")]
            [VerticalGroup("Row/Right")]
            [LabelWidth(100)]
            [MinValue(1)]
            [Tooltip("Number of this neighbor type to spawn around each primary mine")]
            public int SpawnCount = 1;
        }

        private string GroupName => string.IsNullOrEmpty(Description) 
            ? GetMineTypeName() 
            : $"{Description} [{GetMineTypeName()}]";

        private string GetMineTypeName()
        {
            if (MineData == null) return "No Mine Selected";
            if (MineData is MonsterMineData monsterData)
            {
                return $"{monsterData.MonsterType} ({MineData.Type})";
            }
            return MineData.Type.ToString();
        }

        private static IEnumerable<ValueDropdownItem<SpawnStrategyType>> GetSpawnStrategies()
        {
            yield return new ValueDropdownItem<SpawnStrategyType>("Random", SpawnStrategyType.Random);
            yield return new ValueDropdownItem<SpawnStrategyType>("Center", SpawnStrategyType.Center);
            yield return new ValueDropdownItem<SpawnStrategyType>("Edge", SpawnStrategyType.Edge);
            yield return new ValueDropdownItem<SpawnStrategyType>("Corner", SpawnStrategyType.Corner);
            yield return new ValueDropdownItem<SpawnStrategyType>("Surrounded", SpawnStrategyType.Surrounded);
            yield return new ValueDropdownItem<SpawnStrategyType>("Symmetric", SpawnStrategyType.Symmetric);
            yield return new ValueDropdownItem<SpawnStrategyType>("Adjacent", SpawnStrategyType.Adjacent);
            yield return new ValueDropdownItem<SpawnStrategyType>("Neighbor", SpawnStrategyType.Neighbor);
        }

        private void OnSpawnStrategyChanged()
        {
            if (SpawnStrategy != SpawnStrategyType.Surrounded)
            {
                TargetMineType = MineType.Monster;
                TargetMonsterType = MonsterType.None;
            }

            if (SpawnStrategy != SpawnStrategyType.Symmetric)
            {
                SymmetryDirection = SymmetryDirection.AroundVerticalLine;
                SymmetryLinePosition = 0.5f;
                MinDistanceToLine = 0;
                MaxDistanceToLine = 0;
            }

            if (SpawnStrategy != SpawnStrategyType.Adjacent)
            {
                MaxDistance = 1;
                AllowDiagonal = true;
                MinClusterSize = 2;
                MaxClusterSize = 4;
                AdjacencyDirection = AdjacentDirection.Vertical;
            }

            if (SpawnStrategy != SpawnStrategyType.Neighbor)
            {
                NeighborTypes.Clear();
            }
        }

        private void OnTargetMineTypeChanged()
        {
            if (TargetMineType != MineType.Monster)
            {
                TargetMonsterType = MonsterType.None;
            }
        }

        private void OnSymmetryDirectionChanged()
        {
            // Reset line position when changing direction
            SymmetryLinePosition = 0.5f;
        }

        public Sprite GetDirectionalSprite(FacingDirection direction)
        {
            // Use the directional sprites from MineData
            return MineData?.GetDirectionalSprite();
        }

        private string GetSymmetryLineInfo()
        {
            if (SpawnStrategy != SpawnStrategyType.Symmetric) return string.Empty;

            var gridSize = GetCurrentGridSize();
            if (gridSize == null) return "Grid size not available";

            int position = SymmetryDirection == SymmetryDirection.AroundHorizontalLine
                ? Mathf.RoundToInt(gridSize.Value.y * SymmetryLinePosition)
                : Mathf.RoundToInt(gridSize.Value.x * SymmetryLinePosition);

            string direction = SymmetryDirection == SymmetryDirection.AroundHorizontalLine ? "row" : "column";
            return $"Line will be placed at {direction} {position}";
        }

        private Vector2Int? GetCurrentGridSize()
        {
            // Try to find the GridManager in the scene
            var gridManager = UnityEngine.Object.FindObjectOfType<GridManager>();
            if (gridManager == null) return null;

            return new Vector2Int(gridManager.Width, gridManager.Height);
        }

        [OnInspectorGUI]
        private void OnSymmetryLinePositionChanged()
        {
            // The InfoBox will automatically update when any value changes
        }
    }
} 