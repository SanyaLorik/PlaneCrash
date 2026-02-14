using System;
using UnityEngine;
using Architecture_M;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Game Localization PC")]
public class LocalizationDataPC : LocalizationData {
    [Header("Для  больших чисел")]
    public string[] Suffixies = {"", "K", "M", "B", "T"};
    
    
    [Header("Результат полёта")]
    public string DistanceTemplate;
    public string BetMultiplierTemplate;
    public string BetAmountTemplate;
    public string RewardTemplate;
    public string UpgradeMultiplierTemplate;
    public string FlightResultTitle;
    public string FlightComebackButton;
    
    
    [Header("UI")]
    public string PlayerBalanceTemplate;
    public string OpenButton;
   
    
    [Header("Tasks")]
    public string TaskCompletedNotification;
    public string CollectRewardNotification;
    public string TaskTableTitle;
    public TaskTranslate[]  TaskTranslates;
    
    
    
    
    public TutorTranslate[]  TutorTranslates;
    public RangName[]  RangName;
    public string[] BotsPhrases;
    
    
    public string GetPhrase(int id) {
        foreach (var tutorTranslate in TutorTranslates) {
            if (tutorTranslate.Id == id) {
                return tutorTranslate.Phrase;
            }
        }
        return null;
    }
    
    
    public string GetTaskText(TaskType type) {
        foreach (var taskTranslates in TaskTranslates) {
            if (taskTranslates.Type == type) {
                return taskTranslates.TaskText;
            }
        }
        return null;
    }
    
    public string GetRangName(int id) {
        foreach (var rangName in RangName) {
            if (rangName.Id == id) {
                return rangName.Name;
            }
        }
        return null;
    }
    
    
}

[Serializable]
public class TutorTranslate {
    public int Id;
    public string Phrase;
}


[Serializable]
public class TaskTranslate {
    public TaskType Type;
    [TextArea] public string TaskText;
}


[Serializable]
public class RangName {
    public int Id;
    public string Name;
}

