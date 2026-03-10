#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class LocalizationTranslatorWindow : EditorWindow
{
    private const string JsonFolder = "Assets/_PROJECT/Scripts/Configs/JSON_Localizations/";
    private const string AssetSaveFolder = "Assets/_PROJECT/Scripts/Configs/Localizations/";

    public LocalizationDataPC SourceSO;

    public string ExportJsonName = "RU_Localization.json";
    public string ImportJsonName = "EN_Localization.json";

    public string ImportedAssetName = "EN_LocalizationDataPC.asset";

    [MenuItem("Tools/Localization Translator")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationTranslatorWindow>("Localization Translator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Localization Translator", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        GUILayout.Label("JSON Folder:");
        EditorGUILayout.HelpBox(JsonFolder, MessageType.None);

        EditorGUILayout.Space();

        SourceSO = (LocalizationDataPC)EditorGUILayout.ObjectField("Source SO", SourceSO, typeof(LocalizationDataPC), false);

        ExportJsonName = EditorGUILayout.TextField("Export JSON Name", ExportJsonName);

        if (GUILayout.Button("Export SO → JSON"))
        {
            if (SourceSO != null)
                ExportSO(SourceSO, ExportJsonName);
            else
                Debug.LogError("SourceSO not assigned");
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        ImportJsonName = EditorGUILayout.TextField("Import JSON Name", ImportJsonName);
        ImportedAssetName = EditorGUILayout.TextField("New SO Name", ImportedAssetName);

        if (GUILayout.Button("Import JSON → SO"))
        {
            ImportSO(ImportJsonName, ImportedAssetName);
        }
    }

    private void ExportSO(LocalizationDataPC so, string jsonName)
    {
        string fullPath = JsonFolder + jsonName;

        string json = JsonUtility.ToJson(so, true);

        File.WriteAllText(fullPath, json);

        Debug.Log($"Exported {so.name} → {fullPath}");

        AssetDatabase.Refresh();
    }

    private void ImportSO(string jsonName, string assetName)
    {
        string fullPath = JsonFolder + jsonName;

        if (!File.Exists(fullPath))
        {
            Debug.LogError("JSON file not found: " + fullPath);
            return;
        }

        string json = File.ReadAllText(fullPath);

        var so = ScriptableObject.CreateInstance<LocalizationDataPC>();

        JsonUtility.FromJsonOverwrite(json, so);

        string assetPath = AssetSaveFolder + assetName;

        AssetDatabase.CreateAsset(so, assetPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Imported JSON → ScriptableObject: {assetPath}");
    }
}
#endif