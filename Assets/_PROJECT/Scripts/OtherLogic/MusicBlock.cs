using System;
using UnityEngine;

public class MusicBlock  : MonoBehaviour {
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private string _compositionName;
    [SerializeField] private Color _tileColor;

    private void Awake() {
        GetComponent<Renderer>().material.color = _tileColor;
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement _)) {
            _audioSource.Play();
            Debug.Log("Проигрывание " + _compositionName);
        }
    }
}
