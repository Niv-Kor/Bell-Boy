public class CurrentFloorStatistics : ElevatorStatisticsScreen
{
    protected override void UpdateScreen() {
        primaryText[0].text = elevator.CurrentFloorNum.ToString();
    }
}