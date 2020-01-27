using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorStatisticsButton : MonoBehaviour
{
    [Tooltip("All the elevator statistics panels in the game.")]
    [SerializeField] private List<ElevatorStatistics> statisticsPanels;

    [Header("Sprites")]

    [Tooltip("Open doors button sprite.")]
    [SerializeField] private Sprite openDoorsSprite;

    [Tooltip("Close doors button sprite.")]
    [SerializeField] private Sprite closeDoorsSprite;

    private Image image;

    private void Start() {
        this.image = GetComponent<Image>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(delegate() { TransitPanels(); });
    }

    /// <summary>
    /// Open or close the statistics panels, and change the button's icon.
    /// </summary>
    public void TransitPanels() {
        if (statisticsPanels.Count == 0 || statisticsPanels[0].InTransition) return;

        Sprite toggleSprite = statisticsPanels[0].IsOpen ? closeDoorsSprite : openDoorsSprite;
        image.sprite = toggleSprite;
        foreach (ElevatorStatistics panel in statisticsPanels) panel.Transit();
    }
}