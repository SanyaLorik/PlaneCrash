using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class MoneyMultiplyZone : MonoBehaviour {
    
    [SerializeField] private TMP_Text _xMultiplySignText; // на самой табличке
    [SerializeField] private float _xMultiplyValue;
    
    private ZoneManager _zoneManager;
        
    [Inject]
    public void Init(ZoneManager zoneManager) {
        _zoneManager = zoneManager;
    }
    
    private void Start() {
        _xMultiplySignText.text = "x" + _xMultiplyValue;
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            Debug.Log("ChangeMultiplyer MoneyMultiplyZone");
            _zoneManager.ChangeMultiplier(_xMultiplyValue);
        }    
    }


}
