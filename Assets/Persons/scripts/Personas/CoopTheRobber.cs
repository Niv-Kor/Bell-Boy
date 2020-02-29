public class CoopTheRobber : Passenger
{
    protected override int[] GenerateTargetFloor() {
        return new int[] { 0 };
    }

    public override bool CanBeSpawned() { return true; }
}