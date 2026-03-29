using System.Collections.Generic;
using UnityEngine;

public class CanvasHider : MonoBehaviour {
    [SerializeField] private List<GameObject> _canvasesToHide;
    [SerializeField] private bool _hide;

    
    private void OnValidate() {
        _canvasesToHide.ForEach(c => c.SetActive(!_hide));
    }
}
