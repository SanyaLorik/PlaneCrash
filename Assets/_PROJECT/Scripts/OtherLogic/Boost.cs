using System;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class Boost : MonoBehaviour, IMagnetic {
    [SerializeField] private Renderer _renderer;
    
    
    
    public AnimationCurve randomTrajectory;
    public Vector3 nextBooster;
    public bool hasTrap;

    public bool InPool;
    
    private Material _mat;
    private Tween _glowTween;
    
    private PlayerMovement _playerMovement;
    
    [Inject]
    public void Init(PlayerMovement playerMovement) {
        _playerMovement = playerMovement;
    }


    private Vector3 _spawnRotation;
    private void Awake() {
        _mat = _renderer.material;
        _mat.EnableKeyword("_EMISSION");
        _spawnRotation = transform.localEulerAngles;
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement player)) {
            if (player.IsBombed) {
                return;
            }
            Collect();
        }
        else if (collider.gameObject.TryGetComponent(out BotFlight bot)) {
            bot.SetBooster(randomTrajectory, nextBooster);
        }
    }
    


    public Vector3 Position =>  transform.position;
    public MagneticType Type { get; } = MagneticType.Boost;


    public bool CanBeMagnetic { get; set; } = true;

    
    
    public void Attract(Vector3 target, float speed) {
        // Debug.Log("Притяжение буста " + transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            

        // Debug.Log(Vector3.SqrMagnitude(transform.position - target));
        if (Vector3.SqrMagnitude(transform.position - target) <= 1f) {
            Collect();
        }
    }
    
    public void SetBoostDefault() {
        transform.DOKill();          
        _glowTween?.Kill();          
        _mat.SetColor("_EmissionColor", Color.black); // сброс цвета
        InPool = false;
        transform.localEulerAngles = _spawnRotation;
    }

    public void SetBoostPersonalityVisibleAndRevealTheHiddenInnerEnergeticMetaphysicalGameplayEssenceOfThisSpecificAccelerationEntityWhileSynchronizingItsVisualAuraWithPlayerPerceptionSystemsTheLivingBreathingDigitalUniverse() {
        transform
            .DORotate(Vector3.up * 360f, .7f, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetLink(gameObject);

        LightBreeze();
    }
    
    private void LightBreeze() {

        _glowTween = DOTween.To(
                () => _mat.GetColor("_EmissionColor"),
                c => _mat.SetColor("_EmissionColor", c),
                Color.cyan * 3f, // яркость
                0.6f
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(gameObject);
    }
    
        
    private void Collect() {
        _playerMovement.SetBooster(randomTrajectory, nextBooster);
        gameObject.DisactiveSelf();
        CanBeMagnetic = false;
    }

    
    
}
