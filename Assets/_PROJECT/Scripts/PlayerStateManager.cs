using System;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour {
    [SerializeField] LayerMask _groundMask;
    [SerializeField] LayerMask _cruiserMask;
    [SerializeField] private float _distanceCheck = 0.1f;

    public event Action<PlayerState> OnChangeState;

    public float CurrentPlayerDistance
        => CurrentState == PlayerState.Walking ? 0f : transform.position.z;
    

    public PlayerState CurrentState { get; private set; } = PlayerState.Walking;
    
    private void Update() {
        CheckGround();
    }

    public float StartFlightPosition { get; private set; }


    public void ChangePlayerState(PlayerState newState) {
        CurrentState = newState;
        if (newState  == PlayerState.Flight) {
            StartFlightPosition = transform.position.z;
            Debug.Log("StartFlightPosition " + transform.position.z);
            
        }

        if (newState  == PlayerState.Grounded) {
            Debug.Log("EndFlightPosition " + transform.position.z);
        }
        
        OnChangeState?.Invoke(CurrentState);
        
    }

    private void CheckGround() {
        if (CurrentState == PlayerState.Cruisered || CurrentState == PlayerState.Grounded) {
            return;
        }
        Vector3 origin = transform.position;
        if (Physics.Raycast(origin, Vector3.down,  _distanceCheck, _groundMask)) {
            Debug.Log("Упали");
            ChangePlayerState(PlayerState.Grounded);
        }
        if (Physics.Raycast(origin, Vector3.down,  _distanceCheck, _cruiserMask)) {
            Debug.Log("Попали!");
            ChangePlayerState(PlayerState.Cruisered);
        }
    }


}
