using System.Collections.Generic;
using UnityEngine;

public abstract class RoutesPool : MonoBehaviour, IPoolable<Queue<Vector3>>
{
    [Header("Routes Configuration")]

    [Tooltip("Paths a person can walk through.")]
    [SerializeField] protected List<GenericPath> routes;

    protected List<Queue<GenericPathPoint>> defaultPaths;
    protected List<Queue<Vector3>> freePaths;
    protected IDictionary<Queue<Vector3>, Queue<Vector3>> occupiedPaths;

    protected virtual void Start() {
        this.freePaths = new List<Queue<Vector3>>();
        this.occupiedPaths = new Dictionary<Queue<Vector3>, Queue<Vector3>>();
        this.defaultPaths = new List<Queue<GenericPathPoint>>();
        foreach (GenericPath path in routes) defaultPaths.Add(path.GetAsQueue());

        GeneratePaths();
    }

    /// <summary>
    /// Create new deviated variations of the given set of paths.
    /// </summary>
    protected virtual void GeneratePaths() {
        for (int i = 0; i < MaxRoutes(); i++) {
            Queue<GenericPathPoint> randomPath = CollectionsUtil.SelectRandom(defaultPaths);
            Queue<GenericPathPoint> clonePath = new Queue<GenericPathPoint>(randomPath);
            Queue<Vector3> generatedPath = new Queue<Vector3>();

            //enqueue all points of the generated path
            while (clonePath.Count > 0) {
                Vector3 point = clonePath.Dequeue().GeneratePoint(RelativePoint(), RelativeHeight());
                generatedPath.Enqueue(point);
            }

            //add the path to the free paths list
            freePaths.Add(generatedPath);
        }

        CollectionsUtil.ShuffleList(freePaths);
    }

    public virtual Queue<Vector3> Lease() {
        if (freePaths.Count == 0) return null;

        Queue<Vector3> path = CollectionsUtil.SelectRandom(freePaths);
        Queue<Vector3> dupPath = new Queue<Vector3>(path);
        occupiedPaths.Add(dupPath, path);
        freePaths.Remove(path);
        return dupPath;
    }

    public virtual Queue<Vector3> Clone() {
        List<Queue<Vector3>> allPaths = new List<Queue<Vector3>>(freePaths);
        allPaths.AddRange(occupiedPaths.Values);
        CollectionsUtil.ShuffleList(allPaths);
        return new Queue<Vector3>(CollectionsUtil.SelectRandom(allPaths));
    }

    public virtual void Free(Queue<Vector3> obj) {
        if (!occupiedPaths.TryGetValue(obj, out Queue<Vector3> originPath)) return;

        occupiedPaths.Remove(obj);
        freePaths.Add(originPath);
    }

    /// <returns>The maximum amount of routes to generate.</returns>
    protected abstract int MaxRoutes();

    /// <returns>The point from which to measure the routes' points.</returns>
    protected abstract Vector3 RelativePoint();

    /// <returns>The height from which to measure the routes' points.</returns>
    protected abstract float RelativeHeight();
}