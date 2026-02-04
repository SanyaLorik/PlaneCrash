using System;
using SanyaBeerExtension;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConfettiSpawner : MonoBehaviour {
    [SerializeField] private ParticleSystem confettiSystem;
    [SerializeField] private Sprite[] confettiSprites; // массив белых форм

    [SerializeField] private int _countConfetti;

    private ParticleSystemRenderer renderer;


    private void Awake() {
        renderer = confettiSystem.GetComponent<ParticleSystemRenderer>();
    }


    public void SpawnConfetti() {
        var main = confettiSystem.main;
        main.maxParticles = _countConfetti;


        for (int i = 0; i < _countConfetti; i++) {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

            // Рандомная позиция
            emitParams.position = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(0f, 5f),
                Random.Range(-5f, 5f)
            );

            // Рандомный цвет
            emitParams.startColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);


            // Рандомная форма (спрайт)
            int spriteIndex = Random.Range(0, confettiSprites.Length);
            renderer.material.mainTexture = confettiSprites[spriteIndex].texture;

            confettiSystem.Emit(emitParams, 1);
        }

        confettiSystem.Play();
    }
}
