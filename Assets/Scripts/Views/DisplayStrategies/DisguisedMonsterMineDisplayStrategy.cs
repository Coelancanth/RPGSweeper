using UnityEngine;
using TMPro;
using RPGMinesweeper;

namespace RPGMinesweeper
{
    public class DisguisedMonsterMineDisplayStrategy : IMineDisplayStrategy
    {
        private TextMeshPro m_ValueText;
        private TextMeshPro m_DisguisedValueText;
        private TextMeshPro m_StatsText;
        private TextMeshPro m_MineValueText;
        private DisguisedMonsterMine m_DisguisedMonsterMine;
        private MineDisplayConfig m_Config;
        private SpriteRenderer m_MineRenderer;
        private Sprite m_OriginalSprite;
        private bool m_IsRevealed;

        public void SetupDisplay(GameObject cellObject, TextMeshPro valueText)
        {
            m_ValueText = valueText;
            m_Config = cellObject.GetComponent<CellView>().DisplayConfig;
            m_MineRenderer = cellObject.GetComponent<CellView>().MineRenderer;
            
            // Create disguised value text component
            var disguisedValueGO = new GameObject("DisguisedValue");
            disguisedValueGO.transform.SetParent(cellObject.transform, false);
            m_DisguisedValueText = disguisedValueGO.AddComponent<TextMeshPro>();
            m_DisguisedValueText.fontSize = valueText.fontSize;
            m_DisguisedValueText.alignment = TextAlignmentOptions.Center;
            m_DisguisedValueText.sortingOrder = valueText.sortingOrder;
            m_DisguisedValueText.transform.localPosition = m_Config.StandardMineValuePosition;
            m_DisguisedValueText.enabled = false;

            // Create stats text (HP) component
            var statsGO = new GameObject("MonsterStats");
            statsGO.transform.SetParent(cellObject.transform, false);
            m_StatsText = statsGO.AddComponent<TextMeshPro>();
            m_StatsText.fontSize = valueText.fontSize;
            m_StatsText.alignment = TextAlignmentOptions.Center;
            m_StatsText.sortingOrder = valueText.sortingOrder;
            m_StatsText.transform.localPosition = m_Config.HPPosition;
            m_StatsText.enabled = false;

            // Create mine value text component
            var mineValueGO = new GameObject("MineValue");
            mineValueGO.transform.SetParent(cellObject.transform, false);
            m_MineValueText = mineValueGO.AddComponent<TextMeshPro>();
            m_MineValueText.fontSize = valueText.fontSize;
            m_MineValueText.alignment = TextAlignmentOptions.Center;
            m_MineValueText.sortingOrder = valueText.sortingOrder;
            m_MineValueText.transform.localPosition = m_Config.MonsterMineValuePosition;
            m_MineValueText.enabled = false;

            // Position damage text
            m_ValueText.transform.localPosition = m_Config.DamagePosition;
        }

        public void UpdateDisplay(IMine mine, MineData mineData, bool isRevealed)
        {
            // Unsubscribe from previous monster's events if any
            if (m_DisguisedMonsterMine != null)
            {
                m_DisguisedMonsterMine.OnHpChanged -= HandleHpChanged;
                m_DisguisedMonsterMine.OnEnraged -= HandleEnraged;
                m_DisguisedMonsterMine.OnDisguiseChanged -= HandleDisguiseChanged;
            }

            m_IsRevealed = isRevealed;
            if (!isRevealed)
            {
                DisableAllTexts();
                return;
            }

            m_DisguisedMonsterMine = mine as DisguisedMonsterMine;
            if (m_DisguisedMonsterMine == null) return;

            // Store original sprite for when disguise is removed
            if (m_OriginalSprite == null && m_MineRenderer != null && m_MineRenderer.sprite != null)
            {
                m_OriginalSprite = m_MineRenderer.sprite;
            }

            // Subscribe to monster's events
            m_DisguisedMonsterMine.OnHpChanged += HandleHpChanged;
            m_DisguisedMonsterMine.OnEnraged += HandleEnraged;
            m_DisguisedMonsterMine.OnDisguiseChanged += HandleDisguiseChanged;

            // Update text positions
            UpdateTextPositions();
            
            // Check disguise state and update display accordingly
            if (m_DisguisedMonsterMine.IsDisguised)
            {
                UpdateDisguisedDisplay();
            }
            else
            {
                UpdateMonsterDisplay(mineData);
            }
        }

        private void DisableAllTexts()
        {
            if (m_ValueText != null) m_ValueText.enabled = false;
            if (m_DisguisedValueText != null) m_DisguisedValueText.enabled = false;
            if (m_StatsText != null) m_StatsText.enabled = false;
            if (m_MineValueText != null) m_MineValueText.enabled = false;
        }

        private void UpdateTextPositions()
        {
            if (m_StatsText != null)
            {
                m_StatsText.transform.localPosition = m_Config.HPPosition;
            }
            if (m_ValueText != null)
            {
                m_ValueText.transform.localPosition = m_Config.DamagePosition;
            }
            if (m_MineValueText != null)
            {
                m_MineValueText.transform.localPosition = m_Config.MonsterMineValuePosition;
            }
            if (m_DisguisedValueText != null)
            {
                m_DisguisedValueText.transform.localPosition = m_Config.StandardMineValuePosition;
            }
        }

        private void HandleDisguiseChanged(Vector2Int position, bool isDisguised)
        {
            if (isDisguised)
            {
                UpdateDisguisedDisplay();
            }
            else
            {
                // Switch to monster sprite when disguise is removed
                if (m_MineRenderer != null && m_OriginalSprite != null)
                {
                    m_MineRenderer.sprite = m_OriginalSprite;
                }
                
                // Update monster display
                m_DisguisedValueText.enabled = false;
                UpdateDamageDisplay();
                UpdateHPDisplay();
                UpdateMineValueDisplay();
            }
        }

        private void HandleHpChanged(Vector2Int position, float hpPercentage)
        {
            if (m_DisguisedMonsterMine.IsDisguised) return;
            UpdateHPDisplay();
        }

        private void HandleEnraged(Vector2Int position)
        {
            if (m_DisguisedMonsterMine.IsDisguised) return;
            UpdateDamageDisplay();
        }

        private void UpdateDisguisedDisplay()
        {
            // Set disguise sprite
            if (m_MineRenderer != null && m_DisguisedMonsterMine.DisguiseSprite != null)
            {
                m_MineRenderer.sprite = m_DisguisedMonsterMine.DisguiseSprite;
                // Ensure the renderer is enabled
                m_MineRenderer.enabled = true;
                
                // Log for debugging
                Debug.Log($"Setting disguise sprite: {m_DisguisedMonsterMine.DisguiseSprite.name} for mine at position {m_DisguisedMonsterMine?.GetType()?.Name}");
            }
            else
            {
                // Log warning if sprite is missing
                if (m_DisguisedMonsterMine.DisguiseSprite == null)
                {
                    Debug.LogWarning($"DisguisedMonsterMine has null DisguiseSprite! Assign a sprite in the DisguisedMonsterMineData asset.");
                }
                
                // Fallback to using a default sprite if available
                if (m_MineRenderer != null)
                {
                    if (m_Config != null && m_Config.DefaultDisguiseSprite != null)
                    {
                        m_MineRenderer.sprite = m_Config.DefaultDisguiseSprite;
                        m_MineRenderer.enabled = true;
                        Debug.Log("Using fallback disguise sprite");
                    }
                    else
                    {
                        Debug.LogWarning("No fallback disguise sprite available in config");
                    }
                }
            }

            // Hide monster-related displays
            if (m_ValueText != null) m_ValueText.enabled = false;
            if (m_StatsText != null) m_StatsText.enabled = false;
            if (m_MineValueText != null) m_MineValueText.enabled = false;

            // Show disguised value
            if (m_DisguisedValueText != null)
            {
                m_DisguisedValueText.enabled = true;
                m_DisguisedValueText.text = m_DisguisedMonsterMine.DisguisedValue.ToString();
                m_DisguisedValueText.color = m_DisguisedMonsterMine.DisguisedValueColor;
            }
        }

        private void UpdateMonsterDisplay(MineData mineData)
        {
            // Hide disguised display
            if (m_DisguisedValueText != null)
            {
                m_DisguisedValueText.enabled = false;
            }

            // Update monster-related displays
            UpdateDamageDisplay();
            UpdateHPDisplay();
            UpdateMineValueDisplay();
        }

        private void UpdateDamageDisplay()
        {
            if (m_ValueText == null || m_DisguisedMonsterMine == null) return;
            
            int damageValue = m_DisguisedMonsterMine.CalculateDamage();
            m_ValueText.enabled = true;
            m_ValueText.text = damageValue.ToString();
            
            // Use enraged color if monster is enraged
            m_ValueText.color = m_DisguisedMonsterMine.IsEnraged ? 
                m_Config.EnragedColor : 
                m_Config.MonsterPowerColor;
        }

        private void UpdateHPDisplay()
        {
            if (m_StatsText == null || m_DisguisedMonsterMine == null) return;
            
            float hpPercentage = m_DisguisedMonsterMine.HpPercentage;
            m_StatsText.enabled = true;
            m_StatsText.text = Mathf.RoundToInt(hpPercentage * 100) + "%";
            
            // Interpolate between high HP color and low HP color based on percentage
            m_StatsText.color = Color.Lerp(m_Config.HPLowColor, m_Config.HPHighColor, hpPercentage);
        }

        private void UpdateMineValueDisplay()
        {
            if (m_MineValueText == null || m_DisguisedMonsterMine == null) return;
            
            // We need to use a different approach since DisguisedMonsterMine doesn't have Value property
            // Get the disguised value from our monster
            int value = m_DisguisedMonsterMine.DisguisedValue;
            Color color = m_DisguisedMonsterMine.DisguisedValueColor;
            
            // Only show mine value if it's greater than 0
            if (value > 0)
            {
                m_MineValueText.enabled = true;
                m_MineValueText.text = value.ToString();
                m_MineValueText.color = color;
            }
            else
            {
                m_MineValueText.enabled = false;
            }
        }

        public void CleanupDisplay()
        {
            // Unsubscribe from events
            if (m_DisguisedMonsterMine != null)
            {
                m_DisguisedMonsterMine.OnHpChanged -= HandleHpChanged;
                m_DisguisedMonsterMine.OnEnraged -= HandleEnraged;
                m_DisguisedMonsterMine.OnDisguiseChanged -= HandleDisguiseChanged;
                m_DisguisedMonsterMine = null;
            }

            if (m_ValueText != null)
            {
                m_ValueText.enabled = false;
            }
            
            if (m_DisguisedValueText != null)
            {
                Object.Destroy(m_DisguisedValueText.gameObject);
            }
            
            if (m_StatsText != null)
            {
                Object.Destroy(m_StatsText.gameObject);
            }
            
            if (m_MineValueText != null)
            {
                Object.Destroy(m_MineValueText.gameObject);
            }
        }
    }
} 