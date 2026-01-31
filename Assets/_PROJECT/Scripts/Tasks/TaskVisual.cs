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
    [SerializeField] private string _taskString;
    public TaskType TaskType;
    public bool TaskIsComplete;


    public void SetTaskVisual(float rewardMoney, float fullValue, float playerValue) {
        _rewardMoneyText.text = TasksManager.FormatBigNumber(rewardMoney);
        _taskText.text = string.Format(_taskString, TasksManager.FormatBigNumber(fullValue));
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
