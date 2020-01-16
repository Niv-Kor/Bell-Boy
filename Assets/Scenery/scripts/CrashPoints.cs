using UnityEngine;

public class CrashPoints : MonoBehaviour
{
    [Tooltip("The left crashing area.")]
    [SerializeField] public BoxCollider LeftArea;

    [Tooltip("The right crashing area.")]
    [SerializeField] public BoxCollider RightArea;

    [Tooltip("The scene's main terrain.")]
    [SerializeField] private Terrain terrain;

    public Vector3 GenerateLeft() { return Generate(LeftArea); }

    public Vector3 GenerateRight() { return Generate(RightArea); }

    private Vector3 Generate(BoxCollider area) {
        float x = area.bounds.center.x - area.bounds.extents.x;
        float z = area.bounds.center.z - area.bounds.extents.z;

        x += Random.Range(0f, area.size.x);
        z += Random.Range(0f, area.size.z);

        return new Vector3(x, terrain.transform.position.y, z);
    }
}