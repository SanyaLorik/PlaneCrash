using System;
using System.Collections;
using SanyaBeerExtension;
using UnityEngine;

public class MusicBlock  : MonoBehaviour {
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private string _compositionName;
    [SerializeField] private float _brightnessMultiplier;
    [SerializeField] private float _durationToShade;
    [SerializeField] private PairedValue<float> _pitchDiapasone;

    
    private Material _mat;
    private Color _baseColor;
    private Coroutine _shadeRoutine;
    private float _currentEmission = 1f;

    private void Awake() {
        _mat = GetComponent<Renderer>().material;

        // берём базовый цвет прямо из материала
        _baseColor = _mat.color;

        // включаем emission
        _mat.EnableKeyword("_EMISSION");

        // стартовое значение
        SetEmission(1f);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement _)) {
            _audioSource.pitch = UnityEngine.Random.Range(_pitchDiapasone.From, _pitchDiapasone.To);
            _audioSource.Play();
            Debug.Log("Проигрывание " + _compositionName);

            if (_shadeRoutine != null) StopCoroutine(_shadeRoutine);
            _shadeRoutine = StartCoroutine(ChangeEmissionRoutine(_brightnessMultiplier));
        }
    }

    private void OnTriggerExit(Collider other) {
        if (_shadeRoutine != null) StopCoroutine(_shadeRoutine);
        _shadeRoutine = StartCoroutine(ChangeEmissionRoutine(1f));
    }

    private void SetEmission(float intensity) {
        _currentEmission = intensity;
        _mat.SetColor(EmissionColor, _baseColor * intensity);
    }

    private IEnumerator ChangeEmissionRoutine(float targetIntensity) {
        float start = _currentEmission;
        float t = 0f;
        while (t < _durationToShade) {
            t += Time.deltaTime;
            float progress = t / _durationToShade;

            float intensity = Mathf.Lerp(start, targetIntensity, progress);
            SetEmission(intensity);

            yield return null;
        }

        SetEmission(targetIntensity);
    }
    
}
