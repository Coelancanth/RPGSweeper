#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using System.Collections.Generic;
using RPGMinesweeper.Grid;  // For GridShape
using RPGMinesweeper;       // For MineSpawnStrategyType and MineType
using RPGMinesweeper.Core.Mines.Spawning;
public class MineEditorWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/RPG Minesweeper/Mine Editor")]
    private static void Open()
    {
        var window = GetWindow<MineEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        window.titleContent = new GUIContent("Mine Editor");
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;

        // Searches "Assets/ScriptableObjects/Mines" folder for all MineData assets
        tree.AddAllAssetsAtPath("Standard Mines", "Assets/ScriptableObjects/Mines", typeof(MineData), true)
            .ForEach(AddDragHandles);

        // Searches "Assets/ScriptableObjects/Monsters" folder for all MonsterMineData assets
        tree.AddAllAssetsAtPath("Monster Mines", "Assets/ScriptableObjects/Monsters", typeof(MonsterMineData), true)
            .ForEach(AddDragHandles);

        // Add icons to mines if they have sprites (shown in the tree view)
        tree.EnumerateTree().AddIcons<MineData>(x => x.MineSprite);
        tree.EnumerateTree().AddIcons<MonsterMineData>(x => x.MineSprite);

        return tree;
    }

    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Standard Mine")))
            {
                CreateNewAsset<MineData>("Assets/ScriptableObjects/Mines");
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Monster Mine")))
            {
                CreateNewAsset<MonsterMineData>("Assets/ScriptableObjects/Monsters");
            }

            // Add delete button, enabled only when an item is selected
            GUI.enabled = selected != null && selected.Value != null;
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Delete Selected")))
            {
                DeleteSelectedAsset();
            }
            GUI.enabled = true;
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    private void CreateNewAsset<T>(string folder) where T : ScriptableObject
    {
        // Ensure the folder exists
        if (!AssetDatabase.IsValidFolder(folder))
        {
            string[] folderNames = folder.Split('/');
            string currentPath = folderNames[0];
            for (int i = 1; i < folderNames.Length; i++)
            {
                string parentPath = currentPath;
                currentPath = currentPath + "/" + folderNames[i];
                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    AssetDatabase.CreateFolder(parentPath, folderNames[i]);
                }
            }
        }

        // Create the asset
        var asset = ScriptableObject.CreateInstance<T>();
        
        // Initialize default values based on type
        if (asset is MineData mineData)
        {
            InitializeStandardMine(mineData);
        }
        else if (asset is MonsterMineData monsterMineData)
        {
            InitializeMonsterMine(monsterMineData);
        }

        // Show save file dialog
        string path = EditorUtility.SaveFilePanel(
            "Create New " + typeof(T).Name,
            folder,
            "New " + typeof(T).Name,
            "asset"
        );

        // If user didn't cancel
        if (!string.IsNullOrEmpty(path))
        {
            // Convert to project-relative path
            path = FileUtil.GetProjectRelativePath(path);
            
            // Create the asset file
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            
            // Select the created asset
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            
            // Try to select it in our editor window
            base.TrySelectMenuItemWithObject(asset);
        }
        else
        {
            // If cancelled, destroy the temporary asset
            DestroyImmediate(asset);
        }
    }

    private void InitializeStandardMine(MineData mine)
    {
        mine.name = "New Standard Mine";
        mine.Type = MineType.Standard;
        mine.Value = 1;
        mine.Shape = GridShape.Square;
        mine.Radius = 1;
        //mine.SpawnStrategy = SpawnStrategyType.Random;
        
        // Set default colors
        var serializedObject = new SerializedObject(mine);
        var valueColorProp = serializedObject.FindProperty("m_ValueColor");
        var mineValueColorProp = serializedObject.FindProperty("m_MineValueColor");
        
        if (valueColorProp != null) valueColorProp.colorValue = Color.white;
        if (mineValueColorProp != null) mineValueColorProp.colorValue = Color.yellow;
        
        serializedObject.ApplyModifiedProperties();
    }

    private void InitializeMonsterMine(MonsterMineData mine)
    {
        // First initialize base mine properties
        InitializeStandardMine(mine);
        
        mine.name = "New Monster Mine";
        mine.Type = MineType.Monster;  // Set correct type for monster mines
        
        // Initialize monster-specific properties
        var serializedObject = new SerializedObject(mine);
        var monsterTypeProp = serializedObject.FindProperty("m_MonsterType");
        var maxHpProp = serializedObject.FindProperty("m_MaxHp");
        var baseDamageProp = serializedObject.FindProperty("m_BaseDamage");
        var damagePerHitProp = serializedObject.FindProperty("m_DamagePerHit");
        var hasEnrageStateProp = serializedObject.FindProperty("m_HasEnrageState");
        var enrageMultiplierProp = serializedObject.FindProperty("m_EnrageDamageMultiplier");
        
        if (monsterTypeProp != null) monsterTypeProp.enumValueIndex = 0; // None
        if (maxHpProp != null) maxHpProp.intValue = 100;
        if (baseDamageProp != null) baseDamageProp.intValue = 20;
        if (damagePerHitProp != null) damagePerHitProp.intValue = 25;
        if (hasEnrageStateProp != null) hasEnrageStateProp.boolValue = false;
        if (enrageMultiplierProp != null) enrageMultiplierProp.floatValue = 1.5f;
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DeleteSelectedAsset()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        if (selected?.Value == null) return;

        // Cast to UnityEngine.Object
        var selectedObject = selected.Value as UnityEngine.Object;
        if (selectedObject == null) return;

        string assetPath = AssetDatabase.GetAssetPath(selectedObject);
        if (string.IsNullOrEmpty(assetPath)) return;

        // Show confirmation dialog
        if (EditorUtility.DisplayDialog("Delete Asset",
            $"Are you sure you want to delete '{selected.Name}'?", "Delete", "Cancel"))
        {
            // Delete the asset
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            
            // Force rebuild the menu tree to reflect the deletion
            ForceMenuTreeRebuild();
        }
    }

    protected override void OnImGUI()
    {
        // Handle delete key press
        if (Event.current.type == EventType.KeyDown && 
            Event.current.keyCode == KeyCode.Delete && 
            this.MenuTree.Selection.Count > 0)
        {
            DeleteSelectedAsset();
            Event.current.Use();
        }

        base.OnImGUI();
    }
}
#endif