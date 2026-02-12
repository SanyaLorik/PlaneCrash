using System;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TaskVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _rewardMoneyText;
    [SerializeField] private TMP_Text _taskText;
    [SerializeField] private Image _completeImg;
    [SerializeField] private Image _taskBar;
    [field: SerializeField] public TaskType TaskType { get; private set; }
    public bool TaskIsComplete { get; private set; }
    
    private string _taskLocalizationText;
    
    
    [Inject] private NumberFormatter _formatter;
    [Inject] private LocalizationDataPC _localization;
    
    public void SetTaskLocalizationText() {
        _taskLocalizationText = _localization.GetTaskText(TaskType);
    }

    public void SetTaskVisual(float rewardMoney, float fullValue, float playerValue) {
        _rewardMoneyText.text = _formatter.ValuteFormatter(rewardMoney);
        _taskText.text = string.Format(_taskLocalizationText, _formatter.ValuteFormatter(fullValue));
        _completeImg.DisactiveSelf();
        TaskIsComplete = false;
        UpdateTaskScoreVisual(playerValue,  fullValue);
    }

    public void UpdateTaskScoreVisual(float currentValue, float fullValue) {
        _taskBar.fillAmount = currentValue / fullValue;
    }

    public void SetTaskCompleteVisual() {
        _completeImg.ActiveSelf();
        _taskBar.fillAmount = 1f;
        TaskIsComplete = true;
    }
    

    
}
