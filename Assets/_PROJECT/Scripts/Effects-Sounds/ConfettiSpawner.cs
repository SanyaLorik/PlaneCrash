using System;
using System.Collections;
using System.Collections.Generic;
using SanyaBeerExtension;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConfettiSpawner : MonoBehaviour {
    [SerializeField] private ParticleSystem confettiSystem;
    [SerializeField] private Sprite[] confettiSprites; // массив белых форм

    [SerializeField] private int _countConfetti;
    [SerializeField] private float _duration = 0.5f;



    private void Awake() {
        CacheParams();
    }


    public void SpawnConfetti() {
        StartCoroutine(SpawnConfettiRoutine(_duration));
    }

    
    private List<ParticleSystem.EmitParams> _cachedParams;

    private void CacheParams() {
        _cachedParams = new List<ParticleSystem.EmitParams>(_countConfetti);

        for (int i = 0; i < _countConfetti; i++) {
            ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();

            p.position = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(0f, 5f),
                Random.Range(-5f, 5f)
            );

            // Раскрашиваем белый спрайт
            p.startColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

            _cachedParams.Add(p);
        }
    }
    
    private IEnumerator SpawnConfettiRoutine(float duration) {
        int total = _cachedParams.Count;
        float elapsed = 0f;
        int index = 0;

        while (index < total) {
            float fraction = Time.deltaTime / duration;
            int toEmit = Mathf.CeilToInt(total * fraction);

            for (int i = 0; i < toEmit && index < total; i++, index++) {
                confettiSystem.Emit(_cachedParams[index], 1);
            }
            yield return null;
        }
    }


}
