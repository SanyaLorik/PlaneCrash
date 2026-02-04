using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

public class Money2dSpawner : MonoBehaviour {
    [SerializeField] private Transform _parentForMoney;
    [SerializeField] private RectTransform _iconPrefab;
    [SerializeField] private RectTransform _targetPoint;
    [SerializeField] private int _poolSize = 20;
    [SerializeField] private float _animationDuration;
    [SerializeField] private List<AnimationCurve> _trajectories;
    [SerializeField] private PairedValue<float> _jumpHeight;

    [SerializeField] private float _countMoneyAfterFlight;
    [Range(0, 1), SerializeField] private float _spawnTimeDiapasone;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private Transform _playerSpawnPoint;
    
    
    private Queue<RectTransform> _moneyPool = new ();

    private PlayerStateManager _playerStateManager;
    private bool _isFlightBefore = false;
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    
    private void Awake() {
        for (int i = 0; i < _poolSize; i++) {
            RectTransform icon = Instantiate(_iconPrefab, _parentForMoney);
            
            icon.DisactiveSelf();
            _moneyPool.Enqueue(icon);
        }
        
    }
    
    public void SpawnOneMoneyNearPlayer() {
        RectTransform icon = GetIconFromPool();
        icon.ActiveSelf();
        icon.position = GetPointAroundPlayer();
        StartCoroutine(MoneyAnimationRoutine(icon));
    }
    
    
    public void SpawnOneMoneyInPoint(Vector3 object3dPosition) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(object3dPosition);
        RectTransform icon = GetIconFromPool();
        ResetRect(icon);
        
        icon.position = screenPos;
        icon.ActiveSelf();
        StartCoroutine(MoneyAnimationRoutine(icon));
    }


    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _isFlightBefore = true;
        }

        if (state == PlayerState.Walking && _isFlightBefore) {
            _isFlightBefore = false;
            StartCoroutine(AfterFlightAnimationRoutine());
        }
    }


    public bool PlayAnimation = false;
    private void OnValidate() {
        if (PlayAnimation) {
            StartCoroutine(AfterFlightAnimationRoutine());
            PlayAnimation =  false;
        }
    }


    private IEnumerator AfterFlightAnimationRoutine() {
        Debug.Log("AfterFlightAnimationRoutine");
        float spawnedMoney = 0;
        while (spawnedMoney < _countMoneyAfterFlight) {
            spawnedMoney++;
            RectTransform icon = GetIconFromPool();
            icon.ActiveSelf();
            icon.position = GetPointAroundPlayer();
            
            
            StartCoroutine(MoneyAnimationRoutine(icon));
            yield return new WaitForSeconds(_spawnTimeDiapasone);
        }
    }






    private IEnumerator MoneyAnimationRoutine(RectTransform icon) {
        float time = 0f;

        Vector3 start = icon.position;
        Vector3 end = _targetPoint.position;

        float duration = _animationDuration;
        float height = Random.Range(_jumpHeight.From, _jumpHeight.To);
        float sign = Random.value < 0.5f ? 1f : -1f;
        while (time < duration) {
            float t = time / duration;

            // Базовая линия
            Vector3 pos = Vector3.Lerp(start, end, t);

            // Парабола
            float yOffset = sign * 4f * height * t * (1f - t);

            pos.y += yOffset;

            icon.position = pos;

            time += Time.deltaTime;
            yield return null;
        }

        // Жёстко фиксируем конец
        icon.position = end;

        ReturnToPool(icon);
    }

    
    private RectTransform GetIconFromPool() {
        if (_moneyPool.Count > 0)
            return _moneyPool.Dequeue();

        // если не хватило — создаём ещё
        return Instantiate(_iconPrefab, _parentForMoney);
    }
    
    private void ReturnToPool(RectTransform icon) {
        ResetRect(icon);
        icon.DisactiveSelf();
        _moneyPool.Enqueue(icon);
    }

    private static void ResetRect(RectTransform icon) {
        icon.localScale = Vector3.one;
        icon.localRotation = Quaternion.identity;
        icon.anchoredPosition = Vector2.zero;
    }
    
    
    private Vector2 GetRandomPointInCircle() {
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Корень — чтобы точки были равномерно, а не кучей в центре
        float r = Mathf.Sqrt(Random.value) * _spawnRadius;

        float x = Mathf.Cos(angle) * r;
        float y = Mathf.Sin(angle) * r;

        return new Vector2(x, y);
    }
    
    private Vector2 GetPointAroundPlayer() {
        Vector2 offset = GetRandomPointInCircle();

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_playerSpawnPoint.transform.position);
        return new Vector2(offset.x + screenPos.x, offset.y + screenPos.y);
    }
    
    
}
