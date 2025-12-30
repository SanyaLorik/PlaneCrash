using UnityEngine;

public class HeightBoost : MonoBehaviour, IBoostObject {
    [SerializeField] private GameObject _boostPrefab;
    [SerializeField] private float _bustUp = 6f; // сила подьема от буста

    
    public void ApplyBoost(PlayerMovement playerMovement) {
        playerMovement.AddVerticalVelocity(_bustUp);
        Destroy(gameObject);
    }
}
