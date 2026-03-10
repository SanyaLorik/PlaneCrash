#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LocalizationTranslatorWindow : EditorWindow
{
    public LocalizationDataPC SourceSO;   // исходный SO на русском
    public string ExportPath = "Assets/Localization/Exported.json"; // куда сохранять JSON
    public string ImportedAssetName = "LocalizationDataPC_EN.asset"; // название нового SO после перевода

    [MenuItem("Tools/Localization Translator")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationTranslatorWindow>("Localization Translator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Localization Translator", EditorStyles.boldLabel);

        SourceSO = (LocalizationDataPC)EditorGUILayout.ObjectField("Source SO", SourceSO, typeof(LocalizationDataPC), false);
        ExportPath = EditorGUILayout.TextField("Export Path", ExportPath);
        ImportedAssetName = EditorGUILayout.TextField("Imported Asset Name", ImportedAssetName);

        if (GUILayout.Button("Export to JSON"))
        {
            if (SourceSO != null)
                ExportSO(SourceSO, ExportPath);
        }

        if (GUILayout.Button("Import JSON"))
        {
            ImportSO(ExportPath, ImportedAssetName);
        }
    }

    private void ExportSO(LocalizationDataPC so, string path)
    {
        string json = JsonUtility.ToJson(so, true);
        System.IO.File.WriteAllText(path, json);
        Debug.Log($"Exported {so.name} to {path}");
    }

    private void ImportSO(string path, string assetName)
    {
        string json = System.IO.File.ReadAllText(path);
        var so = ScriptableObject.CreateInstance<LocalizationDataPC>();
        JsonUtility.FromJsonOverwrite(json, so);
        so.name = assetName;
        AssetDatabase.CreateAsset(so, $"Assets/Localization/{assetName}");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Imported {assetName} from JSON");
    }
}
#endif