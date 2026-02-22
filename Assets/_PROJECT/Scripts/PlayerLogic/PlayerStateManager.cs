using System;
using Architecture_M;
using UnityEngine;
using Zenject;

public class PlayerStateManager : MonoBehaviour{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    [SerializeField] private Transform _startFlightPoint;

    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private ZoneManager _zoneManager;
    
    public event Action<PlayerState> ChangeState;
    
    public float StartFlightPositionZ { get; private set; }

    private void Awake() {
        StartFlightPositionZ = _startFlightPoint.position.z;
    }

    public int CurrentPlayerDistance() {
        return CurrentState switch {
            PlayerState.Walking => 0,
            PlayerState.Cruisered => (int)_zoneManager.DistanceToCruise,
            _ => (int)(transform.position.z - _startFlightPoint.position.z)
        };

    }
    

    [field: SerializeField] public PlayerState CurrentState { get; private set; } = PlayerState.Walking;
    public PlayerState BeforeState { get; private set; } = PlayerState.Walking;

    
    
    public void ChangePlayerState(PlayerState newState) {
        if (CurrentState == newState) {
            return;
        }

        BeforeState = CurrentState;
        CurrentState = newState;
        if (newState == PlayerState.Flight) {
            _interstitialDelaying.DisableTimer();
            if (StartFlightPositionZ == 0) {
                StartFlightPositionZ = transform.position.z;
            }
            Debug.Log("StartFlightPositionZ : " + StartFlightPositionZ );
        }
        else {
            _interstitialDelaying.EnableTimer();
            if (newState  == PlayerState.Walking) {
                _jumpParticlesController.Play();
            }
        }
        

        Debug.Log("CurrentPlayerState: " + CurrentState);
        ChangeState?.Invoke(CurrentState);
    }


}
