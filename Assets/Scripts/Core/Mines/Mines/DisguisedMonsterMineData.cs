using UnityEngine;
using RPGMinesweeper;
using Sirenix.OdinInspector;
using UnityEditor;

namespace RPGMinesweeper
{
    [CreateAssetMenu(fileName = "DisguisedMonsterMineData", menuName = "RPGMinesweeper/DisguisedMonsterMineData")]
    public class DisguisedMonsterMineData : MonsterMineData
    {
        [TitleGroup("Disguise Properties")]
        [HorizontalGroup("Disguise Properties/Split")]
        [VerticalGroup("Disguise Properties/Split/Left"), LabelWidth(100)]
        [Tooltip("Sprite used when the mine is disguised")]
        [SerializeField] private Sprite m_DisguiseSprite;
        
        [VerticalGroup("Disguise Properties/Split/Left")]
        [Tooltip("The apparent value of the disguised mine (shown to player)")]
        [MinValue(1)]
        [SerializeField] private int m_DisguisedValue = 5;
        
        [VerticalGroup("Disguise Properties/Split/Left")]
        [Tooltip("Color used for displaying the disguised value")]
        [SerializeField] private Color m_DisguisedValueColor = Color.yellow;
        
        [VerticalGroup("Disguise Properties/Split/Left")]
        [Tooltip("Whether the mine is currently disguised")]
        [SerializeField] private bool m_IsDisguised = true;

        public Sprite DisguiseSprite => m_DisguiseSprite;
        public int DisguisedValue => m_DisguisedValue;
        public Color DisguisedValueColor => m_DisguisedValueColor;
        public bool IsDisguised => m_IsDisguised;
        
        public void SetDisguised(bool disguised)
        {
            m_IsDisguised = disguised;
        }
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            // Make sure this is always a DisguisedMonster type
            if (base.Type != MineType.DisguisedMonster)
            {
                // Use SerializedObject to modify the Type property
                var serializedObject = new UnityEditor.SerializedObject(this);
                var typeProp = serializedObject.FindProperty("Type");
                typeProp.enumValueIndex = (int)MineType.DisguisedMonster;
                serializedObject.ApplyModifiedProperties();
            }
#endif
        }
    }
} 