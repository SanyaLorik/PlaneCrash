using UnityEngine;
using Zenject;

public class PlayerAnimator : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int DoubleJump = Animator.StringToHash("doubleJump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    private static readonly int Fly = Animator.StringToHash("fly");
    [SerializeField] private Animator _animator;

    [SerializeField] private SkinElementsController _skinElementsController;
    
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private PlayerMovement _playerMovement;

    public void SetSkinElementsController(SkinElementsController skinElementsController) {
        _skinElementsController = skinElementsController;
    }
    
    private void OnEnable() {
        _stateManager.ChangeState += StateManagerOnChangeState;
        _playerMovement.JumpPressed += FirstJumpAnimation;
        _playerMovement.DoubleJumpPressed += SecondJumpAnimation;
        _playerMovement.RunningStateChanged += PlayerMovementOnRunningStateChanged;
        
        
        _playerMovement.Floored += PlayerMovementOnFloored;
    }

    private void PlayerMovementOnFloored() {
        _skinElementsController.EnableShadow();
    }

    private void PlayerMovementOnRunningStateChanged(bool isRunning) {
        _animator.SetBool(Run, isRunning);
    }

    private void FirstJumpAnimation() {
        _animator.SetTrigger(Jump);
        _skinElementsController.DisableShadow();
    }
    
    
    private void SecondJumpAnimation() {
        if (_stateManager.CurrentState != PlayerState.Walking) return;
        _animator.SetTrigger(DoubleJump);
    }

    
    
    private void StateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            PlayerMovementOnRunningStateChanged(true);
            _animator.SetTrigger(Fly);
            _skinElementsController.DisableShadow();
        }
        else if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
            PlayerMovementOnRunningStateChanged(false);
            _skinElementsController.EnableShadow();
        }
        
        else if (state == PlayerState.TrampolineJumping) {
            _skinElementsController.DisableShadow();
        }
    }
    
    
    private void OnDisable() {
        _stateManager.ChangeState -= StateManagerOnChangeState;
        _playerMovement.JumpPressed -= FirstJumpAnimation;
        _playerMovement.DoubleJumpPressed -= SecondJumpAnimation;
        _playerMovement.RunningStateChanged -= PlayerMovementOnRunningStateChanged;
    }
    
}
