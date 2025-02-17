using UnityEngine;
using RPGMinesweeper.Effects;
/*!SECTION 
- The original monster transforms into one of the split parts
- Spawns (SplitCount - 1) additional monsters
- All split monsters have HP = original HP healthModifier
- Uses circular area for spawning new monsters
- Follows proper error checking and null safety
- Adheres to the codebase's event system for mine spawning
*/

[CreateAssetMenu(fileName = "SplitEffectData", menuName = "RPGMinesweeper/Effects/Split Effect")]
public class SplitEffectData : EffectData
{
    [SerializeField, Range(1, 10)] 
    private int m_SplitRadius = 2;
    
    [SerializeField, Range(0.1f, 1f)] 
    private float m_HealthModifier = 0.5f;
    
    [SerializeField, Range(2, 4)] 
    private int m_SplitCount = 2;

    public float SplitRadius => m_SplitRadius;
    public float HealthModifier => m_HealthModifier;
    public int SplitCount => m_SplitCount;

    public override IEffect CreateEffect()
    {
        return new SplitEffect(m_SplitRadius, m_HealthModifier, m_SplitCount);
    }
} 