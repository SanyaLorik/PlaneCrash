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
        float percent = currentValue / fullValue;
        Debug.Log(percent);
        SetFillAmount(percent);
    }
    
    
    private void SetFillAmount(float percent) {
        _progressRectTransform.offsetMax = new Vector2(GetXPoseByPercent(percent), 0);
        Debug.Log(GetXPoseByPercent(percent));
        
    }

    private float GetXPoseByPercent(float percent) {
        float _xEnd = _parentRectTransform.rect.width;
        if (_xEnd < 0) {
            Debug.LogError("_xEnd < 0, Force UPDATE" );
            Canvas.ForceUpdateCanvases();
            _xEnd = _parentRectTransform.rect.width;
            Debug.LogError("_xEnd = " + _xEnd);
        }
        return -_xEnd * (1f - percent);
    }

    public void SetTaskCompleteVisual() {
        _completeImg.ActiveSelf();
        SetFillAmount(1);
        TaskIsComplete = true;
    }

    
}
