using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShopAnimator : MonoBehaviour {
    [SerializeField] private Button[] _shopButtons;
    [SerializeField] private Button _closeShopButton;
    [SerializeField] private GameObject _shopCanvas;
    
    [Inject] private IInputActivity _inputActivity;
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private TutorialCompiller  _tutorialCompiller;


    private void Awake() {
        foreach (var button in _shopButtons) {
            button.AddListenerWithSound(OpenShop);
        }
        _closeShopButton.AddListenerWithSound(CloseShop);
    }

    private void Start() {
        _shopCanvas.DisactiveSelf();
    }

    private void OpenShop() {
        _interstitialDelaying.DisableTimer();
        _inputActivity.Disable();
        _shopCanvas.ActiveSelf();
    }
    
    
    private void CloseShop() {
        if (_tutorialCompiller.TutorialPassed) {
            _interstitialDelaying.EnableTimer();
        }
        _inputActivity.Enable();
        _shopCanvas.DisactiveSelf();
    }
}
