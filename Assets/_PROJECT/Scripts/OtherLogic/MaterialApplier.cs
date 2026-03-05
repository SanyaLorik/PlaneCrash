using SanyaBeerExtension;
using UnityEngine;

public class MaterialApplier : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _renderers;
    [SerializeField] private Material[] _materials;

    [SerializeField] private bool _isStartApplied = false;

    private void Start()
    {
        if (_isStartApplied == true)
            ApplyRandomMaterial();
    }

    public void ApplyRandomMaterial()
    {
        Material material = _materials.GetRandomElement();
        _renderers.ForEach(i => i.material = material);
    }
}
