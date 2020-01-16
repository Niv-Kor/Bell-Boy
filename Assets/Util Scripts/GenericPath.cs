using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericPath
{
    [Tooltip("A list of general points alongs the path.")]
    [SerializeField] public List<GenericPathPoint> points;

    /// <summary>
    /// Convert the list of points to a queue;
    /// </summary>
    /// <returns>The list as a queue.</returns>
    public Queue<GenericPathPoint> GetAsQueue() {
        return new Queue<GenericPathPoint>(points);
    }
}
