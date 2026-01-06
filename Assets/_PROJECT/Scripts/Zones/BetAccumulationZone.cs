using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;


public class BetAccumulation : MonoBehaviour  {
    [SerializeField] private AnimationCurve _moneyCurve;
    [SerializeField] private float _accumulateDuration;

    public float CurrentMoneyAccum { get; private set; }
    private CancellationTokenSource _accumulateCTS;
    private float _elapsedTime = 0f;
    
    public void ResetBet() { 
        CurrentMoneyAccum = 0;
        _elapsedTime = 0f;
        BetVisual.Instantiate.PlayerBetVisual.text = "Ставка: 0";
        BetVisual.Instantiate.RewardVisual.text = "";
        BetVisual.Instantiate.XMultiplyVisual.text = "";
        StopAccumulate();
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
        while (!token.IsCancellationRequested && _elapsedTime < _accumulateDuration && CurrentMoneyAccum != playerMoney) {
            float t = _elapsedTime / _accumulateDuration;
            CurrentMoneyAccum =  _moneyCurve.Evaluate(t) * playerMoney;
            _elapsedTime += Time.deltaTime;
            BetVisual.Instantiate.PlayerBetVisual.text = "Ставка: " + CurrentMoneyAccum.ToString("F2");
            Debug.Log(CurrentMoneyAccum.ToString("F2"));
            await UniTask.Yield(token);
        }

        if (!token.IsCancellationRequested && _elapsedTime >= _accumulateDuration) {
            CurrentMoneyAccum = playerMoney;
            BetVisual.Instantiate.PlayerBetVisual.text = "Ставка: " + CurrentMoneyAccum.ToString("F2");
        }
    }

    
    
    
}
