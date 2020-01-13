﻿using System.Collections.Generic;
using UnityEngine;

public class ElevatorsManager : MonoBehaviour
{
    [Tooltip("The building's elevators.")]
    [SerializeField] private List<MobileElevator> elevators;

    private static List<MobileElevator> elevatorsList;

    private void Awake() {
        elevatorsList = new List<MobileElevator>(elevators);
    }

    /// <param name="ID">The elevator's ID</param>
    /// <returns>The specified elevator.</returns>
    public static MobileElevator GetElevator(ElevatorID ID) {
        return elevatorsList.Find((x) => x.ID == ID);
    }

    /// <returns>A list of all elevators in the building.</returns>
    public static List<MobileElevator> GetAllElevators() { return elevatorsList; }
}