using System.Collections.Generic;
using UnityEngine;

public class CollectionsUtil
{
    /// <summary>
    /// Shuffle a list and mess its order.
    /// </summary>
    /// <param name="types">A list</param>
    public static void ShuffleList<T>(List<T> list) {
        for (int i = 0; i < list.Count; i++) {
            int rng = Random.Range(0, list.Count);
            T temp = list[rng];
            list[rng] = list[i];
            list[i] = temp;
        }
    }

    /// <summary>
    /// Select a random item from a list.
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="list">The list to select an item from</param>
    /// <returns>A random item from the list.</returns>
    public static T SelectRandom<T>(List<T> list) {
        if (list.Count == 0) return default(T);
        else return list[Random.Range(0, list.Count)];
    }
}
