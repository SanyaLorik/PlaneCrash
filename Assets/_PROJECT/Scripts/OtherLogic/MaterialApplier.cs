using SanyaBeerExtension;
using UnityEngine;

public class MaterialApplier : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _renderers;
    [SerializeField] private Material[] _materials;

    public void ApplyRandomMaterial()
    {
        Material material = _materials.GetRandomElement();
        _renderers.ForEach(i => i.material = material);
    }
}
