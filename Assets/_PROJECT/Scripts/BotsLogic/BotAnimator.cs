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
        _botFlight.StartFlight += BotFlightOnStartFlight;
        _botFlight.LastBoostGet += BotFlightOnLastBoostGet;
        
    }

    
    private void BotFlightOnStartFlight() {
        Debug.Log("Бот Начал полет");
        _animator.SetBool(Run, true);
        _animator.SetTrigger(Fly);
    }

    private void BotFlightOnLastBoostGet() {
        Debug.Log("Бот получил ласт буст");
        OnStartWandering(false);
    }

    
    private void OnStartWandering(bool isRunning) {
        Debug.Log("Бот начал гулять");
        _animator.SetBool(Run, isRunning);
    }

    private void OnJump() {
        Debug.Log("Бот рыгнул");
        _animator.SetTrigger(Jump);
    }
}