using UnityEngine;

public class HeightUnboost : MonoBehaviour, IBoostObject {
    [SerializeField] private GameObject _boostPrefab;
    [SerializeField] private float _bustDown = -3f; // сила подьема от буста

    
    public void ApplyBoost(PlayerMovement playerMovement) {
        playerMovement.AddVerticalVelocity(_bustDown);
        Destroy(gameObject);
    }
}
