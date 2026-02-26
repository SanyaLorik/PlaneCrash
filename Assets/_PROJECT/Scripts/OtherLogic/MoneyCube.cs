using System;
using Architecture_M;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Zenject;

[Serializable]
public enum MoneyCubeType {
    PlayerBank,
    Bet
}


public class MoneyCube : MonoBehaviour {
    [SerializeField] private MoneyCubeType _moneyCubeType;
    [SerializeField, HideIf(nameof(IsBetCube))] private TMP_Text _cubeText;
    [SerializeField] private Transform _bottomPoint; // точка низа куба
    [field: SerializeField, HideIf(nameof(IsBetCube))] public Transform UpPoint { get; private set; } // точка уэрха куба

    [Header("Настройка размеров ")]
    [SerializeField] private float _scaleDivider = 2f; 
    
    

    [Header("Настройка размеров ")]
    [SerializeField] private float _maxSide = 5f; 
    private float _baseSide = 1f;
    [SerializeField] private float _baseAmount = 100000;
    
    
    [Header("Настройка всего тайлинга ")]
    [SerializeField] private Renderer _rend;
    [SerializeField] private float _tileWorldSize = 1f;
    [SerializeField] private Vector2 _tilingRatio = new Vector2(5f, 11f); // тайлинг пачек
    [Header("Слоты кроме верхнего")]
    [SerializeField] private int[] _moneyMaterialSlots = {0, 1}; // какие материалы — деньги
    
    [Header("Настройка верхнего тайлинга")]
    [SerializeField] private Vector2 _upSideTiling;
    [SerializeField] private int _upSideMaterialSlot;
   
    private bool IsBetCube => _moneyCubeType == MoneyCubeType.Bet;
   

    [Inject] private NumberFormatter _formatter; 
    [Inject] private LocalizationDataPC _localization; 
    [Inject] IGameSave<GameSavePC> _gameSave;


    private void Start() {
        SetMoneyAmount(_gameSave.GetSave.Money);
    }

    public float GetCubeHeight(float amount) {
        Transform realTransform = transform; // сохраним временно
        
        Vector3 newScale = NewScale(amount);
        transform.localScale = newScale;

        if (_bottomPoint != null) {
            Bounds b = _rend.bounds;
            float deltaY = _bottomPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }
        float height = _rend.bounds.max.y;
        transform.position = realTransform.position;
        transform.localScale = realTransform.localScale;
        return height;
    }
    
    
    public void SetMoneyAmount(float amount) {
        
        
        Vector3 newScale = NewScale(amount);
        
        transform.localScale = newScale;

        if (_bottomPoint != null) {
            Bounds b = _rend.bounds;
            float deltaY = _bottomPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }

        UpdateTiling();
        SetCubeHeighVisual();
    }
    
    
    private void UpdateTiling() {
        Vector3 size = transform.localScale;

        float baseX = size.x / _tileWorldSize;
        float baseY = size.y / _tileWorldSize;

        Vector2 finalTiling = new Vector2(
            Mathf.Round(baseX * _tilingRatio.x),
            Mathf.Round(baseY * _tilingRatio.y)
        );

        Material[] mats = _rend.materials;

        foreach (int id in _moneyMaterialSlots) {
            Vector2 tiling = finalTiling;
            tiling.x = Mathf.Min(tiling.x, _upSideTiling.x);
            
            if (id == _upSideMaterialSlot) {
                tiling.y = Mathf.Min(tiling.y, _upSideTiling.y);
            }

            mats[id].mainTextureScale = tiling;
        }

        _rend.materials = mats;
    }

    
    
    

    private void SetCubeHeighVisual() {
        if (!IsBetCube) {
            _cubeText.text = _formatter.ValuteFormatter(transform.localScale.y / _scaleDivider);
        }
    }

    private Vector3 NewScale(float amount) {
        float linearSide = _baseSide * (amount / _baseAmount);

        float side;
        float height;

        if (linearSide <= _maxSide) {
            // нормальный рост во все стороны
            side = linearSide;
            height = side * 0.5f;
        }
        else {
            // ширина зафиксирована, деньги идут в высоту
            side = _maxSide;

            float extra = linearSide / _maxSide; 
            height = (_maxSide * 0.5f) * extra;
        }

        Vector3 newScale = new Vector3(side, height, side);
        return newScale;
    }


}