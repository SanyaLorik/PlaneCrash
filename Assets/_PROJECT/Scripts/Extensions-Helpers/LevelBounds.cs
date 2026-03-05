using System;
using UnityEngine;

public class  LevelBounds : MonoBehaviour {
    
    // Потом наверное лучше добавить Collider вместо Renderer
    [SerializeField] private Transform _floor;
    [SerializeField] private Renderer _leftWall;
    [SerializeField] private Renderer _rightWall;
    [SerializeField] private Transform _cruiser;
    [SerializeField] private Transform _cruisePoint;
    [field: SerializeField] public float MaxY {get; private set; }
    [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }
    [field: SerializeField] public Transform BetZonePosition { get; private set; }
    [field: SerializeField] public Transform ParkourPosition { get; private set; }

    
    
    public Vector3 RecalculateCruiserY() =>
        new (_cruiser.position.x, _cruisePoint.position.y, _cruiser.position.z);


    public float MinY { get;private set; }
    public float LeftX { get;private set; }
    public float RightX { get;private set; }
    public Vector3 CruiserPosition { get;private set; }
    
    private void Awake() {
        MinY = _floor.position.y;
        LeftX = _leftWall.bounds.max.x;
        RightX = _rightWall.bounds.min.x;
    }

    public float CalculateFlightWidth() => Math.Abs(_leftWall.bounds.max.x) + Math.Abs(_rightWall.bounds.min.x);
    public float CalculateFlightHeight() => Math.Abs(MinY) + Math.Abs(MaxY);


    public Vector3 ClampPosition(Vector3 pos) {
        pos.x = Mathf.Clamp(pos.x, LeftX, RightX);
        pos.y = Mathf.Max(pos.y, MinY);
        return pos;
    }
}
