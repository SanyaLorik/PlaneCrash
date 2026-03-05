using System;
using UnityEngine;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

enum NickType
{
    Simple,
    NameNumber,
    NameLast,
    PrefixName,
    NameSuffix,
    TwoWords,
    Leet,
    GamerTag
}

public class NicknameRandomizer : IInitializable
{
    private readonly NicknameSettings _settings;

    private string[] _ruMaleFirst;
    private string[] _ruFemaleFirst;
    private string[] _ruMaleLast;
    private string[] _ruFemaleLast;
    private string[] _enMaleFirst;
    private string[] _enFemaleFirst;
    private string[] _enLast;

    private string[] _numberCombines;

    private string[] _prefix =
    {
        "Ultra","Mega","Dark","Real","Top","Pro","xX"
    };

    private string[] _suffix =
    {
        "Pro","YT","TV","GG","LOL","EZ"
    };

    private string[] _memWords =
    {
        "Pelmen","Borsh","Kefir","Bread","Ninja","Dragon","Slav"
    };

    private string[] _linkWords =
    {
        "The","Of","King","Lord","Mr"
    };

    public NicknameRandomizer(NicknameSettings settings)
    {
        _settings = settings;
    }

    public void Initialize()
    {
        GetFilesStrings();
    }

    

    public string GetRandomName()
    {
        bool rus = Random.value < _settings.ChanceToRusName;
        bool male = Random.value < _settings.ChanceToMale;

        string first;
        string last;

        if (rus && male)
        {
            first = GetRandom(_ruMaleFirst);
            last = GetRandom(_ruMaleLast);
        }
        else if (rus)
        {
            first = GetRandom(_ruFemaleFirst);
            last = GetRandom(_ruFemaleLast);
        }
        else if (male)
        {
            first = GetRandom(_enMaleFirst);
            last = GetRandom(_enLast);
        }
        else
        {
            first = GetRandom(_enFemaleFirst);
            last = GetRandom(_enLast);
        }

        return BuildNick(first, last);
    }

    private string BuildNick(string first, string last)
    {
        NickType type = GetWeightedNickType();

        string nick = type switch
        {
            NickType.Simple => first,

            NickType.NameNumber => first + GetNumber(),

            NickType.NameLast => first + "_" + last,

            NickType.PrefixName => GetRandom(_prefix) + first,

            NickType.NameSuffix => first + GetRandom(_suffix),

            NickType.TwoWords => first + GetRandom(_memWords),

            NickType.Leet => ApplyLeet(first),

            NickType.GamerTag => BuildGamerTag(first),

            _ => first
        };

        nick = MutateCase(nick);

        return Trim(nick);
    }
    
    
    private string BuildGamerTag(string first)
    {
        int variant = Random.Range(0, 6);

        string num = GetNumber();
        string mem = GetRandom(_memWords);
        string link = GetRandom(_linkWords);
        string prefix = GetRandom(_prefix);
        string suffix = GetRandom(_suffix);

        return variant switch
        {
            0 => "xX_" + first + "_" + num + "_Xx",

            1 => prefix + first + num,

            2 => first + "_" + mem,

            3 => first + link + mem,

            4 => link + "_" + first + "_" + num,

            5 => first + "_" + suffix,

            _ => first
        };
    }
    
    private NickType GetWeightedNickType()
    {
        // веса (чем больше, тем чаще)
        float[] weights = new float[]
        {
            1f, // Simple
            1f, // NameNumber
            1f, // NameLast
            0.5f, // PrefixName
            0.5f, // NameSuffix
            0.8f, // TwoWords
            0.5f, // Leet
            0.3f  // GamerTag (xX_…Xx)
        };

        float total = 0f;
        foreach (var w in weights) total += w;

        float r = Random.value * total;
        for (int i = 0; i < weights.Length; i++)
        {
            if (r < weights[i])
                return (NickType)i;
            r -= weights[i];
        }

        return NickType.Simple;
    }
    
    
    

    private string GetRandom(string[] arr)
    {
        return arr[Random.Range(0, arr.Length)];
    }

    private string GetNumber()
    {
        if (Random.value > _settings.ChanceToHaveNumber)
            return "";

        return _numberCombines[Random.Range(0, _numberCombines.Length)];
    }

    private string ApplyLeet(string name)
    {
        return name
            .Replace("a", "4")
            .Replace("e", "3")
            .Replace("o", "0")
            .Replace("i", "1");
    }

    private string MutateCase(string s)
    {
        float r = Random.value;

        if (r < 0.2f) return s.ToUpper();
        if (r < 0.4f) return s.ToLower();
        if (r < 0.6f)
        {
            char[] c = s.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (Random.value < 0.5f)
                    c[i] = char.ToUpper(c[i]);
            }
            return new string(c);
        }

        return s;
    }

    private string Trim(string s)
    {
        if (s.Length <= _settings.MaxCharsName) return s;
        return s.Substring(0, _settings.MaxCharsName);
    }

    private string[] LoadFile(string path)
    {
        TextAsset asset = Resources.Load<TextAsset>(path);

        return asset.text
            .Replace("\r", "")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }

    private void GetFilesStrings() {
        _ruMaleFirst = LoadFile("PlayerNames/ru_male_first");
        _ruMaleLast = LoadFile("PlayerNames/ru_male_last");
        _ruFemaleFirst = LoadFile("PlayerNames/ru_female_first");
        _ruFemaleLast = LoadFile("PlayerNames/ru_female_last");
        _enMaleFirst = LoadFile("PlayerNames/en_male_first");
        _enFemaleFirst = LoadFile("PlayerNames/en_female_first");
        _enLast = LoadFile("PlayerNames/en_last");
        _numberCombines = LoadFile("PlayerNames/number_combine");
    }

}