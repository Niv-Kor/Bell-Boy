using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageKids : Passenger
{
    protected override int[] GenerateTargetFloor() {
        int[] floors = new int[StoreyBuilder.Instance.Storeys.Length - 1];
        for (int i = 0; i < floors.Length; i++) floors[i] = i;

        return floors;
    }

    public override bool CanBeSpawned() { return true; }
}