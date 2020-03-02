using UnityEngine;

public class SpawnController : Singleton<SpawnController>
{
    [Tooltip("The point at which a person can be safely idle until spawned somewhere else.")]
    [SerializeField] public Vector3 NeutralPoint;

    [Tooltip("Amount of time (in seconds) until the next spawn of a person in the scene.")]
    [SerializeField] public float SpawnRate;

    [Tooltip("The role of the regularly spawned persons in the scene")]
    [SerializeField] private PersonRole regularSpawnRole;

    private static readonly string HUMANS_PARENT_NAME = "Humans";

    private GameObject humansParent;
    private PersonPool pool;
    private PedestrianStreetMap streetMap;
    private float spawnTimer;

    private void Start() {
        this.pool = GetComponent<PersonPool>();
        this.humansParent = new GameObject(HUMANS_PARENT_NAME);
        this.streetMap = FindObjectOfType<PedestrianStreetMap>();
        humansParent.transform.SetParent(transform);
        this.spawnTimer = SpawnRate;
    }

    private void Update() {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= SpawnRate) {
            spawnTimer = 0;
            Spawn(regularSpawnRole);
        }
    }

    /// <summary>
    /// Spawn a random person in the scene.
    /// </summary>
    /// <param name="role">The role of the person</param>
    public void Spawn(PersonRole role) {
        switch (role) {
            case PersonRole.Passenger:
                Floor floor = StoreyBuilder.Instance.GetRandomFloor(false, false);
                if (!floor.IsAtFullCapacity()) Spawn(pool.Lease(), floor);
                break;
            case PersonRole.Pedestrian:
                //check if there are free routes to use
                if (streetMap == null || streetMap.HasFree())
                    SpawnPedestrian(pool.Lease());

                break;
        }
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
        passenger.GetComponent<PersonInlayer>().Inlay(PersonRole.Passenger);
        Passenger passengerComponent = passenger.GetComponent<Passenger>();
        passengerComponent.StartingFloorNum = floor.FloorNumber;
        passengerComponent.Reset();
        passengerComponent.AddJourney(JourneyPath.FloorEntrance, floor);
        floor.Passengers.Add(passengerComponent);
    }

    /// <summary>
    /// Spawn a pedestrian in the street.
    /// </summary>
    /// <param name="pedestrian">The person instance to spawn</param>
    public void SpawnPedestrian(GameObject pedestrian) {
        pedestrian.transform.SetParent(humansParent.transform);
        pedestrian.GetComponent<PersonInlayer>().Inlay(PersonRole.Pedestrian);
        Pedestrian pedestrianComponent = pedestrian.GetComponent<Pedestrian>();
        pedestrianComponent.AddJourney(JourneyPath.Pedestrian, null);
    }

    /// <summary>
    /// Free a person and send it back to the pool of passengers.
    /// </summary>
    /// <param name="person">The person to cache</param>
    public void CachePerson(GameObject person) { pool.Free(person); }
}