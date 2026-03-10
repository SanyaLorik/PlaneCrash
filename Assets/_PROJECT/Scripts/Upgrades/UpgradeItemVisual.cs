using TMPro;
using UnityEngine;
using Zenject;

public class UpgradeItemVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _levelVisual;
    
    [SerializeField] private TMP_Text _xCurrentVisual;
    [SerializeField] private TMP_Text _xNextVisual;
    
    [SerializeField] private TMP_Text _priceVisual;
    
    [Header("ВИЗУАЛ")]
    [SerializeField] private TMP_Text _titleVisual;
    [SerializeField] private TMP_Text _levelCounterTextInSkills;
    [SerializeField] private TMP_Text _levelInStationsText;
    [SerializeField] private TMP_Text _levelNamingTextInSkills;
    [Header("Партиклы покупки")]
    [SerializeField] private ParticleSystem _particleSystem;
    
    
    
    [Inject] private NumberFormatter _formatter; 
    [Inject] protected LocalizationDataPC _localization;

    public void SetNameText(UpgradeType type) {
        _titleVisual.text = _localization.GetTranslatedName(type, _localization.UpgradeNames);
        _levelNamingTextInSkills.text = _localization.Level;
        _levelInStationsText.text = _localization.Level;
    }

        
    public void UpdateData(int level, float xCurrent, float xNext, double price, string mesure, bool needRound) {
        // Debug.Log($"level {level} xCurrent {xCurrent} xNext {xNext} price {price}");
        _levelVisual.text = level.ToString();
        _priceVisual.text = _formatter.ValuteFormatter(price);
        if (needRound) {
            _xCurrentVisual.text = ((int)xCurrent) + mesure;
            _xNextVisual.text = ((int)xNext) + mesure;
            return;
        }
        _xCurrentVisual.text = xCurrent.ToString("F2")  + mesure;
        _xNextVisual.text = xNext.ToString("F2")  + mesure;
    }

    public void UpdateLevelInLeft(int level) {
        _levelCounterTextInSkills.text = level.ToString();
    }
    
    public void SetRed() {
        _priceVisual.color = Color.red;
    }
    
    public void SetGreen() {
        _priceVisual.color = Color.white;
    }


    public void SkillISBought() {
        _particleSystem.Play();
    }
}
