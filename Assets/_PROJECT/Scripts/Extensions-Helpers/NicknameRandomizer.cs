using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class NicknameRandomizer: IInitializable {

    private readonly NicknameSettings _settings;
    private string[] _ruMaleFirst;
    private string[] _ruFemaleFirst;
    private string[] _ruMaleLast;
    private string[] _ruFemaleLast;
    private string[] _enMaleFirst;
    private string[] _enFemaleFirst;
    private string[] _enLast;

    public NicknameRandomizer(NicknameSettings settings) {
        _settings = settings;
    }
    
    public void Initialize() {
        GetFilesStrings();
    }
    
    public string GetRandomName() {
        bool rusPlayer = Random.value < _settings.ChanceToRusName;
        bool male = Random.value < _settings.ChanceToMale;
        
        string name;
        if (rusPlayer &&  male) {
            name = GetRandomLocalName(_ruMaleFirst, _ruMaleLast);
        }
        else if (rusPlayer && !male) {
            name = GetRandomLocalName(_ruFemaleFirst, _ruFemaleLast);
        }
        else if (!rusPlayer && male) {
            name = GetRandomLocalName(_enMaleFirst, _enLast);
        }
        else {
            name = GetRandomLocalName(_enFemaleFirst, _enLast);
        }
        
        
        if (name.Length > _settings.MaxCharsName) {
            name = AddNumbersInName(name);
        }
        return name;
    }
    
    private string GetRandomLocalName(string[] firstName, string[] lastName) {
        return firstName[Random.Range(0, firstName.Length)] + 
               " " +  
               lastName[Random.Range(0, lastName.Length)];
    }
    
    
    private void GetFilesStrings() {
        _ruMaleFirst = LoadFile("PlayerNames/ru_male_first");
        _ruMaleLast = LoadFile("PlayerNames/ru_male_last");
        _ruFemaleFirst = LoadFile("PlayerNames/ru_female_first");
        _ruFemaleLast = LoadFile("PlayerNames/ru_female_last");
        _enMaleFirst = LoadFile("PlayerNames/en_male_first");
        _enFemaleFirst = LoadFile("PlayerNames/en_female_first");
        _enLast = LoadFile("PlayerNames/en_last");
    }
    
    private string[] LoadFile(string path) {
        TextAsset asset = Resources.Load<TextAsset>(path);

        return asset.text
            .Replace("\r", "")          // убираем carriage return
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }

    private string AddNumbersInName(string name) {
        
        string[] names = name.Split(' ');
        string newName = names[0];
        if (Random.value > 0.5f) newName = names[1];
        if (Random.value > _settings.ChanceToHaveNumber) return newName;
        
        if (newName.Length > _settings.MaxCharsName) {
            return newName.Substring(0, _settings.MaxCharsName);
        }

        // Свободное пространство под символы
        int freePlaces = _settings.MaxCharsName - newName.Length;
        freePlaces = Mathf.Clamp(freePlaces, 0, _settings.MaxDigitCount);
        

        // Добавляем случайные цифры
        string suffix = RandomDigits(0, freePlaces);

        return newName + suffix;
    }

    private string RandomDigits(int minLength, int maxLength) {
        int length = Random.Range(minLength, maxLength + 1);
        string result = "";
        for (int i = 0; i < length; i++)
            result += Random.Range(0, 10).ToString();
        return result;
    }


}