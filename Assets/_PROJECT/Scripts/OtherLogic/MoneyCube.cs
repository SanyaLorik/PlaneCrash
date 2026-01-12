using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class MoneyCube : MonoBehaviour
{
    [SerializeField] private Renderer rend;

    [SerializeField] private float tileWorldSize = 1f;
    [SerializeField] private Vector2 tilingRatio = new Vector2(5f, 11f); // форма пачек
    [SerializeField] private int[] moneyMaterialSlots = {0, 1}; // какие материалы — деньги


    [SerializeField] private Transform anchorPoint; // точка, на которой должен стоять низ куба
    [SerializeField] private Button _recalculateBtn;
    [SerializeField] private float _maxSide = 5f; 
    [SerializeField] private float _baseSide = 1f;
    [SerializeField] private TMP_Text _textCount;
    private MoneyRadiusSpawn _moneyRadiusSpawn;
    private float _baseAmount = 10000f;


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

    public void SetMoneyAmount(float amount, bool updateMiniMoney = true) {
        _textCount.text = amount.ToString("N0"); // шо за NO 
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

        transform.localScale = new Vector3(side, height, side);

        if (anchorPoint != null) {
            Bounds b = rend.bounds;
            float deltaY = anchorPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }

        if (updateMiniMoney) {
            UpdateSpawnRadius();
        }
        UpdateTiling();
    }

    // Прокину здесь чтоб не создавать еще ссылок
    public void UpdateSpawnRadius() {
        _moneyRadiusSpawn.SpawnMoney();
    }

}