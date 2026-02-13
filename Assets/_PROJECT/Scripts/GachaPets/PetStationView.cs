using System;
using UnityEngine;

public class PetStationView : MonoBehaviour {
    [SerializeField] private DelayedTrigger _customTrigger;
    [SerializeField] private PetStationConfig _petStationConfig;

    
    
    private void OnTriggerEnter(Collider collider) {
        if(collider.TryGetComponent(out PlayerMovement _)){
            _customTrigger.DelayedTriggerAction(() => Debug.Log("Пет куплен"));
        }
    }
    
    private void OnTriggerExit(Collider collider) {
        if(collider.TryGetComponent(out PlayerMovement _)){
            _customTrigger.CancelTriggerAction();
            Debug.Log("Покупка пета отменена");
        }
    }


    private void Initialize() {
        
    }
}