using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerVisual : MonoBehaviour {
    [Header("Головокружение")]
    [SerializeField] private ParticleSystem _dizzyPS;
    [SerializeField] private float _dizzyDuration;
    
    
    [Header("Покупка")]
    [SerializeField] private ParticleSystem _upgradePS;
    
    [Header("Телепорт")]
    [SerializeField] private ParticleSystem _teleportPS;
    
    [Header("Бустинг")]
    [SerializeField] private ParticleSystem _boostPS;
    
    [Header("Префаб щита")]
    [SerializeField] private  MinusShieldView _minusShieldView;
    [SerializeField] private Transform _parentToShield;
    [SerializeField] private float _radiusToSpawn;
    
    
    private CancellationTokenSource _tokenSource;

    
    [Inject] PlayerMovement _playerMovement;
    
    
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

    public void MinusShield(int count) {
        Vector3 posAroundPlayer = RectTransformHelper.GetPointAroundPoint(_radiusToSpawn, _playerMovement.transform.position);
        MinusShieldView shieldView = Instantiate(_minusShieldView, _parentToShield);
        shieldView.transform.position = posAroundPlayer;
        shieldView.SetCount(count);
    }
    
    
    public void TeleportParticles() {
        _teleportPS.Play();
    }

    public void SetBoosted() {
        _boostPS.Play();
    }
    
    
    private void StopDizzy() {
        _dizzyPS.Stop();
    }

    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }


}