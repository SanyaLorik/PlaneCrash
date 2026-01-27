using System;
using UnityEngine;

public class TrapVisual : MonoBehaviour {
     
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _brightnessMultiply = 5f;

    [SerializeField] private Renderer _bombRenderer; 
    
    private Material _triggerObjectMat;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
    
    public void Awake() {
        _triggerObjectMat = _bombRenderer.GetComponent<Renderer>().material;
        _triggerObjectMat.EnableKeyword("_EMISSION");
    }


    public void SetRed() {
        Color emission = Color.red * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка красным");
    }
    
    public void SetGreen() {
        Color emission = Color.green * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка зеленым");
    } 
    
    public void SetBlue() {
        Color emission = Color.blue * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка зеленым");
    }
    
    
    public void SetYellow() {
        Color emission = Color.yellow * _brightnessMultiply; // множитель яркости
        _triggerObjectMat.SetColor(EmissionColor, emission);
        // Debug.Log("Установка зеленым");
    }


    public void GetEffect() {
        _particleSystem.Play();
    }
}
