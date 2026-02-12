using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;


public class BetAccumulation : MonoBehaviour  {
    [SerializeField] private AnimationCurve _moneyCurve;
    [SerializeField] private float _accumulateDuration;

    private float _elapsedTime;
    private CancellationTokenSource _accumulateCTS;
    
    private ZoneManager _zoneManager;
        
    [Inject]
    public void Init(ZoneManager zoneManager) {
        _zoneManager = zoneManager;
    }
    
    
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
        float betAmount = _zoneManager.BetAmount;
        if (betAmount == 0) {
            _elapsedTime = 0f;
        }
        while (!token.IsCancellationRequested && _elapsedTime < _accumulateDuration && !Mathf.Approximately(betAmount, playerMoney)) {
            float t = _elapsedTime / _accumulateDuration;
            betAmount = _moneyCurve.Evaluate(t) * playerMoney;
            _elapsedTime += Time.deltaTime;
            _zoneManager.ChangeBet(betAmount);
            await UniTask.Yield(token);
        }

        // Если время кончилось
        if (!token.IsCancellationRequested && _elapsedTime >= _accumulateDuration) {
            betAmount = playerMoney;
            _zoneManager.ChangeBet(betAmount);
        }
    }

    
    
    
}
