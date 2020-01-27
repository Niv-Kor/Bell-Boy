public class CapacityStatistics : ElevatorStatisticsScreen
{
    protected override void UpdateScreen() {
        primaryText[0].text = elevator.Passengers.Count.ToString();
        secondaryText[1].text = "20"; ///TEMP
    }
}