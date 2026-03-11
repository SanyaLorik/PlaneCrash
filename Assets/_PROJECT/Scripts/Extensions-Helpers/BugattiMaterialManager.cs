using System.Collections.Generic;
using UnityEngine;

public class BugattiMaterialManager : MonoBehaviour {
    [SerializeField] private List<Color> _colors;
    [SerializeField] private string[] _matNames;
    [SerializeField] private Renderer _renderer;


    private void OnEnable() {
        Material[] materials = _renderer.materials;
        List<int> newColorsIndexes = EnumerableHelper.GetNewRandomNumberSet(_colors.Count);
        int colorIndex = 0;
        foreach (var matName in _matNames) {
            for (int i = 0; i < materials.Length; i++) {
                if (materials[i].name.Contains(matName)) {
                    Color newColor = _colors[newColorsIndexes[colorIndex]];
                    materials[i].color = newColor;
                    colorIndex++;
                }
            }
        }
        _renderer.materials = materials;
    }
    
}
