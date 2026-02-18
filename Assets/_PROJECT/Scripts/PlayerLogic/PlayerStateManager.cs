using System;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour {
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    [SerializeField] private Transform _startFlightPoint;

    public event Action<PlayerState> ChangeState;
    
    public float StartFlightPositionZ { get; private set; }

    private void Awake() {
        StartFlightPositionZ = _startFlightPoint.position.z;
    }
    
    public float CurrentPlayerDistance
        => CurrentState == PlayerState.Walking ? 0f : transform.position.z - StartFlightPositionZ;
    

    [field: SerializeField] public PlayerState CurrentState { get; private set; } = PlayerState.Walking;
    public PlayerState BeforeState { get; private set; } = PlayerState.Walking;

    public void ChangePlayerState(PlayerState newState) {
        if (CurrentState == newState) {
            return;
        }

        BeforeState = CurrentState;
        CurrentState = newState;
        if (newState  == PlayerState.Flight) {
            StartFlightPositionZ = transform.position.z;
        }
        else if (newState  == PlayerState.Walking) {
            _jumpParticlesController.Play();
        }

        Debug.Log("CurrentPlayerState: " + CurrentState);
        ChangeState?.Invoke(CurrentState);
    }
}
