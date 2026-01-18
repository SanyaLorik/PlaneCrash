using SanyaBeerExtension;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject {
    [Header("Flight data")]
    [field: SerializeField] public float SpeedForce  { get; private set; } = 10f;
    [field: SerializeField] public float WalkSpeed { get; private set; } = 10f;
    [field: SerializeField] public float FallingSpeed { get; private set; } = 7f;
    [field: SerializeField] public float JumpHeight { get; private set; }
    [field: SerializeField] public float MaxRotate { get; private set; } = 20f;
    [field: SerializeField] public PairedValue<float> XMovement { get; private set; }
    
    [Header("Rotate data")]
    [field: SerializeField] public float RotateSpeed { get; private set; } = 6f;
   
    [Header("Rotate data")]
    [field: SerializeField] public Vector3 PlayerSpawnPosition { get; private set; }
    
    [Header("Jump/Collider data")]
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float SecondJumpForce { get; private set; }
    [field: SerializeField] public float WallOffset { get; private set; }
    [field: SerializeField] public LayerMask FloorMask { get; private set; }
    [field: SerializeField] public float GravityScale { get; private set; } = 2f;
}
