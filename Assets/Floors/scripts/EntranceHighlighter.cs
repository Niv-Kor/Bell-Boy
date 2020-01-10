using UnityEngine;

public class EntranceHighlighter : MonoBehaviour, IHighlightable
{
    [Tooltip("The highlighting material.")]
    [SerializeField] private Material colorMaterial;

    [Tooltip("The top metal boundary of the entrance.")]
    [SerializeField] private MeshRenderer topBorder;

    [Tooltip("The left metal boundary of the entrance.")]
    [SerializeField] private MeshRenderer leftBorder;

    [Tooltip("The right metal boundary of the entrance.")]
    [SerializeField] private MeshRenderer rightBorder;

    private Material originMaterial;

    private void Start() {
        this.originMaterial = topBorder.material;
    }

    public void Highlight(bool flag) {
        Material material = flag ? colorMaterial : originMaterial;
        topBorder.material = material;
        leftBorder.material = material;
        rightBorder.material = material;
    }
}