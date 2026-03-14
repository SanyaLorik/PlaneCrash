using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PetStationsManager : MonoBehaviour  {
    [SerializeField] private List<PetStationViewBase> _petStations;
    [SerializeField] private PetStationViewPurchase _tutorialStation;
    [SerializeField] private int _indexToAllowPetOpen;


    [Inject] private TutorialCompiller _tutorialCompiller;

    private void OnEnable() {
        if(_tutorialCompiller.TutorialPassed) return;
        _tutorialCompiller.TutorialStepChanged += CheckPetsStationsStep;
        _tutorialCompiller.TutorialIsOver += OnTutorialIsOver;
    }

    private void OnTutorialIsOver() {
        InitStationsUse(true);
    }

    private void CheckPetsStationsStep(int tutorialStep) {
        if(tutorialStep != _indexToAllowPetOpen) return;
        _tutorialStation.SetZeroPrice();
    }

    private void Start() {
        InitStationsUse(_tutorialCompiller.TutorialPassed);
    }

    private void InitStationsUse(bool state) {
        _petStations.ForEach(s => s.SetAllowUse(state));
    }
}
