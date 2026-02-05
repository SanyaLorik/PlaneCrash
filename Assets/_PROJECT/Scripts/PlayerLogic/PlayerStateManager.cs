using System;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

public class PlayerStateManager : MonoBehaviour {
    [SerializeField] LayerMask _groundMask;
    [SerializeField] LayerMask _cruiserMask;
    [SerializeField] LayerMask _floorMask;
    [SerializeField] private float _distanceCheck = 0.1f;
    [SerializeField] private Renderer _floor;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _playerFootPoint;
    [SerializeField] private JumpParticlesController _jumpParticlesController;


    private LevelBounds _levelBounds;
    
    public event Action<PlayerState> ChangeState;
    public event Action LandedInSpawn;
    public float StartFlightPosition { get; private set; }


    [Inject]
    public void Init(LevelBounds  levelBounds) {
        _levelBounds = levelBounds;
    }
    
    
    public float CurrentPlayerDistance
        => CurrentState == PlayerState.Walking ? 0f : transform.position.z;
    

    public PlayerState CurrentState { get; private set; } = PlayerState.Walking;
    
    private void Update() {
        CheckGround();
        FloorCheck();
    }


    public void ChangePlayerState(PlayerState newState) {
        CurrentState = newState;
        if (newState  == PlayerState.Flight) {
            StartFlightPosition = transform.position.z;
            // Debug.Log("StartFlightPosition " + transform.position.z);
        }

        if (newState  == PlayerState.Grounded) {
            // Debug.Log("EndFlightPosition " + transform.position.z);
        }
        
        ChangeState?.Invoke(CurrentState);
        
    }

    private void CheckGround() {
        if (CurrentState == PlayerState.Cruisered || CurrentState == PlayerState.Grounded) {
            return;
        }
        Vector3 origin = transform.position;
 
        if (Physics.Raycast(origin, Vector3.down,  _distanceCheck, _cruiserMask)) {
            Debug.Log("Попали!");
            Debug.Log($"_levelBounds.MinimumY = {_levelBounds.MinY}, игрок в {transform.position.y}" );
            ChangePlayerState(PlayerState.Cruisered);
            return;
        }
        if (transform.position.y <= _levelBounds.MinY + _distanceCheck) {
            Debug.Log("Упали");
            Debug.Log($"_levelBounds.MinimumY = {_levelBounds.MinY}, игрок в {transform.position.y}" );
            ChangePlayerState(PlayerState.Grounded);
            return;
        }
    }


    private bool _wasOnFloor;
    private void FloorCheck() {
        Vector3 feetPoint = _playerFootPoint.position;
        
        bool isOnFloor = Physics.Raycast(feetPoint, Vector3.down, _floorMask);

        if (isOnFloor && !_wasOnFloor) {
            LandedInSpawn?.Invoke();
            _jumpParticlesController.Play();
        }

        _wasOnFloor = isOnFloor;
    }
}
