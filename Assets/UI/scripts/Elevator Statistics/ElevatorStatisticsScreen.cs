using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElevatorStatisticsScreen : MonoBehaviour
{
    [Header("Primary Stats")]

    [Tooltip("A list of primary text components.")]
    [SerializeField] protected List<TextMeshProUGUI> primaryText;

    [Tooltip("Primary color of the text.")]
    [SerializeField] protected Color primaryColor;

    [Header("Secondary Stats")]

    [Tooltip("A list of secondary text components.")]
    [SerializeField] protected List<TextMeshProUGUI> secondaryText;

    [Tooltip("Secondary color of the text.")]
    [SerializeField] protected Color secondaryColor;

    private ElevatorStatistics statistics;
    protected MobileElevator elevator;

    protected virtual void Start() {
        this.statistics = GetComponentInParent<ElevatorStatistics>();
        this.elevator = ElevatorsManager.GetElevator(statistics.ElevatorID);

        //colorize texts
        foreach (TextMeshProUGUI text in primaryText) text.color = primaryColor;
        foreach (TextMeshProUGUI text in secondaryText) text.color = secondaryColor;

        //subsribe to elevator status changes
        elevator.ElevatorStatusUpdateTrigger += UpdateScreen;
    }

    /// <summary>
    /// Update the relevant components on the screen.
    /// </summary>
    protected virtual void UpdateScreen() {}
}