using UnityEngine;
using System;

namespace RPGMinesweeper
{
    [CreateAssetMenu(fileName = "MarkPanelDisplayConfig", menuName = "RPGMinesweeper/Mark Panel Display Config")]
    public class MarkPanelDisplayConfig : ScriptableObject
    {
        [Header("Panel Settings")]
        [SerializeField] private Vector2 m_PanelSize = new Vector2(120f, 150f);
        [SerializeField] private Vector2 m_Offset = new Vector2(20f, 20f);
        [SerializeField] private float m_ShowDuration = 0.25f;
        [SerializeField] private Color m_PanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        [Header("Mark Sprites")]
        [SerializeField] private Sprite m_FlagMarkSprite;
        [SerializeField] private Sprite m_QuestionMarkSprite;
        [SerializeField] private Sprite m_NumbersMarkSprite;
        [SerializeField] private Sprite m_CustomInputMarkSprite;
        
        [Header("Mark Size Settings")]
        [SerializeField] private float m_MarkScale = 0.7f;
        [SerializeField] private Vector2 m_MarkOffset = Vector2.zero;
        
        [Header("Button Settings")]
        [SerializeField] private Vector2 m_ButtonSize = new Vector2(100f, 30f);
        [SerializeField] private float m_ButtonSpacing = 10f;
        [SerializeField] private Color m_ButtonColor = Color.white;
        [SerializeField] private Color m_SelectedButtonColor = new Color(0.85f, 0.85f, 1f, 1f);
        
        // Event for changes to the configuration
        public event Action OnConfigChanged;
        
        // Properties for accessing serialized fields
        public Vector2 PanelSize => m_PanelSize;
        public Vector2 Offset => m_Offset;
        public float ShowDuration => m_ShowDuration;
        public Color PanelBackgroundColor => m_PanelBackgroundColor;
        
        public Sprite FlagMarkSprite => m_FlagMarkSprite;
        public Sprite QuestionMarkSprite => m_QuestionMarkSprite;
        public Sprite NumbersMarkSprite => m_NumbersMarkSprite;
        public Sprite CustomInputMarkSprite => m_CustomInputMarkSprite;
        
        public float MarkScale => m_MarkScale;
        public Vector2 MarkOffset => m_MarkOffset;
        
        public Vector2 ButtonSize => m_ButtonSize;
        public float ButtonSpacing => m_ButtonSpacing;
        public Color ButtonNormalColor => m_ButtonColor;
        public Color ButtonHoverColor => m_SelectedButtonColor;
        
        // Get a mark sprite based on the mark type
        public Sprite GetMarkSprite(CellMarkType markType)
        {
            switch (markType)
            {
                case CellMarkType.Flag:
                    return m_FlagMarkSprite;
                case CellMarkType.Question:
                    return m_QuestionMarkSprite;
                case CellMarkType.Numbers:
                    return m_NumbersMarkSprite;
                case CellMarkType.CustomInput:
                    return m_CustomInputMarkSprite;
                default:
                    return null;
            }
        }
        
        // Method to notify listeners of configuration changes
        public void NotifyConfigChanged()
        {
            OnConfigChanged?.Invoke();
        }
        
        private void OnValidate()
        {
            // Notify listeners of changes made in the inspector
            NotifyConfigChanged();
        }
    }
} 