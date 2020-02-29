using UnityEngine;

public class EntranceHighlighter : MonoBehaviour, IHighlightable
{
    [Header("Identification")]

    [Tooltip("The type of this entrance's highlighting.")]
    [SerializeField] public HighlightType HighlightType;

    [Header("Prefabs")]

    [Tooltip("The highlighting material.")]
    [SerializeField] private Material colorMaterial;

    [Tooltip("The top metal boundary of the entrance.")]
    [SerializeField] private MeshRenderer topBorder;

    [Tooltip("The left metal boundary of the entrance.")]
    [SerializeField] private MeshRenderer leftBorder;

    [Tooltip("The right metal boundary of the entrance.")]
    [SerializeField] private MeshRenderer rightBorder;

    private Material originMaterial;

    public bool IsHighlighted { get; private set; }

    private void Start() {
        this.originMaterial = topBorder.material;
        this.IsHighlighted = false;
    }

    public void Highlight(bool flag) {
        Material material = flag ? colorMaterial : originMaterial;
        topBorder.material = material;
        leftBorder.material = material;
        rightBorder.material = material;
        IsHighlighted = flag;
    }
}