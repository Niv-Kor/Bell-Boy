using UnityEngine;

public class RandomUtils : MonoBehaviour
{
    /// <summary>
    /// Get a completely random boolean result.
    /// </summary>
    /// <returns>True at a completely random chance.</returns>
    public static bool UnstableCondition() {
        return UnstableCondition(Random.Range(0f, 1f));
    }

    /// <summary>
    /// Get a positive result at a specified chance.
    /// </summary>
    /// <param name="percent">The chance to receive True as a result</param>
    /// <returns>True at a specified chance.</returns>
    public static bool UnstableCondition(float percent) {
        float rng = Random.Range(0f, 1f);
        return rng <= percent;
    }
}