using SanyaBeerExtension;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class LocationModule: MonoBehaviour {
    [Header("Начало и конец буста")]
    [field: SerializeField] public Transform Start { get; private set; }
    [field: SerializeField] public Transform End { get; private set; }
    
    [Header("Обьекты")]
    [SerializeField] private List<LocationZone> _zones;
    [SerializeField] private MaterialApplier[] _materialAppliers;

    public void Init(DiContainer diContainer)
    {
        foreach (var zone in _zones)
            diContainer.Inject(zone);
    } 
    
    public void GenerateProps() 
    {
        foreach (var zone in _zones)
            zone.GenerateProps();

        _materialAppliers.ForEach(i => i.ApplyRandomMaterial());
    }

    public void HideObjects() 
    {
        foreach (var zone in _zones) 
            zone.HideObjects();
    }
}
