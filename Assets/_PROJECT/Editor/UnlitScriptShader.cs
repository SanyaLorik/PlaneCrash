using UnityEngine;
using UnityEditor;

public class ChangeToUnlit : EditorWindow
{
    [MenuItem("Tools/Change Selected to Unlit")]
    static void ChangeSelectedToUnlit()
    {
        foreach (GameObject parent in Selection.gameObjects)
        {
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                foreach (Material mat in rend.sharedMaterials)
                {
                    if (mat != null)
                    {
                        mat.shader = Shader.Find("Universal Render Pipeline/Unlit");  // Для URP; для Built-in: "Unlit/Color"
                    }
                }
            }
        }
        Debug.Log("Shaders changed to Unlit on selected hierarchies.");
    }
}
