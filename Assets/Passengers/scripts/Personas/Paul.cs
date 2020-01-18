public class Paul : Passenger
{
    protected override int[] GenerateTargetFloor() {
        int floor = StartingFloorNum;

        while (floor == StartingFloorNum)
            floor = StoreyBuilder.Instance.GetRandomFloorNumber(true, false);

        return new int[] { floor };
    }

    public override bool CanBeSpawned() { return true; }
}