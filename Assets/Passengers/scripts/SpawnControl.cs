using UnityEngine;

public class SpawnControl : Singleton<SpawnControl>
{
    [Header("Timinig")]

    [Tooltip("Amount of time (in seconds) until the next spawn of a passenger in one of the floors.")]
    [SerializeField] public float FloorSpawnRate;

    [Tooltip("Amount of time (in seconds) until the next spawn of a pedestrian.")]
    [SerializeField] public float PedestrianSpawnRate;

    [Header("Debug")]

    [Tooltip("True to enable pedestrians to travel around the street.")]
    [SerializeField] private bool enablePedestrians = true;

    private static readonly string HUMANS_PARENT_NAME = "Humans";

    private GameObject humansParent;
    private PassengersPool pool;
    private float floorSpawnTimer, pedestrianSpawnTimer;

    private void Start() {
        this.pool = GetComponent<PassengersPool>();
        this.humansParent = new GameObject(HUMANS_PARENT_NAME);
        humansParent.transform.SetParent(transform);
        this.floorSpawnTimer = FloorSpawnRate;
        this.pedestrianSpawnTimer = PedestrianSpawnRate;
    }

    private void Update() {
        if (floorSpawnTimer >= FloorSpawnRate) {
            floorSpawnTimer = 0;
            Spawn();
        }

        if (enablePedestrians && pedestrianSpawnTimer >= PedestrianSpawnRate) {
            pedestrianSpawnTimer = 0;
            SpawnPedestrian();
        }

        floorSpawnTimer += Time.deltaTime;
        pedestrianSpawnTimer += Time.deltaTime;
    }

    /// <summary>
    /// Spawn a random passenger in one of the floors.
    /// </summary>
    public void Spawn() {
        Floor floor = StoreyBuilder.Instance.GetRandomFloor(false, false);
        if (!floor.IsAtFullCapacity()) Spawn(pool.Lease(), floor);
    }

    /// <summary>
    /// Spawn a random passenger in a certain floor.
    /// </summary>
    /// <param name="floor">The floor to spawn the passenger in</param>
    public void Spawn(Floor floor) {
        if (!floor.IsAtFullCapacity()) Spawn(pool.Lease(), floor);
    }

    /// <summary>
    /// Spawn a passenger in one of the floors.
    /// </summary>
    /// <param name="passenger">The passenger prefab to spawn</param>
    public void Spawn(Persona persona) {
        Floor floor = StoreyBuilder.Instance.GetRandomFloor(false, false);
        if (!floor.IsAtFullCapacity()) Spawn(persona, floor);
    }

    /// <summary>
    /// Spawn a passenger in a certain floor.
    /// </summary>
    /// <param name="persona">The passenger persona to spawn</param>
    /// <param name="floor">The floor to spawn the passenger in</param>
    public void Spawn(Persona persona, Floor floor) {
        GameObject passenger = pool.Lease(persona);
        Spawn(passenger, floor);
    }

    /// <summary>
    /// Spawn a passenger in a certain floor.
    /// </summary>
    /// <param name="passenger">The passenger instance to spawn</param>
    /// <param name="floor">The floor to spawn the passenger in</param>
    private void Spawn(GameObject passenger, Floor floor) {
        passenger.transform.SetParent(humansParent.transform);
        Passenger passengerComponent = passenger.GetComponent<Passenger>();
        passengerComponent.StartingFloorNum = floor.FloorNumber;
        passengerComponent.Reset();
        passengerComponent.AddJourney(JourneyPath.FloorEntrance, floor);
        floor.Passengers.Add(passengerComponent);
    }

    public void SpawnPedestrian() {
        SpawnPedestrian(pool.Lease());
    }

    private void SpawnPedestrian(GameObject passenger) {
        passenger.transform.SetParent(humansParent.transform);
        Passenger passengerComponent = passenger.GetComponent<Passenger>();
        passengerComponent.StartingFloorNum = 0;
        passengerComponent.Reset();
        passengerComponent.AddJourney(JourneyPath.Pedestrian, null);
    }

    /// <summary>
    /// Free a passenger and send it back to the pool of passengers.
    /// </summary>
    /// <param name="passenger">The passenger to cache</param>
    public void CachePassenger(GameObject passenger) { pool.Free(passenger); }
}