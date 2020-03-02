using System.Collections.Generic;
using UnityEngine;

public abstract class RoutesPool : MonoBehaviour, IPoolable<Queue<Vector3>>
{
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

    [Tooltip("Paths a person can walk through.")]
    [SerializeField] protected List<GenericPath> routes;

    private static readonly float GIZMO_RADIUS = .4f;
    private static readonly Color GIZMO_COLOR = Color.blue;

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

    private void OnDrawGizmos() {
        for (int i = 0; i < routes.Count; i++) {
            Gizmos.color = GIZMO_COLOR;

            foreach (GenericPathPoint point in routes[i].points)
                Gizmos.DrawSphere(point.point, GIZMO_RADIUS);
        }
    }

    /// <returns>True if there are free routes.</returns>
    public bool HasFree() { return freePaths.Count > 0; }

    /// <summary>
    /// Create new deviated variations of the given set of paths.
    /// </summary>
    protected virtual void GeneratePaths() {
        bool exactDefinition = GenerateExactDefinition();
        int pathsAmount = exactDefinition ? routes.Count : MaxRoutes();

        for (int i = 0; i < pathsAmount; i++) {
            Queue<GenericPathPoint> selectedPath;
            if (GenerateExactDefinition()) selectedPath = defaultPaths[i];
            else selectedPath = CollectionsUtil.SelectRandom(defaultPaths);

            Queue<GenericPathPoint> clonePath = new Queue<GenericPathPoint>(selectedPath);
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

    /// <returns>True if there should only be route of each kind.</returns>
    protected abstract bool GenerateExactDefinition();

    /// <returns>The maximum amount of routes to generate.</returns>
    protected abstract int MaxRoutes();

    /// <returns>The point from which to measure the routes' points.</returns>
    protected abstract Vector3 RelativePoint();

    /// <returns>The height from which to measure the routes' points.</returns>
    protected abstract float RelativeHeight();
}