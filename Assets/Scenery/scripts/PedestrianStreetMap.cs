using UnityEngine;

public class PedestrianStreetMap : RoutesPool
{
    protected override bool GenerateExactDefinition() { return true; }
    protected override int MaxRoutes() { return 4; }
    protected override Vector3 RelativePoint() { return Vector3.zero; }
    protected override float RelativeHeight() { return 0; }
}