using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using RTLTMPro;

public class FontReplacer : EditorWindow
{
    private TMP_FontAsset baseFont; // Font to be replaced
    private TMP_FontAsset newFont;  // New font to assign
    public List<SceneAsset> scenes = new List<SceneAsset>(); // List of scene assets
    public List<GameObject> prefabs = new List<GameObject>(); // List of prefabs
    private int replacementsCount = 0; // Counter for replacements

    private SerializedObject serializedObject;
    private SerializedProperty scenesProperty;
    private SerializedProperty prefabsProperty;

    private Vector2 scrollPosition = Vector2.zero; // Scroll position for the GUI window

    // Enum to select between RTLTextMeshPro and regular TextMeshPro
    public enum TextComponentType { RTLTextMeshPro, TextMeshPro }
    private TextComponentType textComponentType = TextComponentType.RTLTextMeshPro;

    [MenuItem("Tools/Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacer>("Font Replacer");
    }

    private void OnEnable()
    {
        // Initialize SerializedObject and SerializedProperty
        serializedObject = new SerializedObject(this);
        scenesProperty = serializedObject.FindProperty("scenes");
        prefabsProperty = serializedObject.FindProperty("prefabs");
    }

    private void OnGUI()
    {
        // Start the scroll view
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));

        GUILayout.Label("Replace Fonts in Text Components", EditorStyles.boldLabel);

        baseFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Base Font (to replace)", baseFont, typeof(TMP_FontAsset), false);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font", newFont, typeof(TMP_FontAsset), false);

        // Enum selection for Text Component Type
        textComponentType = (TextComponentType)EditorGUILayout.EnumPopup("Text Component Type", textComponentType);

        GUILayout.Space(10);
        GUILayout.Label("Assign Scenes and Prefabs:", EditorStyles.boldLabel);

        // Update the serialized object before modifying it
        serializedObject.Update();
        EditorGUILayout.PropertyField(scenesProperty, new GUIContent("Scenes"), true);
        EditorGUILayout.PropertyField(prefabsProperty, new GUIContent("Prefabs"), true);
        serializedObject.ApplyModifiedProperties(); // Apply changes to the serialized object

        GUILayout.Space(10);

        if (GUILayout.Button("Replace Fonts"))
        {
            if (newFont == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a new font.", "OK");
                return;
            }
            replacementsCount = 0; // Reset count
            ReplaceFontsInAssignedAssets();
        }

        // End the scroll view
        EditorGUILayout.EndScrollView();
    }

    private void ReplaceFontsInAssignedAssets()
    {
        // Replace fonts in assigned scenes
        foreach (SceneAsset scene in scenes)
        {
            if (scene == null) continue;
            string scenePath = AssetDatabase.GetAssetPath(scene);

            // Open the scene additively without unloading the current scene
            Scene openScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            if (!openScene.isLoaded)
            {
                Debug.LogError($"Failed to load scene: {scenePath}");
                continue;
            }

            GameObject[] rootObjects = openScene.GetRootGameObjects();
            ReplaceFontsInGameObjects(rootObjects);

            // Save the scene changes
            EditorSceneManager.MarkSceneDirty(openScene);
            EditorSceneManager.SaveScene(openScene);

            // Optionally, close the scene to avoid clutter
            EditorSceneManager.CloseScene(openScene, true);
        }

        // Replace fonts in assigned prefabs
        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null) continue;
            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            ReplaceFontsInGameObjects(new[] { instance });

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Show final dialog with details
        string baseFontName = baseFont != null ? baseFont.name : "All Fonts";
        string newFontName = newFont.name;
        EditorUtility.DisplayDialog(
            "Complete",
            $"Font replacement is complete.\n\n" +
            $"Base Font: {baseFontName}\n" +
            $"New Font: {newFontName}\n" +
            $"Total Replacements: {replacementsCount}",
            "OK"
        );
    }

    private void ReplaceFontsInGameObjects(GameObject[] rootObjects)
    {
        foreach (GameObject obj in rootObjects)
        {
            // Check the selected text component type
            if (textComponentType == TextComponentType.RTLTextMeshPro)
            {
                var rtlTextComponents = obj.GetComponentsInChildren<RTLTextMeshPro>(true);
                foreach (var rtlText in rtlTextComponents)
                {
                    if (baseFont == null || rtlText.font == baseFont)
                    {
                        Undo.RecordObject(rtlText, "Replace RTLTextMeshPro Font");
                        rtlText.font = newFont;
                        EditorUtility.SetDirty(rtlText);
                        replacementsCount++;
                    }
                }
            }
            else if (textComponentType == TextComponentType.TextMeshPro)
            {
                var textMeshProComponents = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var tmpText in textMeshProComponents)
                {
                    if (baseFont == null || tmpText.font == baseFont)
                    {
                        Undo.RecordObject(tmpText, "Replace TextMeshPro Font");
                        tmpText.font = newFont;
                        EditorUtility.SetDirty(tmpText);
                        replacementsCount++;
                    }
                }
            }
        }
    }
}
