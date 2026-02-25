using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;


public class DelayedTrigger : MonoBehaviour {
    [SerializeField] private float _duration = 2f;
    [SerializeField] private Image _progress;
    [SerializeField] private Image _notAvailableImage;
    [SerializeField] private Color _notAvailableColor;
    [SerializeField] private Color _availableColor;
    
    private CancellationTokenSource _tokenSource;


    public void DelayedTriggerAction(Action action) {
        _tokenSource?.Cancel();
        _tokenSource = new CancellationTokenSource();
        ProgressVisual(_tokenSource.Token, action).Forget();
    }

    public void CancelTriggerAction() {
        _tokenSource?.Cancel();
        _progress.fillAmount = 1f;
    }
    
    public void SetUnvailable() {
        if (_notAvailableImage.color != _notAvailableColor) {
            _notAvailableImage.color = _notAvailableColor;
        }
    }

    public void SetAvailable() {
        if (_notAvailableImage.color != _availableColor) {
            _notAvailableImage.color = _availableColor;
        }
    }

    private async UniTask ProgressVisual(CancellationToken token, Action action) {
        float elapsedTime = 0f;
        _progress.fillAmount = 0f;
        while (!token.IsCancellationRequested && elapsedTime < _duration) {
            elapsedTime += Time.deltaTime;
            _progress.fillAmount = Mathf.Clamp01(elapsedTime / _duration);
            await UniTask.Yield();
        }
        _progress.fillAmount = 1f;
        if (!token.IsCancellationRequested) {
            action?.Invoke();
        }

    }


    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }
}
