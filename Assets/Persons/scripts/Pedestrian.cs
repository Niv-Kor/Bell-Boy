public class Pedestrian : Person
{
    private void OnEnable() {
        TargetMark targetMark = GetComponentInChildren<TargetMark>();
        targetMark.gameObject.SetActive(false);
        AddJourney(JourneyPath.Pedestrian, null);
    }
}