using Architecture_M;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Game Localization PC")]
public class LocalizationDataPC : LocalizationData 
{
    [field: Header("Статический текст")]
    [field: SerializeField] public StaticTranslation<string>[] StaticTranslates { get; private set; }

    [Header("Для  больших чисел")]
    public string[] Suffixies = {"", "K", "M", "B", "T"};
    
    
    
    [Header("UI")]
    public string IsWeared;
    public string TakeAPet;
    public string Receive;
    public string Level;
    public string Bet;
    public string Reward;
   
    
    
    [Header("Ед. измерения")]
    public string Meters;
    public string Pieces;
    public string Xs;
    public string MagnetPower;
    
    
    
    [Header("Tasks")]
    public string TaskCompletedNotification;
    public string CollectRewardTaskNotification;
    public string TaskTableTitle;
    

    
    [Header("Обращение к игроку в таблице лидерборда")]
    public string You;
    
    
    public string[] BotsPhrases;
    [Header("Словари")]
    public TutorTranslate[]  TutorTranslates;
    public UpgradeName[]  UpgradeNames;
    public SkinName[]  SkinNameTranslates;
    public RangName[]  RangNameTranslates;
    public EggStationName[] EggStationNameTranslates;
    public TaskTranslate[]  TaskTranslates;
    
    
    public string GetTranslatedName<TId, TItem>(TId id, IEnumerable<TItem> arr)
        where TItem : IIdName<TId>
    {
        foreach (var item in arr)
        {
            if (EqualityComparer<TId>.Default.Equals(item.Id, id))
                return item.Name;
        }
        return null;
    }
}


[Serializable]
public class TutorTranslate : IIdName<string> {
    [field: SerializeField] public string Id { get; set; }
    [TextArea] [SerializeField] private string name;
    public string Name { get => name; set => name = value; }
}


[Serializable]
public class TaskTranslate : IIdName<TaskType> {
    [field: SerializeField] public TaskType Id { get; set; }
    [TextArea] [SerializeField] private string name;
    public string Name { get => name; set => name = value; }
}


[Serializable]
public class UpgradeName : IIdName<UpgradeType> {
    [field: SerializeField] public UpgradeType Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
}


[Serializable]
public class SkinName : IIdName<string> {
    [field: SerializeField] public string Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
}


[Serializable]
public class RangName : IIdName<int> {
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
}


[Serializable]
public class EggStationName : IIdName<string> {
    [field: SerializeField] public string Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
}

public interface IIdName<T> {
    T Id { get; }
    string Name { get; }
}

