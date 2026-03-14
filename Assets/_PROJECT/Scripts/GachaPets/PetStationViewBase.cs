using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;


[Serializable]
public class EntityView {
    public Image Icon;
    public TMP_Text Percentage;
}



public abstract class PetStationViewBase : MonoBehaviour {

    [SerializeField] protected string _statonNameId;
    [SerializeField] protected TextMeshProUGUI _statonNameText;
    [SerializeField] protected DelayedTrigger _customTrigger;
    [SerializeField] protected PetStationConfig _config;
    [SerializeField] protected EntityView[] _views;

    
    [Inject] protected NumberFormatter _formatter;
    [Inject] protected PlayerBank _bank;
    [Inject] protected PetOpenView _petOpenView;
    [Inject] protected PetsManager _petsManager;
    [Inject] protected LocalizationDataPC _localization;

    
    protected bool _allowToUse;
    protected int _showedReward = 0;
    private float _divider;

    public void SetAllowUse(bool use) {
        _allowToUse = use;
    }

    
    protected void Start() {
        _divider = ChanceSum(_config.Pets);
        Initialize();
        StartInit();
        _statonNameText.text = _localization.GetTranslatedName(_statonNameId, _localization.EggStationNameTranslates);
    }


    private void OnTriggerEnter(Collider collider) {
        if (!_allowToUse) return;
        if (!collider.TryGetComponent(out PlayerMovement _)) return;
        if (AllowToGetPet) {
            _customTrigger.DelayedTriggerAction(AddPet);
        }
    }

    protected abstract void AddPet();
    protected virtual void StartInit(){}

    protected void OnTriggerExit(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement _)) return;
        _customTrigger.CancelTriggerAction();
        Debug.Log("Операция по получению пета отменена");
    }
    
    protected void Initialize() {
        for (int i = 0; i < _config.Pets.Length; i++) {
            _views[i].Icon.sprite = _config.Pets[i].PetItemConfig.Sprite;
            _views[i].Percentage.text = $"{_config.Pets[i].Chance / _divider * 100f:#0}  %";
        }
    }

    protected bool AllowToGetPet = true;

    protected PetChance GetRandomPet(PetStationConfig config) {
        float random = Random.Range(0f, _divider);
        float cumulative = 0f;

        foreach (var pet in config.Pets) {
            cumulative += pet.Chance;
            if (random <= cumulative)
                return pet;
        }
        
        return config.Pets[^1];
    }



    protected float ChanceSum(PetChance[] pets) {
        float sum = 0f;
        foreach (var petChance in pets) {
            sum+= petChance.Chance;
        }
        return sum;
    }
}