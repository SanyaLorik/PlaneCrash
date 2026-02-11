using System;
using UnityEngine;
using Architecture_M;

[CreateAssetMenu(menuName = "Architecture_M/Localization/Game Localization PC")]
public class LocalizationDataPC : LocalizationData {


    public string lol;
    public TutorTranslate[]  TutorTranslates;
    
    public string GetPhrase(int id) {
        foreach (var tutorTranslate in TutorTranslates) {
            if (tutorTranslate.Id == id) {
                return tutorTranslate.Phrase;
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


