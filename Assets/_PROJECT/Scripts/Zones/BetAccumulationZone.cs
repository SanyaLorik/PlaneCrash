using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;


public class BetAccumulation : MonoBehaviour  {
    [SerializeField] private AnimationCurve _moneyCurve;
    [SerializeField] private float _accumulateDuration;

    private float _elapsedTime;
    private CancellationTokenSource _accumulateCTS;
    
    public void StopAccumulate() {
        if (_accumulateCTS == null) return;
        _accumulateCTS.Cancel();
        _accumulateCTS.Dispose();
        _accumulateCTS = null;
    }

    private void Start() {
        _accumulateCTS = new CancellationTokenSource();
    }

    private void OnDestroy() {
        StopAccumulate();
    }
    

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerBank bank)) {
            StopAccumulate();
            _accumulateCTS = new CancellationTokenSource();
            AccumulateBet(_accumulateCTS.Token, bank);
        }    
    }

    private void OnTriggerExit(Collider collider) {
        StopAccumulate();
    }

    
    
    private async UniTaskVoid AccumulateBet(CancellationToken token, PlayerBank bank) {
        float playerMoney = bank.PlayerCapital;
        float currentBet = ZoneManager.Instance.CurrentBet;
        if (currentBet == 0) {
            _elapsedTime = 0f;
        }
        while (!token.IsCancellationRequested && _elapsedTime < _accumulateDuration && currentBet != playerMoney) {
            float t = _elapsedTime / _accumulateDuration;
            currentBet = _moneyCurve.Evaluate(t) * playerMoney;
            _elapsedTime += Time.deltaTime;
            ZoneManager.Instance.ChangeBet(currentBet);
            await UniTask.Yield(token);
        }

        // Если время кончилось
        if (!token.IsCancellationRequested && _elapsedTime >= _accumulateDuration) {
            currentBet = playerMoney;
            ZoneManager.Instance.ChangeBet(currentBet);
        }
    }

    
    
    
}
