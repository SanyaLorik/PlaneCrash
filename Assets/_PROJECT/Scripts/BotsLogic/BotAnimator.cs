using UnityEngine;

public class BotAnimator : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Run = Animator.StringToHash("isRunning");
    private static readonly int Fly = Animator.StringToHash("fly");
    [SerializeField] private Animator _animator;

    private BotFlight _botFlight;
    private BotWander _botWander;
    
    
    public void SetModel(SkinItemConfig skin) {
        _animator.avatar = skin.Avatar;
    }

    public void InitAnimator(BotFlight botFlight, BotWander botWander) {
        _botFlight =  botFlight;
        _botWander =  botWander;
        _botWander.OnJump += OnJump;
        _botWander.StartWandering += OnStartWandering;
        _botFlight.StartFlight += BotFlightOnOnStartFlight;
        _botFlight.EndFlight += BotFlightOnEndFlight;
        
    }

    
    private void BotFlightOnOnStartFlight() {
        _animator.SetTrigger(Fly);
    }

    private void BotFlightOnEndFlight() {
        OnStartWandering(false);
    }

    
    private void OnStartWandering(bool isRunning) {
        _animator.SetBool(Run, isRunning);
    }

    private void OnJump() {
        _animator.SetTrigger(Jump);
    }
}