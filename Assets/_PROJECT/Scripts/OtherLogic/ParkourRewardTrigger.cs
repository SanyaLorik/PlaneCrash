using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class ParkourRewardTrigger : MonoBehaviour {
    [SerializeField] private float _heightFly;
    [SerializeField] private float _durationFly;
    [SerializeField] private float _rewardMultiplier;
    [SerializeField, Range(0, 1)] private float _rangPerentageAmountForReward;
    [SerializeField, Range(0, 3)] private float _accumulateMultiplierMax;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Collider[] _interfereColliders;

    
    
    private long _reward;
    private float _accumulateMultiplier = 1f;
    private long RangPercentageAmount => _rangManager.GetNextRangePercentage(_rangPerentageAmountForReward);
    private float PlayerMultiplier => _multiplierCalculator.GetUpgradeMultiplierByLevel();


    [Inject] private LevelBounds _levelBounds;
    [Inject] private PlayerBank _bank;
    [Inject] private NumberFormatter _formatter;
    [Inject] private RangConfig _config;
    [Inject] private UpgradesCalculator _multiplierCalculator;
    [Inject] private PetsManager _petsManager;
    [Inject] private RangManager _rangManager;

    private void OnEnable() {
        _rangManager.RangChanged += OnRangChanged;
        _petsManager.GetPet += RecalculateReward;
    }
    
    private void Start() {
        // Типо за прохождение - n процентов от суммы ранга
        RecalculateReward();
    }

    private void RecalculateReward() {
        _reward = (long)(RangPercentageAmount * PlayerMultiplier * _accumulateMultiplier);
        _rewardText.text = _formatter.ValuteFormatter(_reward);
    }


    private void OnRangChanged() {
        _reward = (long)(RangPercentageAmount * PlayerMultiplier);
        _rewardText.text = _formatter.ValuteFormatter(_reward);
        
        // Сбрасываем накапливаемый множитель
        _accumulateMultiplier = 1f;
    }


    private bool _allowToReward = true;
    private CancellationTokenSource _token;
    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement player)) return;
        if (!_allowToReward) return;
        _allowToReward = false;
        _bank.AddMoney(_reward);
        // Обновление за прохождение
        _accumulateMultiplier = Mathf.Min(_accumulateMultiplier * _rewardMultiplier, _accumulateMultiplierMax);
        _reward = (long)(_reward * _accumulateMultiplier);
        Debug.Log($"За петов {PlayerMultiplier}");
        _rewardText.text = _formatter.ValuteFormatter(_reward);

        MovePlayerToSpawn(player);
        _token?.Cancel();
        _token = new CancellationTokenSource();
        UniTaskHelper.TimerAction(
            10f,
            () => _allowToReward = true,
            _token.Token
        ).Forget();
    }

    
    
    
    private async void MovePlayerToSpawn(PlayerMovement player) {
        await MoveParabola(player.Controller, _levelBounds.PlayerSpawnPoint.position, _heightFly, _durationFly);
    }

    private async UniTask MoveParabola(CharacterController controller, Vector3 target, float height, float duration) {
        Vector3 start = controller.transform.position;
        float time = 0f;
        _interfereColliders.ForEach(c => c.DisactiveSelf());
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 pos = Vector3.Lerp(start, target, t);

            float parabola = 4 * height * t * (1 - t);
            pos.y += parabola;

            Vector3 delta = pos - controller.transform.position;
            controller.Move(delta);
            

            await UniTask.Yield();
        }
        _interfereColliders.ForEach(c => c.ActiveSelf());
    }
}
