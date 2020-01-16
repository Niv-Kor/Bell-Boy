using System.Collections.Generic;
using UnityEngine;

public class StreetsMap : RoutesPool
{
    protected override void Start() {
        List<GenericPath> reversedPaths = new List<GenericPath>();

        foreach (GenericPath path in routes) {
            GenericPath reversed = new GenericPath { points = new List<GenericPathPoint>(path.points) };
            reversed.points.Reverse();
            reversedPaths.Add(reversed);
        }

        routes.AddRange(reversedPaths);
        base.Start();
    }

    protected override int MaxRoutes() { return 10; }

    protected override float RelativeHeight() { return 0; }

    protected override Vector3 RelativePoint() { return Vector3.zero; }
}