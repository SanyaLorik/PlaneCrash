using System.Collections;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class TorpedoSpawn : MonoBehaviour {
    [SerializeField] private Torpedo _torpedoPrefab;
    [SerializeField] private PairedValue<float> _diapasoneSpawnProgress;
    [Range(0f,1f),SerializeField] private float _chanseToSpawn;

    [SerializeField] private GameObject _rocketWarning;
    [SerializeField] private GameObject _circlePrefab;
        
    
    
    
    [Header("Настройки анимации")]
    [SerializeField] private float pulseScale = 1.2f;      // максимальный масштаб при пульсации
    [SerializeField] private float pulseDuration = 0.5f;   // время на один цикл пульсации
    [SerializeField] private float _rotationSpeed;
    
    
    
    
    
    private Vector3 _currentHitPoint;
    
    
    private PlayerMovement _player;
    private PlayerStateManager _stateManager;
    [Inject] private LevelBounds _levelBounds;
    [Inject] private BoostSpawner _boostSpawner;
    [Inject] TutorialCompiller _tutorialCompiller;
    
    
    [Inject]
    private void Init(PlayerMovement player, PlayerStateManager stateManager) {
        _player = player;
        _stateManager = stateManager;
        player.SetBoost += PlayerOnSetBoost;
        _stateManager.ChangeState += StateManagerOnChangeState;
    }

    private void StateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
            if (_circleObject != null) {
                _circleObject.DisactiveSelf();
                _rocketWarnObject.DisactiveSelf();
            }
        }
    }

    private void PlayerOnSetBoost() {
        float playerProgress = Random.Range(_diapasoneSpawnProgress.From, _diapasoneSpawnProgress.To);
        _currentHitPoint = _player.GetPlayerPositionAt(playerProgress);
        if (Random.value > _chanseToSpawn || _currentHitPoint.z < _boostSpawner.MinDistance) {
            return;
        }
        float spawnX;
        if (!_tutorialCompiller.TutorialPassed) {
            float sign = Random.value > 0.5f ? -1 : 1;
            spawnX = _currentHitPoint.x + Random.Range(10, 20) * sign;
            _currentHitPoint.x = spawnX;
        }
        else {
            spawnX = _currentHitPoint.x;
        }
        float spawnY = _levelBounds.MinY;
        
        Vector3 spawnPos = new Vector3(spawnX, spawnY, _currentHitPoint.z);
        
        
        ShowWarning(spawnPos);
        

        // Время долета до точки у торпеды
        float torpedoTime = (_currentHitPoint.y - spawnY) / _torpedoPrefab.Speed;

        // Доля кривой, которую игрок проходит за время полета торпеды
        float playerTorpedoProgress = torpedoTime / _player.SegmentDuration;
        // На каком прогрессе ее спавнить
        float progressFire = playerProgress - playerTorpedoProgress;
        progressFire = Mathf.Max(progressFire, 0f);
        
        // Когда ее запустить чтоб она попала в нужную точку одновременно с игроком
        float fireTime = progressFire * _player.SegmentDuration;

        StartCoroutine(WaitForTorpedaRoutine(fireTime, _currentHitPoint, spawnPos));

    }

    private IEnumerator WaitForTorpedaRoutine(float time, Vector3 predictedHitPoint, Vector3 spawnPos) {
        yield return new WaitForSeconds(time);
        SpawnTorpedo(predictedHitPoint, spawnPos);
    }


    private GameObject _circleObject;
    private GameObject _rocketWarnObject;
    private void ShowWarning(Vector3 spawnPos) {
        if (_circleObject != null) {
            _circleObject.transform.position = spawnPos;
            _rocketWarnObject.transform.position = spawnPos;
            if (!_circleObject.activeSelf) {
                _circleObject.ActiveSelf();
                _rocketWarnObject.ActiveSelf();
            }
            return;
        }

        _circleObject = Instantiate(_circlePrefab, spawnPos, Quaternion.identity);
        _rocketWarnObject = Instantiate(_rocketWarning.gameObject, spawnPos, Quaternion.identity);
        // Вращаем бесконечно вокруг Y
        _circleObject.transform.DORotate(new Vector3(0, 360f, 0), 360f / _rotationSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetLink(_circleObject);
        Vector3 _initialScale = _circleObject.transform.localScale;
        transform.DOScale(_initialScale * pulseScale, pulseDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(_circleObject);
    }
    
    
    

    private void SpawnTorpedo(Vector3 predictedHitPoint, Vector3 spawnPos) {
        Torpedo torpedo = Instantiate(_torpedoPrefab, spawnPos, Quaternion.identity);
        Torpedo tScript = torpedo.GetComponent<Torpedo>();
        tScript.Launch(predictedHitPoint, _torpedoPrefab.Speed);
    }
    
    

}
