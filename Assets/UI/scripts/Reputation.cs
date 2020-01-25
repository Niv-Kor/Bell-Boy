using System.Collections.Generic;
using UnityEngine;

public class Reputation : MonoBehaviour
{
    [Tooltip("The prefab of the life bit itself.")]
    [SerializeField] private LifeBit lifeBitPrefab;

    [Tooltip("The position of the first life icon.")]
    [SerializeField] private Vector2 leftBitPos;

    [Tooltip("The space from one life icon to the other.")]
    [SerializeField] private float lifeBitSpace;

    private static readonly string EARN_LIFE_SFX = "plus_life";

    private List<LifeBit> repBits;
    private SoundMixer soundMixer;

    public delegate void GameLoss();
    public event GameLoss GameLossTrigger;

    private void Awake() {
        this.soundMixer = GetComponent<SoundMixer>();
        ScoreSystem.Instance.TimerRunner.MinuteNotifierTrigger += EarnLife;
    }

    /// <summary>
    /// Create the life bits on top of the in-game UI panel.
    /// </summary>
    /// <param name="amount">Amout of life bits to create</param>
    public void CreateLifeBits(int amount) {
        repBits = new List<LifeBit>();
        float xExtent = lifeBitPrefab.GetComponent<SpriteRenderer>().bounds.extents.x;

        for (int i = 0; i < amount; i++) {
            Vector2 pos = leftBitPos;
            pos.x += (xExtent + lifeBitSpace) * i;

            GameObject instance = Instantiate(lifeBitPrefab.gameObject);
            LifeBit lifeBitComponent = instance.GetComponent<LifeBit>();
            instance.transform.SetParent(transform);
            instance.transform.localPosition = pos;
            repBits.Add(lifeBitComponent);
        }
    }

    /// <summary>
    /// Lose a life bit.
    /// If all life bits are lost, a game loss event will be fired.
    /// </summary>
    public void LoseLife() {
        int lostIndex = -1;

        for (int i = repBits.Count - 1; i >= 0; i--) {
            if (repBits[i].Exists) {
                repBits[i].Switch(false);
                lostIndex = i;
                break;
            }
        }

        //lost last life bit
        if (lostIndex == 0) GameLossTrigger?.Invoke();
    }

    /// <summary>
    /// Earn a life bit.
    /// If all life bits still exist, nothing happens.
    /// </summary>
    /// <param name="minutes">Amount of minutes passed in the game</param>
    public void EarnLife(int minutes) {
        for (int i = 0; i < repBits.Count; i++) {
            if (!repBits[i].Exists) {
                repBits[i].Switch(true);
                repBits[i].PlayParticles();
                soundMixer.Activate(EARN_LIFE_SFX);
                break;
            }
        }
    }
}