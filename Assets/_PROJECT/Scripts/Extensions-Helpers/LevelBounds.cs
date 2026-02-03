using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class  LevelBounds : MonoBehaviour {
    
    // Потом наверное лучше добавить Collider вместо Renderer
    [SerializeField] private Renderer _floor;
    [SerializeField] private Renderer _leftWall;
    [SerializeField] private Renderer _rightWall;
    [SerializeField] private Renderer _cruiser;
    [field: SerializeField] public float MaxY {get; private set; }




    public Vector3 RecalculateCruiserY() {
       float cruiserY = _cruiser.bounds.max.y;
       CruiserPosition = new Vector3(_cruiser.transform.position.x, cruiserY, _cruiser.transform.position.z);
       return CruiserPosition;
    }


    public float MinY { get;private set; }
    public float LeftX { get;private set; }
    public float RightX { get;private set; }
    public Vector3 CruiserPosition { get;private set; }
    
    private void Awake() {
        MinY = _floor.bounds.max.y;
        LeftX = _leftWall.bounds.max.x;
        RightX = _rightWall.bounds.min.x;
        RecalculateCruiserY();
        Debug.Log($"MinY = {MinY}   |  MaxY = {MaxY} ");
        Debug.Log($"LeftX = {LeftX} | RightX = {RightX} ");
    }

    public float CalculateFlightWidth() => Math.Abs(_leftWall.bounds.max.x) + Math.Abs(_rightWall.bounds.min.x);
    public float CalculateFlightHeight() => Math.Abs(MinY) + Math.Abs(MaxY);


    public Vector3 ClampPosition(Vector3 pos) {
        pos.x = Mathf.Clamp(pos.x, LeftX, RightX);
        pos.y = Mathf.Max(pos.y, MinY);
        return pos;
    }
}
