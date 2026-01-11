using System;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class MoneyCube : MonoBehaviour
{
    [SerializeField] private Renderer rend;

    [SerializeField] private float tileWorldSize = 1f;
    [SerializeField] private Vector2 tilingRatio = new Vector2(5f, 11f); // форма пачек
    [SerializeField] private int[] moneyMaterialSlots = {0, 1}; // какие материалы — деньги

    [SerializeField] private float _amount = 1000;

    [SerializeField] private Transform anchorPoint; // точка, на которой должен стоять низ куба
    [SerializeField] private Button _recalculateBtn;
    [SerializeField] float _maxSide = 5f; 
    [SerializeField] float _baseSide = 1f;
    [SerializeField] float _baseAmount = 1000f;
    
    
    private void Start() {
        _recalculateBtn.onClick.AddListener(() => SetMoneyAmount(_amount));
    }

    private void Update() {
        SetMoneyAmount(_amount);

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

    private void SetMoneyAmount(float amount) {
        _amount = amount;

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

        // фиксируем дно на anchorPoint через bounds
        if (anchorPoint != null)
        {
            Bounds b = rend.bounds;
            float deltaY = anchorPoint.position.y - b.min.y;
            transform.position += new Vector3(0, deltaY, 0);
        }

        UpdateTiling();
    }

}