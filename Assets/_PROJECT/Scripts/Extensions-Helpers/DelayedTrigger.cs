using System.Threading;
using _PROJECT.Scripts.Helpers;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;


public class DelayedTrigger : MonoBehaviour {
    [SerializeField] private float _duration = 2f;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _progress;
    
    
    private CancellationTokenSource _tokenSource;
    

    public void DelayedTriggerAction(Action action) {
        _tokenSource?.Cancel();
        _tokenSource = new CancellationTokenSource();
        ProgressVisual(_tokenSource.Token, action).Forget();
    }

    public void CancelTriggerAction() {
        _tokenSource?.Cancel();
        _canvas.DisactiveSelf();
    }

    private async UniTask ProgressVisual(CancellationToken token, Action action) {
        float elapsedTime = 0f;
        _progress.fillAmount = 0f;
        _canvas.ActiveSelf();
        while (!token.IsCancellationRequested && elapsedTime < _duration) {
            elapsedTime += Time.deltaTime;
            _progress.fillAmount = Mathf.Clamp01(elapsedTime / _duration);
            await UniTask.Yield();
        }

        if (!token.IsCancellationRequested) {
            action?.Invoke();
        }
        _canvas.DisactiveSelf();
    }


    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }
}
