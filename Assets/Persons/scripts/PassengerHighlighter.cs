using UnityEngine;

public class PassengerHighlighter : MonoBehaviour, IHighlightable
{
    [Tooltip("The passenger's mesh renderer.")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [Tooltip("The outline highlight material.")]
    [SerializeField] private Material outlineMaterial;

    public void Highlight(bool flag) {
        Material[] materialsArr = meshRenderer.materials;
        Material secondMaterial = flag ? outlineMaterial : null;
        materialsArr[1] = secondMaterial;
        meshRenderer.materials = materialsArr;
    }
}