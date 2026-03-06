using UnityEngine;

public class CanvasWindowNotifier : MonoBehaviour {
    private void OnEnable() {
        SystemEvents.InvokeCanvasWindow(true);
    }
    
    private void OnDisable() {
        SystemEvents.InvokeCanvasWindow(false);
    }
}