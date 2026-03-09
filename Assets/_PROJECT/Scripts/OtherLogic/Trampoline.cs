using UnityEngine;
using Zenject;

public class Trampoline : MonoBehaviour {
    
    [Inject] private TrampolineManager _trampolineManager;
    
    public AudioSource AudioSource;
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement _)) {
            _trampolineManager.TrampolineJump(this);
        }
    }
    
}