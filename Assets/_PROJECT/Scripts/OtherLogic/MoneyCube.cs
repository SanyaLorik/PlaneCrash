using System;
using _PROJECT.Scripts.Extensions_Helpers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCube : MonoBehaviour {
    [SerializeField] private Renderer rend;

    [SerializeField] private float tileWorldSize = 1f;
    [SerializeField] private Vector2 tilingRatio = new Vector2(5f, 11f); // форма пачек
    [SerializeField] private int[] moneyMaterialSlots = {0, 1}; // какие материалы — деньги


    [SerializeField] private Transform _bottomPoint; // точка, на которой должен стоять низ куба
    [SerializeField] private float _maxSide = 7f; 
    [SerializeField] private float _baseSide = 1f;
    [SerializeField] private TMP_Text _textCount;
    [SerializeField] private float _baseAmount = 10000f;

    private MoneyRadiusSpawn _moneyRadiusSpawn;

    private void Awake() {
        _moneyRadiusSpawn = GetComponent<MoneyRadiusSpawn>();
    }

    private void UpdateTiling() {
        Vector3 size = transform.localScale;

        float baseX = size.x / tileWorldSize;
        float baseZ = size.z / tileWorldSize;

        Vector2 finalTiling = new Vector2(
            Mathf.Round(baseX * tilingRatio.x),
            Mathf.Round(baseZ * tilingRatio.y)
        );

        Material[] mats = rend.materials;

        foreach (int id in moneyMaterialSlots)
            mats[id].mainTextureScale = finalTiling;

        rend.materials = mats;
    }

    public float GetCubeHeight(float amount) {
        Transform realTransform = transform; // сохраним временно
        
        Vector3 newScale = NewScale(amount);
        transform.localScale = newScale;

        if (_bottomPoint != null) {
            Bounds b = rend.bounds;
            float deltaY = _bottomPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }
        float height = rend.bounds.max.y;
        transform.position = realTransform.position;
        transform.localScale = realTransform.localScale;
        return height;
    }
    
    private Tween _tween;
    public void SetMoneyAmount(float amount, bool updateMiniMoney = true) {
        _textCount.text = GameHelper.ValuteFormatter(amount);
        
        
        Vector3 newScale = NewScale(amount);
        
        transform.localScale = newScale;

        if (_bottomPoint != null) {
            Bounds b = rend.bounds;
            float deltaY = _bottomPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }
        
        
        
        

        if (updateMiniMoney) {
            UpdateSpawnRadius();
        }
        UpdateTiling();
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


    // Прокину здесь чтоб не создавать еще ссылок
    private void UpdateSpawnRadius() {
        _moneyRadiusSpawn.SpawnMoney();
    }

}