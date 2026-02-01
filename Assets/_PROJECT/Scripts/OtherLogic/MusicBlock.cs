using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MusicBlock  : MonoBehaviour {
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private string _compositionName;
    [SerializeField] private Color _tileColor;
    [SerializeField] private float _brightnessMultiplier;
    [SerializeField] private float _durationToShade;

    private Material _mat;
    private Color _brightnessColor;
    
    
    private void Awake() {
        _mat = GetComponent<Renderer>().material;
        _mat.color = _tileColor;
        _brightnessColor = AdjustBrightness();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement _)) {
            _audioSource.Play();
            Debug.Log("Проигрывание " + _compositionName);
            if (_shadeRoutine != null) {
                StopCoroutine(_shadeRoutine);
            }
            _shadeRoutine = StartCoroutine(ChangeColorRoutine(_brightnessColor));
        }
    }


    private void OnTriggerExit(Collider other) {
        if (_shadeRoutine != null) {
            StopCoroutine(_shadeRoutine);
        }
        _shadeRoutine = StartCoroutine(ChangeColorRoutine(_tileColor));
    }

    private Coroutine _shadeRoutine;
    private IEnumerator ChangeColorRoutine(Color target) {
        Color start = _mat.color;

        float t = 0f;

        while (t < _durationToShade) {
            t += Time.deltaTime;
            float progress = t / _durationToShade;
            _mat.color = Color.Lerp(start, target, progress);
            yield return null;
        }

        _mat.color = target;
    }
    private Color AdjustBrightness() {
        Color.RGBToHSV(_tileColor, out float h, out float s, out float v);

        v *= _brightnessMultiplier;
        v = Mathf.Clamp01(v);

        return Color.HSVToRGB(h, s, v);
    }
    
    
}
