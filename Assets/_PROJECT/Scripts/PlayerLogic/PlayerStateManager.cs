using System;
using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class PlayerStateManager : MonoBehaviour{
    [SerializeField] private GameObject _flightCanvasContainerShow;
    [SerializeField] private GameObject _flightCanvasContainerHide;
    
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    [SerializeField] private Transform _startFlightPoint;

    
    [Inject] private IInterstitialDelaying  _interstitialDelaying;
    [Inject] private ZoneManager _zoneManager;
    [Inject] private TutorialCompiller _tutorialCompiller;
    [Inject] private PlayerMovement _playerMovement;
    
    public event Action<PlayerState> ChangeState;
    
    public float StartFlightPositionZ { get; private set; }


    private void OnEnable() {
        _playerMovement.Floored += PlayerMovementOnFloored;
    }

    
    private void PlayerMovementOnFloored() {
        _jumpParticlesController.Play();
    }

    private void Awake() {
        StartFlightPositionZ = _startFlightPoint.position.z;
        SetWalkingCanvas();
        SetGroundedCanvas();
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
        
        if (newState != PlayerState.Flight && _tutorialCompiller.TutorialPassed) {
            _interstitialDelaying.EnableTimer();
        }
        
        BeforeState = CurrentState;
        CurrentState = newState;
        if (newState == PlayerState.Flight) {
            SetFlightCanvas();
            _interstitialDelaying.DisableTimer();
            if (StartFlightPositionZ == 0) {
                StartFlightPositionZ = transform.position.z;
            }
            Debug.Log("StartFlightPositionZ : " + StartFlightPositionZ );
        }
        else if (_tutorialCompiller.TutorialPassed) {
            _interstitialDelaying.EnableTimer();
        }
        
        
        if (newState == PlayerState.Walking) {
            SetWalkingCanvas();
        }
        else if (newState == PlayerState.Cruisered || newState == PlayerState.Grounded) {
            SetGroundedCanvas();
        }
        

        Debug.Log("CurrentPlayerState: " + CurrentState);
        ChangeState?.Invoke(CurrentState);
    }

    private void SetGroundedCanvas() {
        _flightCanvasContainerShow.DisactiveSelf();
    }

    private void SetWalkingCanvas() {
        _flightCanvasContainerHide.ActiveSelf();
    } 
    
    private void SetFlightCanvas() {
        _flightCanvasContainerHide.DisactiveSelf();
        _flightCanvasContainerShow.ActiveSelf();
    }


}
