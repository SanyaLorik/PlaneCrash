using UnityEngine;
using Zenject;

public class CanvasWindowNotifier : MonoBehaviour {
    [SerializeField] private bool _allowCameraZoom;
    
    [Inject] private AdvTimerStarter  _advTimerStarter;

    
    private void OnEnable() {
        SystemEvents.WindowOpen(true);
        _advTimerStarter.DisableTimer();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(true);
        }
    }
    
    private void OnDisable() {
        _advTimerStarter.EnableTimer();
        SystemEvents.WindowOpen(false);
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(false);
        }
    }
}