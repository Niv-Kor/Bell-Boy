using UnityEngine;

public static class VectorSensitivity
{
    /// <summary>
    /// Check if a vector has already reached a certain percentage of the distance it should make.
    /// </summary>
    /// <param name="pos">The current position of the vector</param>
    /// <param name="destPos">The position that the vector should finally reach</param>
    /// <param name="distance">The total distance that's needed to be done</param>
    /// <param name="tolerance">A percentage of the total distance</param>
    /// <returns>True if the vector has already reached the specified percent of the distance.</returns>
    public static bool EffectivelyReached(Vector3 pos, Vector3 destPos, float distance, float tolerance) {
        return EffectivelyReached(pos, destPos, tolerance * distance / 100);
    }

    /// <summary>
    /// Check if a vector has already reached a certain percentage of the distance it should make.
    /// </summary>
    /// <param name="pos">The current position of the vector</param>
    /// <param name="destPos">The position that the vector should finally reach</param>
    /// <param name="toleranceUnits">A distance between the two vectors that can be ignored</param>
    /// <returns>True if the vector has already reached the specified percent of the distance.</returns>
    public static bool EffectivelyReached(Vector3 pos, Vector3 destPos, float toleranceUnits) {
        float dist = Vector3.Distance(pos, destPos);
        return dist <= toleranceUnits;
    }
}