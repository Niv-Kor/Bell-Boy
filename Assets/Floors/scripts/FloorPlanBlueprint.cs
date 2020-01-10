using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Floor))]
public class FloorPlanBlueprint : MonoBehaviour, IPoolable<Queue<Vector3>>
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
        public Vector3 GeneratePoint(Floor floor) {
            Vector3 floorPos = floor.transform.position;

            float x = floorPos.x + GenerateValue(point.x, EpsilonDeviation.x);
            float y = floor.IndoorHeight + point.y;
            float z = floorPos.z + GenerateValue(point.z, EpsilonDeviation.z);
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

    [Header("Routes Configuration")]

    [Tooltip("Paths a passenger can walk through," +
             "from building's depths to the elevators waiting hall or vise versa.")]
    [SerializeField] private List<GenericPath> routes;

    [Header("Decoration")]

    [Tooltip("Spots at which decorations can be placed.")]
    [SerializeField] public List<Vector3> DecorationSpots;

    private static readonly float WAITING_HALL_WIDTH_MULTIPLIER = .9f;
    private static readonly float WAITING_HALL_DEVIATION_PERCENT = .2f;

    private Floor floor;
    private List<Queue<GenericPathPoint>> defaultPaths;
    private List<Queue<Vector3>> freePaths;
    private IDictionary<Queue<Vector3>, Queue<Vector3>> occupiedPaths;

    private void Start() {
        this.floor = GetComponent<Floor>();
        this.freePaths = new List<Queue<Vector3>>();
        this.occupiedPaths = new Dictionary<Queue<Vector3>, Queue<Vector3>>();
        this.defaultPaths = new List<Queue<GenericPathPoint>>();
        foreach (GenericPath path in routes) defaultPaths.Add(path.GetAsQueue());

        GeneratePaths();
    }

    /// <summary>
    /// Create new deviated variations of the given set of paths.
    /// </summary>
    private void GeneratePaths() {
        for (int i = 0; i < floor.MaxCapacity; i++) {
            Queue<GenericPathPoint> randomPath = CollectionsUtil.SelectRandom(defaultPaths);
            Queue<GenericPathPoint> clonePath = new Queue<GenericPathPoint>(randomPath);
            Queue<Vector3> generatedPath = new Queue<Vector3>();

            //enqueue all points of the generated path
            while (clonePath.Count > 0) {
                Vector3 point = clonePath.Dequeue().GeneratePoint(floor);
                generatedPath.Enqueue(point);
            }

            //add the last point at the elevators' waiting hall
            Vector3 floorPos = floor.transform.position;
            Vector3 hallCenter = floor.WaitingHallContainer.center;
            Vector3 hallSize = floor.WaitingHallContainer.bounds.size;
            float deviation;

            //x position
            float hallWidth = hallSize.x * WAITING_HALL_WIDTH_MULTIPLIER;
            float mostLeftX = floorPos.x + hallCenter.x - hallWidth / 2;
            float spaceBlock = hallWidth / floor.MaxCapacity;
            deviation = Random.Range(-1f, 1f) * spaceBlock * WAITING_HALL_DEVIATION_PERCENT;
            float waitingHallX = mostLeftX + spaceBlock * (i + .5f) + deviation;

            //y position
            float waitingHallY = floor.IndoorHeight;

            //z position
            deviation = Random.Range(-1f, 1f);
            float waitingHallZ = floorPos.z + hallCenter.z + (hallSize.z / 2) * deviation;

            Vector3 lastPoint = new Vector3(waitingHallX, waitingHallY, waitingHallZ);
            generatedPath.Enqueue(lastPoint);

            //add the path to the free paths list
            freePaths.Add(generatedPath);
        }

        CollectionsUtil.ShuffleList(freePaths);
    }

    public Queue<Vector3> Lease() {
        if (freePaths.Count == 0) return null;

        Queue<Vector3> path = CollectionsUtil.SelectRandom(freePaths);
        Queue<Vector3> dupPath = new Queue<Vector3>(path);
        occupiedPaths.Add(dupPath, path);
        freePaths.Remove(path);
        return dupPath;
    }

    public Queue<Vector3> Clone() {
        List<Queue<Vector3>> allPaths = new List<Queue<Vector3>>(freePaths);
        allPaths.AddRange(occupiedPaths.Values);
        CollectionsUtil.ShuffleList(allPaths);
        return new Queue<Vector3>(CollectionsUtil.SelectRandom(allPaths));
    }

    public void Free(Queue<Vector3> obj) {
        if (!occupiedPaths.TryGetValue(obj, out Queue<Vector3> originPath)) return;

        occupiedPaths.Remove(obj);
        freePaths.Add(originPath);
    }
}