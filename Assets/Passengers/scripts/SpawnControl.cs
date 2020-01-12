using UnityEngine;

public class SpawnControl : Singleton<SpawnControl>
{
    [Header("Timinig")]

    [Tooltip("Amount of time (in seconds) until the next spawn.")]
    [SerializeField] public float SpawnRate;

    [Tooltip("Amount of time (in seconds) until an unsuccessful spawn is attempted again.")]
    [SerializeField] public float RespawnRate;

    private static readonly string HUMANS_PARENT_NAME = "Humans";

    private GameObject humansParent;
    private PassengersPool pool;
    private float spawnTimer;

    private void Start() {
        this.pool = GetComponent<PassengersPool>();
        this.humansParent = new GameObject(HUMANS_PARENT_NAME);
        humansParent.transform.SetParent(transform);
        this.spawnTimer = SpawnRate;
    }

    private void Update() {
        if (spawnTimer >= SpawnRate) {
            spawnTimer = 0;
            Spawn();
        }
        else spawnTimer += Time.deltaTime;
    }

    /// <summary>
    /// Spawn a random passenger in one of the floors.
    /// </summary>
    public void Spawn() {
        Floor floor = FloorBuilder.Instance.GetRandomFloor(false, false);
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
        Floor floor = FloorBuilder.Instance.GetRandomFloor(false, false);
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

    /// <summary>
    /// Free a passenger and send it back to the pool of passengers.
    /// </summary>
    /// <param name="passenger">The passenger to cache</param>
    public void CachePassenger(GameObject passenger) { pool.Free(passenger); }
}