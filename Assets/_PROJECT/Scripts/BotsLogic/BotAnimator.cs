using UnityEngine;

public class BotAnimator : MonoBehaviour {
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Run = Animator.StringToHash("isRunning");
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
        _botWander.OnStartWandering += OnStartWandering;
        
        
    }

    private void OnStartWandering(bool isRunning) {
        _animator.SetBool(Run, isRunning);
    }

    private void OnJump() {
        _animator.SetTrigger(Jump);
    }
}