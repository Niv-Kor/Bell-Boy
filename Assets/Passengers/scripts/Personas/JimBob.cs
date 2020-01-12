public class JimBob : Passenger
{
    protected override int[] GenerateTargetFloor() {
        int floor = StartingFloorNum;

        while (floor == StartingFloorNum)
            floor = FloorBuilder.Instance.GetRandomFloorNumber(true, false);

        return new int[] { floor };
    }

    public override bool CanBeSpawned() { return true; }
}