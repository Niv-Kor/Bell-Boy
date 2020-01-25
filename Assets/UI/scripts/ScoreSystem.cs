using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : Singleton<ScoreSystem>
{
    [Header("Initial Values")]

    [Tooltip("Tips value to start with.")]
    [SerializeField] private int initialTips;

    [Tooltip("Timer value to start with (in milliseconds).")]
    [SerializeField] private int initialTimer;

    [Tooltip("Amount of lives to start with.")]
    [SerializeField] [Range(1, 10)] private int initialReputation = 3;

    public TipsJar TipsJar { get; private set; }
    public TimerRunner TimerRunner { get; private set; }
    public Reputation Reputation { get; private set; }
    public CoinsPool CoinsPool { get; private set; }
    public List<LifeBit> ReputationBits { get; private set; }

    private void Awake() {
        this.TipsJar = FindObjectOfType<TipsJar>();
        this.TimerRunner = FindObjectOfType<TimerRunner>();
        this.Reputation = FindObjectOfType<Reputation>();
        this.CoinsPool = GetComponent<CoinsPool>();
        this.ReputationBits = new List<LifeBit>();

        TipsJar.TotalValue = initialTips;
        TimerRunner.Timer = initialTimer;
        Reputation.CreateLifeBits(initialReputation);
    }
}