using Architecture_M;
using UnityEngine;
using Zenject;

public class CanvasWindowNotifier : MonoBehaviour {
    [SerializeField] private bool _allowCameraZoom;
    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private TutorialCompiller _tutorialCompiller;
    [Inject] private PlayerStateManager _stateManager;

    
    private void OnEnable() {
        SystemEvents.WindowOpen(true);
        _interstitialDelaying.DisableTimer();
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(true);
        }
    }
    
    private void OnDisable() {
        if (_tutorialCompiller.TutorialPassed 
            && 
            (_stateManager.CurrentState == PlayerState.Walking || _stateManager.CurrentState == PlayerState.TrampolineJumping)) 
        {
            _interstitialDelaying.EnableTimer();
        }
        SystemEvents.WindowOpen(false);
        if (!_allowCameraZoom) {
            SystemEvents.ForbidZoomChange(false);
        }
    }
}