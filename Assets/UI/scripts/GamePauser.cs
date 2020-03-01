﻿using UnityEngine;
using UnityEngine.UI;

public class GamePauser : Singleton<GamePauser>
{
    [Tooltip("The actual child tint rectangle object.")]
    [SerializeField] private Image tintRect;

    [Tooltip("The tint of the screen when the game is paused.")]
    [SerializeField] private Color screenTint;

    [Tooltip("The time it takes to perform a full transition from transparent to the tint color.")]
    [SerializeField] private float transitionTime;

    [Tooltip("Pause all Update methods.")]
    [SerializeField] private bool updatePause = true;

    private static readonly Color TRANSPARENT = new Color(0, 0, 0, 0);

    private float originTimeScale, lerpedTransitionTime;
    private bool transition;

    public bool IsPaused { get { return tintRect.gameObject.activeSelf; } }

    private void Start() {
        this.lerpedTransitionTime = 0;
        this.transition = false;
        tintRect.color = TRANSPARENT;
    }

    private void Update() {
        //transition from transparent tint to the parametered color
        if (transition) {
            lerpedTransitionTime += Time.deltaTime;
            tintRect.color = Color.Lerp(TRANSPARENT, screenTint, lerpedTransitionTime / transitionTime);

            //reached the desired tint color
            if (tintRect.color == screenTint) {
                transition = false;
                lerpedTransitionTime = 0;

                if (updatePause) Time.timeScale = 0;
            }
        }
    }

    /// <summary>
    /// Pause the game.
    /// This method has no effect if the game is already paused.
    /// </summary>
    public void PauseGame() {
        originTimeScale = Time.timeScale;
        transition = true;
        tintRect.gameObject.SetActive(true);
    }

    /// <summary>
    /// Resume the game.
    /// This method has no effect if the game is not paused.
    /// </summary>
    public void ResumeGame() {
        transition = false;
        Time.timeScale = originTimeScale;
        tintRect.color = TRANSPARENT;
        tintRect.gameObject.SetActive(false);
    }
}