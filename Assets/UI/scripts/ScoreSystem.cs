using UnityEngine;

public class ScoreSystem : Singleton<ScoreSystem>
{
    [Header("Prefabs")]

    [Tooltip("The parent object of the reputation lives")]
    [SerializeField] private GameObject livesParent;

    [Tooltip("The prefab of the life icon itself.")]
    [SerializeField] private GameObject lifeIconPrefab;

    [Header("Initial Values")]

    [Tooltip("Tips value to start with.")]
    [SerializeField] private int initialTips;

    [Tooltip("Timer value to start with (in milliseconds).")]
    [SerializeField] private int initialTimer;

    [Tooltip("Amount of lives to start with.")]
    [SerializeField] [Range(1, 10)] private int initialReputation = 3;

    [Header("Reputation")]

    [Tooltip("The position of the first life icon.")]
    [SerializeField] private Vector2 leftLifePos;

    [Tooltip("The space from one life icon to the other.")]
    [SerializeField] private float lifeIconSpace;

    public TipsJar TipsJar { get; private set; }
    public TimerRunner TimerRunner { get; private set; }
    public CoinsPool CoinsPool { get; private set; }

    private void Start() {
        this.TipsJar = FindObjectOfType<TipsJar>();
        this.TimerRunner = FindObjectOfType<TimerRunner>();
        this.CoinsPool = GetComponent<CoinsPool>();

        TipsJar.TotalValue = initialTips;
        TimerRunner.Timer = initialTimer;
        CreateLives();
    }

    private void CreateLives() {
        float lifeIconExtent = lifeIconPrefab.GetComponent<SpriteRenderer>().bounds.extents.x;

        for (int i = 0; i < initialReputation; i++) {
            Vector2 pos = leftLifePos;
            pos.x += (lifeIconExtent + lifeIconSpace) * i;

            GameObject instance = Instantiate(lifeIconPrefab);
            instance.transform.SetParent(livesParent.transform);
            instance.transform.localPosition = pos;
        }
    }
}