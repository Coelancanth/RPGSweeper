## SpecStory

## Implementing Monster Transformation Effect (2/19/2025, 1:17:54 PM)

_**User**_

first make sure that you follow @Principle.md, now I would like to implement a new "transform" effect, which when applied, it will transform certain monster type into the other , you can check @TargetedRevealEffect.cs as the reference implementation

---

_**Assistant**_

```diff
- 
+ using UnityEngine;
+ using RPGMinesweeper.Grid;
+ using RPGMinesweeper.Effects;
+ using RPGMinesweeper; // For MonsterType
+ 
+ namespace RPGMinesweeper.Effects
+ {
+     public class MonsterTransformEffect : ITriggerableEffect
+     {
+         #region Private Fields
+         private readonly float m_Radius;
+         private readonly GridShape m_Shape;
+         private readonly MonsterType m_SourceMonsterType;
+         private readonly MonsterType m_TargetMonsterType;
+         #endregion
+ 
+         #region Public Properties
+         public EffectType Type => EffectType.Triggerable;
+         public string Name => "Monster Transform";
+         #endregion
+ 
+         #region Constructors
+         public MonsterTransformEffect(float radius, MonsterType sourceMonsterType, MonsterType targetMonsterType, GridShape shape = GridShape.Square)
+         {
+             m_Radius = radius;
+             m_SourceMonsterType = sourceMonsterType;
+             m_TargetMonsterType = targetMonsterType;
+             m_Shape = shape;
+         }
+         #endregion
+ 
+         #region Public Methods
+         public void Apply(GameObject target, Vector2Int sourcePosition)
+         {
+             var gridManager = GameObject.FindFirstObjectByType<GridManager>();
+             var mineManager = GameObject.FindFirstObjectByType<MineManager>();
+ 
+             if (gridManager == null || mineManager == null)
+             {
+                 return;
+             }
+ 
+             var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));
+ 
+             foreach (var pos in affectedPositions)
+             {
+                 if (gridManager.IsValidPosition(pos))
+                 {
+                     var cellObject = gridManager.GetCellObject(pos);
+                     if (cellObject != null)
+                     {
+                         var cellView = cellObject.GetComponent<CellView>();
+                         if (cellView != null && mineManager.HasMineAt(pos))
+                         {
+                             var mines = mineManager.GetMines();
+                             if (mines.TryGetValue(pos, out var mine) && mine is MonsterMine monsterMine)
+                             {
+                                 if (monsterMine.MonsterType == m_SourceMonsterType)
+                                 {
+                                     monsterMine.MonsterType = m_TargetMonsterType;
+ 
+                                     var newMineData = mineManager.GetMineDataAt(pos);
+                                     if (newMineData != null)
+                                     {
+                                         cellView.ShowMineSprite(newMineData.MineSprite, mine, newMineData);
+                                         cellView.UpdateVisuals(true);
+                                     }
+                                 }
+                             }
+                         }
+                     }
+                 }
+             }
+         }
+ 
+         public void Remove(GameObject source, Vector2Int sourcePosition)
+         {
+             // Transformation is permanent; no removal logic required.
+         }
+         #endregion
+     }
+ } 
```

---

_****_

Pseudocode:
1. Create a new class named MonsterTransformEffect in the namespace RPGMinesweeper.Effects.
2. The class implements ITriggerableEffect and declares:
   • Private fields: m_Radius, m_Shape, m_SourceMonsterType, and m_TargetMonsterType.
   • Public properties: Type (returning EffectType.Triggerable) and Name ("Monster Transform").
3. In the constructor, initialize the fields with parameters (radius, sourceMonsterType, targetMonsterType, and optional shape).
4. For the Apply method:
   • Get a reference to GridManager and MineManager.
   • Retrieve affected positions using GridShapeHelper.GetAffectedPositions based on the source position, shape, and radius.
   • For each affected position, if valid and a cell exists:
     - Retrieve the CellView and, if a mine exists, check whether it is a MonsterMine.
     - If the MonsterMine’s type equals m_SourceMonsterType, update it to m_TargetMonsterType.
     - Retrieve the new mine data and update the display (using ShowMineSprite and UpdateVisuals).
5. The Remove method is empty as the transformation is permanent.

I have written and saved the complete implementation in Assets/Scripts/Core/Effects/MonsterTransformEffect.cs below:

using UnityEngine;
using RPGMinesweeper.Grid;
using RPGMinesweeper.Effects;
using RPGMinesweeper; // For MonsterType

namespace RPGMinesweeper.Effects
{
    public class MonsterTransformEffect : ITriggerableEffect
    {
        #region Private Fields
        private readonly float m_Radius;
        private readonly GridShape m_Shape;
        private readonly MonsterType m_SourceMonsterType;
        private readonly MonsterType m_TargetMonsterType;
        #endregion

        #region Public Properties
        public EffectType Type => EffectType.Triggerable;
        public string Name => "Monster Transform";
        #endregion

        #region Constructors
        public MonsterTransformEffect(float radius, MonsterType sourceMonsterType, MonsterType targetMonsterType, GridShape shape = GridShape.Square)
        {
            m_Radius = radius;
            m_SourceMonsterType = sourceMonsterType;
            m_TargetMonsterType = targetMonsterType;
            m_Shape = shape;
        }
        #endregion

        #region Public Methods
        public void Apply(GameObject target, Vector2Int sourcePosition)
        {
            var gridManager = GameObject.FindFirstObjectByType<GridManager>();
            var mineManager = GameObject.FindFirstObjectByType<MineManager>();

            if (gridManager == null || mineManager == null)
            {
                return;
            }

            var affectedPositions = GridShapeHelper.GetAffectedPositions(sourcePosition, m_Shape, Mathf.RoundToInt(m_Radius));

            foreach (var pos in affectedPositions)
            {
                if (gridManager.IsValidPosition(pos))
                {
                    var cellObject = gridManager.GetCellObject(pos);
                    if (cellObject != null)
                    {
                        var cellView = cellObject.GetComponent<CellView>();
                        if (cellView != null && mineManager.HasMineAt(pos))
                        {
                            var mines = mineManager.GetMines();
                            if (mines.TryGetValue(pos, out var mine) && mine is MonsterMine monsterMine)
                            {
                                if (monsterMine.MonsterType == m_SourceMonsterType)
                                {
                                    monsterMine.MonsterType = m_TargetMonsterType;

                                    var newMineData = mineManager.GetMineDataAt(pos);
                                    if (newMineData != null)
                                    {
                                        cellView.ShowMineSprite(newMineData.MineSprite, mine, newMineData);
                                        cellView.UpdateVisuals(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Remove(GameObject source, Vector2Int sourcePosition)
        {
            // Transformation is permanent; no removal logic required.
        }
        #endregion
    }
}

This implementation follows the conventions specified in Principle.md and serves as a complete, functional "transform" effect analogous to TargetedRevealEffect.cs.

---



---

