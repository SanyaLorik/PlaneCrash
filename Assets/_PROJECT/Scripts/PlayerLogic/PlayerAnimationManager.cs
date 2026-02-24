using UnityEngine;
using Zenject;

public class PlayerAnimationManager : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int DoubleJump = Animator.StringToHash("doubleJump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    [SerializeField] private Animator _animator;
    
    
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private PlayerMovement _playerMovement;

    
    
    private void OnEnable() {
        _stateManager.ChangeState += StateManagerOnChangeState;
        // _stateManager.FlooredInChillZone += () => PlayerMovementOnRunningStateChanged(false);
        _playerMovement.OnJumpPressed += FirstJumpAnimation;
        _playerMovement.OnDoubleJumpPressed += SecondJumpAnimation;
        _playerMovement.OnRunningStateChanged += PlayerMovementOnRunningStateChanged;
        
    }

    private void PlayerMovementOnRunningStateChanged(bool isRunning) {
        _animator.SetBool(Run, isRunning);
    }

    private void FirstJumpAnimation() {
        _animator.SetTrigger(Jump);
    }
    
    
    private void SecondJumpAnimation() {
        _animator.SetTrigger(DoubleJump);
    }

    
    
    private void StateManagerOnChangeState(PlayerState obj) {
        // FLIGHT ANIMATION IN DEV...
    }
}
