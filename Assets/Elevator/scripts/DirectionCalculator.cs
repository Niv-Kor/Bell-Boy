public static class DirectionCalculator
{
    public static bool Equals(ElevatorDirection dir1, ElevatorDirection dir2) {
        var up = ElevatorDirection.Up;
        var down = ElevatorDirection.Down;

        if ((dir1 == up && dir2 == down) || (dir1 == down && dir2 == up)) return false;
        else return true;
    }
}