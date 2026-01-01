using UnityEngine;

public class HeightBoost : MonoBehaviour, IBoostObject {
    [SerializeField] private GameObject _boostPrefab;
    [SerializeField] private float _bustUp = 4f; // сила подьема от буста

    
    public void ApplyBoost(PlayerMovement playerMovement) {
        playerMovement.ApplyVerticalBoost(_bustUp);
        Destroy(gameObject);
    }
}
