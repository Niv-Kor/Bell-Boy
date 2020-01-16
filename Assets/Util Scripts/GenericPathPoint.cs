using UnityEngine;

[System.Serializable]
public class GenericPathPoint
{
    [Tooltip("The starting point before deviation.")]
    [SerializeField] public Vector3 point;

    [Tooltip("Allowed deviation for the point, which will occur at the x and z axes.")]
    [SerializeField] private Vector3 EpsilonDeviation;

    /// <summary>
    /// Create a deviated point as a variant of the default point.
    /// </summary>
    /// <param name="position">The position of the floor</param>
    /// <returns></returns>
    public Vector3 GeneratePoint(Vector3 relativePoint, float relativeHeight) {
        float x = relativePoint.x + GenerateValue(point.x, EpsilonDeviation.x);
        float y = relativeHeight + point.y;
        float z = relativePoint.z + GenerateValue(point.z, EpsilonDeviation.z);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Deviate a value.
    /// </summary>
    /// <param name="value">The value to deviate</param>
    /// <param name="deviation">Maximum allowed deviation</param>
    /// <returns>A deviated value.</returns>
    private static float GenerateValue(float value, float deviation) {
        float randomDirection = Random.Range(-1f, 1f);
        return value + deviation * randomDirection;
    }
}
