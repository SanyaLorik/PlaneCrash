using System;
using UnityEngine;
using Architecture_M;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Game Localization PC")]
public class LocalizationDataPC : LocalizationData {


    public string lol;
    public TutorTranslate[]  tutorTranslates;
    
}

[Serializable]
public class TutorTranslate {
    public int Id;
    public string Phrase;
}
