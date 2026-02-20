using Architecture_M;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class StaticTranslater : MonoBehaviour
{
    [SerializeField] private StaticTranslation<TextMeshProUGUI>[] _texts;

    [Inject] private LocalizationDataPC _localization;
    [Inject] private IInputActivity a;

    private void Start()
    {
        foreach (var translation in _localization.StaticTranslates)
        {
            StaticTranslation<TextMeshProUGUI>? text = _texts.FirstOrDefault(i => i.Id == translation.Id);
            if (text.HasValue == false)
                Debug.LogError($"Нет перевода для {translation.Id}");

            text.Value.Data.text = translation.Data;
        }
    }
}