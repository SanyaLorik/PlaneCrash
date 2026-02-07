using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;

public class PlayerVisual : MonoBehaviour {
    [Header("Головокружение")]
    [SerializeField] private ParticleSystem _dizzyPS;
    [SerializeField] private float _dizzyDuration;
    
    
    [Header("Покупка")]
    [SerializeField] private ParticleSystem _upgradePS;
    
    CancellationTokenSource _tokenSource;

    private void Start() {
        StopDizzy();
    }


    public void SetBought() {
        _upgradePS.Play();
    }
    
    
    public void StartDizzy() {
        StopDizzy();
        _dizzyPS.Play();
        _tokenSource = new CancellationTokenSource();
        UniTaskHelper.TimerAction(
            _dizzyDuration,
            StopDizzy,
            _tokenSource.Token
        ).Forget();
    }
     
    
    private void StopDizzy() {
        _dizzyPS.Stop();
    }

    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }
}
