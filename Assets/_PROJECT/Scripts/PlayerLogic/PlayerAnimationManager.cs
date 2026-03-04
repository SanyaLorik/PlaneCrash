using UnityEngine;
using Zenject;

public class PlayerAnimationManager : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int DoubleJump = Animator.StringToHash("doubleJump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    private static readonly int Fly = Animator.StringToHash("fly");
    [SerializeField] private Animator _animator;
    
    
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private PlayerMovement _playerMovement;

    
    
    private void OnEnable() {
        _stateManager.ChangeState += StateManagerOnChangeState;
        _playerMovement.JumpPressed += FirstJumpAnimation;
        _playerMovement.DoubleJumpPressed += SecondJumpAnimation;
        _playerMovement.RunningStateChanged += PlayerMovementOnRunningStateChanged;
        
    }

    private void PlayerMovementOnRunningStateChanged(bool isRunning) {
        _animator.SetBool(Run, isRunning);
    }

    private void FirstJumpAnimation() {
        _animator.SetTrigger(Jump);
    }
    
    
    private void SecondJumpAnimation() {
        if (_stateManager.CurrentState != PlayerState.Walking) return;
        _animator.SetTrigger(DoubleJump);
    }

    
    
    private void StateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            PlayerMovementOnRunningStateChanged(true);
            _animator.SetTrigger(Fly);
        }
        else if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
            PlayerMovementOnRunningStateChanged(false);
        }
    }
    
    
    private void OnDisable() {
        _stateManager.ChangeState -= StateManagerOnChangeState;
        _playerMovement.JumpPressed -= FirstJumpAnimation;
        _playerMovement.DoubleJumpPressed -= SecondJumpAnimation;
        _playerMovement.RunningStateChanged -= PlayerMovementOnRunningStateChanged;
    }
    
}
