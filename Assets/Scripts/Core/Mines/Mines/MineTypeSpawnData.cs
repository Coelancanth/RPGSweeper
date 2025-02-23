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

        [VerticalGroup("$GroupName/Header/Right")]
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

        [VerticalGroup("$GroupName/Spawn Settings/Split/Right")]
        [ToggleLeft]
        [LabelText("Enabled")]
        [Tooltip("When disabled, this mine type will not be spawned")]
        public bool IsEnabled = true;

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

        private string GroupName => string.IsNullOrEmpty(Description) 
            ? GetMineTypeName() 
            : $"{GetMineTypeName()} ({Description})";

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
        }

        private void OnSpawnStrategyChanged()
        {
            if (SpawnStrategy != SpawnStrategyType.Surrounded)
            {
                TargetMineType = MineType.Monster;
                TargetMonsterType = MonsterType.None;
            }
        }

        private void OnTargetMineTypeChanged()
        {
            if (TargetMineType != MineType.Monster)
            {
                TargetMonsterType = MonsterType.None;
            }
        }
    }
} 