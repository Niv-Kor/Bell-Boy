public struct JourneyData
{
    public Journey Journey;
    public JourneyPath PathType;
    public bool Paused;

    public JourneyData(Journey journey, JourneyPath type) {
        this.Journey = journey;
        this.PathType = type;
        this.Paused = false;
    }

    public void Clear() {
        if (Journey != null) Journey.Stop();
        this.Journey = null;
        this.PathType = JourneyPath.Blank;
    }
}
