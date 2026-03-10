using System;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class TaskVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _rewardMoneyText;
    [SerializeField] private TMP_Text _taskText;
    [SerializeField] private GameObject _completeImg;
    [SerializeField] private RectTransform _parentRectTransform;
    [SerializeField] private RectTransform _progressRectTransform;
    [field: SerializeField] public TaskType TaskType { get; private set; }
    public bool TaskIsComplete { get; private set; }
    
    private string _taskLocalizationText;
    
    
    [Inject] private NumberFormatter _formatter;
    [Inject] private LocalizationDataPC _localization;
    [Inject] private RectTransformHelper _fillAmounthMover;

    
    public void SetTaskLocalizationText() {
        _taskLocalizationText = _localization.GetTranslatedName(TaskType,  _localization.TaskTranslates);
    }

    public void SetTaskVisual(float rewardMoney, float fullValue, float playerValue) {
        _rewardMoneyText.text = _formatter.ValuteFormatter(rewardMoney);
        _taskText.text = string.Format(_taskLocalizationText, _formatter.ValuteFormatter(fullValue));
        _completeImg.DisactiveSelf();
        TaskIsComplete = false;
        UpdateTaskScoreVisual(playerValue,  fullValue);
    }

    public void UpdateTaskScoreVisual(float currentValue, float fullValue) {
        float percent = currentValue / fullValue;
        _fillAmounthMover.SetFillAmount(_progressRectTransform, _parentRectTransform, percent);
    }

    
    public void SetTaskCompleteVisual() {
        _completeImg.ActiveSelf();
        _fillAmounthMover.SetFillAmount(_progressRectTransform, _parentRectTransform, 1);
        TaskIsComplete = true;
    }

    
}
