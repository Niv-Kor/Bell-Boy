using UnityEngine;

[RequireComponent(typeof(Passenger))]
[RequireComponent(typeof(Pedestrian))]
public class PersonInlayer : MonoBehaviour
{
    private Passenger passengerComponent;
    private Pedestrian pedestrianComponent;

    public PersonRole Role { get; private set; }

    private void Awake() {
        //spawn at neutral point
        transform.position = SpawnController.Instance.NeutralPoint;

        this.passengerComponent = GetComponent<Passenger>();
        this.pedestrianComponent = GetComponent<Pedestrian>();
    }

    /// <summary>
    /// Set a role for a spawned person.
    /// </summary>
    /// <param name="role">
    /// PersonRole.Passenger to spawn the person as a passenger in one of the floors,
    /// or PersonRole.Pedestrian to spawn the person as a pedestrian in the street
    /// </param>
    public void Inlay(PersonRole role) {
        Role = role;
        bool passenger = role == PersonRole.Passenger;
        bool pedestrian = role == PersonRole.Pedestrian;
        passengerComponent.enabled = passenger;
        pedestrianComponent.enabled = pedestrian;
    }
}