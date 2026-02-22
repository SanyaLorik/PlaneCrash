using Architecture_M;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Game Localization PC")]
public class LocalizationDataPC : LocalizationData 
{
    [field: Header("Статический текст")]
    [field: SerializeField] public StaticTranslation<string>[] StaticTranslates { get; private set; }

    [Header("Для  больших чисел")]
    public string[] Suffixies = {"", "K", "M", "B", "T"};
    
    
    
    [Header("UI")]
    public string OpenButton;
    public string TakeAPet;
   
    
    [Header("Tasks")]
    public string TaskCompletedNotification;
    public string CollectRewardNotification;
    public string TaskTableTitle;
    public TaskTranslate[]  TaskTranslates;
    
    
    
    [Header("Tasks")]
    public string Meters;
    
    
    
    
    [Header("Обращение к игроку в таблице лидерборда")]
    public string You;
    
    
    
    public TutorTranslate[]  TutorTranslates;
    public UpgradeName[]  UpgradeName;
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
    
    
    public string GetUpgradeName(UpgradeType type) {
        foreach (var upgradeName in UpgradeName) {
            if (type == upgradeName.Type) {
                return upgradeName.Name;
            }
        }
        return string.Empty;
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
public class UpgradeName {
    public UpgradeType Type;
    public string Name;
}
