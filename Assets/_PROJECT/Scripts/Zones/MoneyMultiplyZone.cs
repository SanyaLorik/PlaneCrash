using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MoneyMultiplyZone : MonoBehaviour {
    
    [SerializeField] private BetAccumulation _accumulationZone;
    [SerializeField] private TMP_Text _xMultiplySignText; // на самой табличке
    [SerializeField] private float _xMultiplyValue;

    
    private void Start() {
        _xMultiplySignText.text = "x" + _xMultiplyValue;
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerBank bank)) {
            _accumulationZone.StopAccumulate();
            BetVisual.Instantiate.XMultiplyVisual.text = "Множитель: x" + _xMultiplyValue;
            BetVisual.Instantiate.RewardVisual.text = "Выигрышь: " + (_accumulationZone.CurrentMoneyAccum * _xMultiplyValue).ToString("F2");
        }    
    }


}
