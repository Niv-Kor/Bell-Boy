using UnityEngine;

public class TargetMark : MonoBehaviour
{
    [Tooltip("The color that the mark starts with.")]
    [SerializeField] private Color startColor;

    [Tooltip("The color that the mark slowly changes to as the passenger waits.")]
    [SerializeField] private Color endColor;

    [Tooltip("The color that the mark slowly changes to as it fades.")]
    [SerializeField] private Color fadeColor;

    [Tooltip("The time it takes the mark to fade away.")]
    [SerializeField] private float fadeTime = 1;

    [Tooltip("The material to show as an arrow pointing upwards (to a higher floor).")]
    [SerializeField] private Material upArrow;

    [Tooltip("The material to show as an arrow pointing downwards (to a lower floor).")]
    [SerializeField] private Material downArrow;

    [Tooltip("Mark materials for each floor.")]
    [SerializeField] private Material[] floorMaterials;

    private static readonly string DIRECTION_CHILD = "Direction";
    private static readonly string AURA_CHILD = "Aura";

    private Passenger passenger;
    private ParticleSystemRenderer[] particleRenderer;
    private ParticleSystem[] partSystem;
    private Color currentStartColor, currentEndColor;
    private float timeLerped, totalLerpTime;
    private bool timeUp;

    public bool IsActivated { get; private set; }
    public bool IsFading { get; private set; }

    private void Awake() {
        this.passenger = GetComponentInParent<Passenger>();
        this.particleRenderer = new ParticleSystemRenderer[3];
        particleRenderer[0] = GetComponent<ParticleSystemRenderer>();
        particleRenderer[1] = transform.Find(DIRECTION_CHILD).GetComponent<ParticleSystemRenderer>();
        particleRenderer[2] = transform.Find(AURA_CHILD).GetComponent<ParticleSystemRenderer>();

        this.partSystem = new ParticleSystem[3];
        partSystem[0] = GetComponent<ParticleSystem>();
        partSystem[1] = transform.Find(DIRECTION_CHILD).GetComponent<ParticleSystem>();
        partSystem[2] = transform.Find(AURA_CHILD).GetComponent<ParticleSystem>();

        Reset();
    }

    public void Reset() {
        this.timeUp = false;
        this.IsActivated = false;
        this.IsFading = false;

        //if this happens for the first time, wait for the target floor to be set manually via passenger
        try { SetFloorNumber(passenger.TargetFloorNum[0]); }
        catch (System.Exception) {}

        SetColor(startColor);
    }

    private void Update() {
        //not yet activated
        if (!IsActivated && !IsFading) return;

        //lerp color
        timeLerped += Time.deltaTime;
        Color nextColor = Color.Lerp(currentStartColor, currentEndColor, timeLerped / totalLerpTime);
        SetColor(nextColor);

        //time up
        if (!timeUp && currentEndColor != fadeColor && timeLerped >= totalLerpTime) {
            Activate(false);
            passenger.Kill();
            timeUp = true;
        }
    }

    /// <summary>
    /// Activate or deactivate the mark.
    /// </summary>
    /// <param name="flag">True to activate or false to fade the mark away.</param>
    public void Activate(bool flag) {
        if (!CanChangeMode(flag)) return;

        timeLerped = 0;
        IsActivated = flag;
        IsFading = !flag;

        if (flag) {
            totalLerpTime = passenger.Patience;
            currentStartColor = startColor;
            currentEndColor = endColor;
        }
        else {
            totalLerpTime = fadeTime;
            currentStartColor = partSystem[0].main.startColor.color;
            currentEndColor = fadeColor;
            partSystem[2].Stop();
        }
    }

    /// <param name="switchFlag">True to activate the mark</param>
    /// <returns>True if the mark's state can be changed</returns>
    private bool CanChangeMode(bool switchFlag) {
        return switchFlag != IsActivated || (IsActivated == IsFading && switchFlag);
    }

    /// <param name="floorNum">The floor number to show on the mark</param>
    public void SetFloorNumber(int floorNum) {
        particleRenderer[0].material = floorMaterials[floorNum];

        //change the direction arrow
        Material arrow = (floorNum > passenger.CurrentFloorNum) ? upArrow : downArrow;
        particleRenderer[1].material = arrow;
    }

    /// <param name="color">The new color of the target floor mark</param>
    private void SetColor(Color color) {
        for (int i = 0; i < partSystem.Length - 1; i++) SetColor(color, i);
    }

    /// <param name="color">The new color of the target floor mark</param>
    /// <param name="componentIndex">
    /// The index of the particle component:
    /// 0 - floor number
    /// 1 - direction
    /// 2 - aura
    /// </param>
    private void SetColor(Color color, int componentIndex) {
        var particleMain = partSystem[componentIndex].main;
        particleMain.startColor = color;
    }
}