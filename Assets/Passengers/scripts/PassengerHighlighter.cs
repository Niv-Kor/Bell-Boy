using UnityEngine;

public class PassengerHighlighter : MonoBehaviour, IHighlightable
{
    [Tooltip("The passenger's mesh renderer.")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [Tooltip("The outline highlight material.")]
    [SerializeField] private Material outlineMaterial;

    private Material originMaterial;

    private void Start() {
        this.originMaterial = meshRenderer.materials[0];
    }

    public void Highlight(bool flag) {
        Material[] materialsArr = meshRenderer.materials;
        Material secondMaterial = flag ? outlineMaterial : null;
        materialsArr[1] = secondMaterial;
        meshRenderer.materials = materialsArr;
    }
}