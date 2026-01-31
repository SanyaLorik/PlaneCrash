using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class TorpedoSpawn : MonoBehaviour {
    [SerializeField] private Torpedo _torpedoPrefab;
    [SerializeField] private GameObject _circlePrefab;
    [SerializeField] private PairedValue<float> _diapasoneSpawnProgress;
    [SerializeField] private float _chanseToSpawn;
        
    
    
    
    [Header("Настройки анимации")]
    [SerializeField] private float pulseScale = 1.2f;      // максимальный масштаб при пульсации
    [SerializeField] private float pulseDuration = 0.5f;   // время на один цикл пульсации
    [SerializeField] private float _rotationSpeed;
    
    
    
    private Vector3 _currentHitPoint;
    private PlayerMovement _player;
    private LevelBounds _levelBounds;
    
    
    
    
    [Inject]
    private void Init(PlayerMovement player, LevelBounds levelBounds) {
        _player = player;
        player.SetBoost += PlayerOnSetBoost;
        _levelBounds =  levelBounds;
    }


    private Vector3 _spawnPos;
    private void PlayerOnSetBoost() {
        if (Random.value < _chanseToSpawn) {
            return;
        }
        // Debug.Log("PlayerOnSetBoost");

        float playerProgress = Random.Range(_diapasoneSpawnProgress.From, _diapasoneSpawnProgress.To);
        float spawnY = _levelBounds.MinimumY;
        _currentHitPoint = _player.GetPlayerPositionAt(playerProgress);
        _spawnPos = new Vector3(_currentHitPoint.x, spawnY, _currentHitPoint.z);
        ShowWarning();
        

        // Время долета до точки у торпеды
        float torpedoTime = (_currentHitPoint.y - spawnY) / _torpedoPrefab.Speed;

        // Доля кривой, которую игрок проходит за время полета торпеды
        float playerTorpedoProgress = torpedoTime / _player._segmentDuration;
        // На каком прогрессе ее спавнить
        float progressFire = playerProgress - playerTorpedoProgress;
        progressFire = Mathf.Max(progressFire, 0f);
        
        // Когда ее запустить чтоб она попала в нужную точку одновременно с игроком
        float fireTime = progressFire * _player._segmentDuration;
        
        Invoke(nameof(SpawnTorpedo), fireTime);
    }


    private GameObject _circleObject;
    private void ShowWarning() {
        if (_circleObject != null) {
            _circleObject.transform.position = _spawnPos;
            return;
        }

        _circleObject = Instantiate(_circlePrefab, _spawnPos, Quaternion.identity);
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
    
    
    

    private void SpawnTorpedo() {
        Debug.Log("Torpedo spawned!");
        Torpedo torpedo = Instantiate(_torpedoPrefab, _spawnPos, Quaternion.identity);
        Torpedo tScript = torpedo.GetComponent<Torpedo>();
        tScript.Launch(_currentHitPoint, _torpedoPrefab.Speed);
    }
    
    

}
