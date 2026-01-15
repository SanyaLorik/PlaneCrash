using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;

public class WaterMoving : MonoBehaviour {
    [SerializeField] private PairedValue _yZones;
    [SerializeField] private PairedValue _xZones;
    [SerializeField] private float _xTiling;
    [SerializeField] private Renderer _waterRenderer;
    [SerializeField] private float _changeDuration;

    private Material _mat;
    public bool stopWaterMovement;
    private CancellationTokenSource _waterCTS;
    
    private void Start() {
        _mat = GetComponent<Renderer>().material;
        _fromY = _yZones.From;
        _fromX = _xZones.From;
        _toY = _yZones.To;
        _toX = _xZones.To;
        _waterCTS = new CancellationTokenSource();
        StartChangeTiling(_waterCTS.Token).Forget();

    }

    private float _timer;
    private float _fromY;
    private float _fromX;
    private float _toY;
    private float _toX;
    private float _progress;
    
    private async UniTaskVoid StartChangeTiling(CancellationToken token) {
        while (!stopWaterMovement) {
            _progress = _timer / _changeDuration;
            Vector2 tiling = new Vector2(Mathf.Lerp(_fromX, _toX, _progress), Mathf.Lerp(_fromY, _toY, _progress));
            
            _mat.mainTextureScale = tiling;
            
            _timer+= Time.deltaTime;
            if (_timer >= _changeDuration) {
                SwapNumbers();
                _timer = 0;
            }
            await UniTask.Yield(token);
        }
        
    }

    private void SwapNumbers() {
        (_fromY, _toY) = (_toY, _fromY);
        (_fromX, _toX) = (_toX, _fromX);
    }

    private void OnDestroy() {
        _waterCTS?.Cancel();
        _waterCTS?.Dispose();
    }
}
