using System.Collections.Generic;
using UnityEditor.Rendering;using UnityEngine;
using Zenject;


public class LocationModule: MonoBehaviour {
    [Header("Начало и конец буста")]
    [field: SerializeField] public Transform Start { get; private set; }
    [field: SerializeField] public Transform End { get; private set; }
    
    [Header("Обьекты")]
    [SerializeField] private List<LocationZone> _zones;


    public void Init(DiContainer diContainer) {
        foreach (var zone in _zones) {
            Debug.Log(diContainer);
            diContainer.Inject(zone);
        }

    } 
    
    public void GenerateProps() {
        foreach (var zone in _zones) {
            zone.GenerateProps();
        }
    }
}
