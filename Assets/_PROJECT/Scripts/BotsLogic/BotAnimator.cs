using UnityEngine;

public class BotAnimator : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    private static readonly int Fly = Animator.StringToHash("fly");
    [SerializeField] private Animator _animator;

    private BotFlight _botFlight;
    private BotWander _botWander;
    private SkinElementsController _skinController;
    
    public void SetModelData(Avatar avatar, SkinElementsController controller) {
        _animator.avatar = avatar;
        _skinController = controller;
        controller.EnableShadow();
    }
    
    public void InitAnimator(BotFlight botFlight, BotWander botWander) {
        _botFlight =  botFlight;
        _botWander =  botWander;
        _botWander.OnJump += OnJump;
        _botWander.StartWandering += OnStartWandering;
        _botFlight.StartFlight += BotFlightOnStartFlight;
        _botFlight.LastBoostGet += BotFlightOnLastBoostGet;
        _botWander.Grounded += BotGrounded;
    }

    private void BotGrounded(bool grounded) {
        if (grounded) {
            _skinController.EnableShadow();
        }
        else {
            _skinController.DisableShadow();
        }
    }


    private void BotFlightOnStartFlight() {
        _animator.SetBool(Run, true);
        _animator.SetTrigger(Fly);
        _skinController.DisableShadow();
    }

    private void BotFlightOnLastBoostGet() {
        OnStartWandering(false);
        _skinController.EnableShadow();
    }

    
    private void OnStartWandering(bool isRunning) {
        _animator.SetBool(Run, isRunning);
        _skinController.EnableShadow();
    }

    private void OnJump() {
        _animator.SetTrigger(Jump);
    }
}