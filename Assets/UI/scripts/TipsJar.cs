using TMPro;
using UnityEngine;

public class TipsJar : MonoBehaviour
{
    [Header("Prefabs")]

    [Tooltip("The textual value of the timer.")]
    [SerializeField] private TextMeshProUGUI value;

    [Header("Base Values")]

    [Tooltip("Minimum base tip value.")]
    [SerializeField] private int minBaseValue;

    [Tooltip("Maximum base tip value.")]
    [SerializeField] private int maxBaseValue;

    [Tooltip("Amount of patience seconds that derive the minimum benefit of tip value.")]
    [SerializeField] private float minimalBenfitSeconds;

    [Tooltip("Amount of patience seconds that derive the maximum benefit of tip value.")]
    [SerializeField] private float maximalBenfitSeconds;

    private static readonly int DIGITS = 7;
    private static readonly float MAX_SLOW_INCREMENT_PERCENT = 2f;

    private TimerRunner timer;
    private CoinsPool coinsPool;
    private int processingValue, totalValue;

    public int MaxValue {
        get {
            int num = 1;
            for (int i = 0; i < DIGITS; i++) num *= 10;
            return num - 1;
        }
    }

    public int TotalValue {
        get { return totalValue; }
        set {
            int summed = value;
            int maxVal = MaxValue;
            if (summed > maxVal) summed = maxVal;

            totalValue = summed;
        }
    }
    
    private void Start() {
        this.timer = ScoreSystem.Instance.TimerRunner;
        this.coinsPool = ScoreSystem.Instance.CoinsPool;
        Add(0); //init counter
    }

    private void Update() {
        int calculationDifference = TotalValue - processingValue;

        if (Mathf.Abs(calculationDifference) > 0) {
            int differenceDigits = NumeralUtils.CountDigits(calculationDifference);
            int decimalScale = (int) Mathf.Pow(10, differenceDigits - 1);
            bool slowDown = calculationDifference <= decimalScale * MAX_SLOW_INCREMENT_PERCENT;

            int additiveStep = decimalScale;
            additiveStep /= slowDown ? 10 : 1;
            if (additiveStep == 0) additiveStep = 1;

            Add(additiveStep);
        }
    }

    /// <summary>
    /// Add coins to the tips jar, as a function of game time.
    /// </summary>
    /// <param name="tossFrom">The position from which the coin should be tossed</param>
    /// <param name="baseValue">The base value of the tip</param>
    public void Tip(Vector3 tossFrom, int baseValue) {
        int calculatedValue = (int) (Mathf.Pow(2, timer.Minutes) * baseValue);
        Coin coin = coinsPool.Lease();
        coin.Value = calculatedValue;
        coin.Toss(tossFrom);
    }

    /// <summary>
    /// Add coins to the tips jar.
    /// </summary>
    /// <param name="val">The amount of coins to add</param>
    private void Add(int val) {
        string cleanText = value.text.Replace(",", ""); //clear commas
        int currentValue = int.Parse(cleanText); //retrieve value
        processingValue = currentValue + val; //sum values
        string stringVal = processingValue.ToString(); //convert back to string
        int valueDigits = stringVal.Length;

        //shift right
        for (int i = 0; i < DIGITS - valueDigits; i++)
            stringVal = "0" + stringVal;

        //add commas
        for (int i = DIGITS - 1, counter = 0; i >= 0; i--, counter++) {
            if (counter == 3) {
                counter = 0;
                stringVal = stringVal.Insert(i + 1, ",");
            }
        }

        //update text
        value.text = stringVal;
    }

    /// <summary>
    /// Convert total patience seconds of a passenger to base tip value.
    /// </summary>
    /// <param name="tolerance">Total patience of a passenger</param>
    /// <returns>Base tip value.</returns>
    public int PatienceToTipValue(float patience) {
        if (patience >= minimalBenfitSeconds) return minBaseValue;
        else if (patience <= maximalBenfitSeconds) return maxBaseValue;
        else {
            float percentOfBenefit = 1 - (patience - maximalBenfitSeconds) / (minimalBenfitSeconds - maximalBenfitSeconds);
            return (int) (percentOfBenefit * (maxBaseValue - minBaseValue)) + minBaseValue;
        }
    }
}
