﻿using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [Tooltip("The material to apply on the button's frame when it's on.")]
    [SerializeField] private Material onColor;

    [Tooltip("The material to apply on the button's frame when it's off.")]
    [SerializeField] private Material offColor;

    [Tooltip("The button frame objet.")]
    [SerializeField] private GameObject buttonFrame;

    public event System.Action<ElevatorButton> OnElevatorCall = delegate {};
    private MeshRenderer buttonRender;
    private Floor floor;

    public bool IsOn { get; private set; }

    private void Start() {
        this.floor = GetComponentInParent<Floor>();
        this.buttonRender = buttonFrame.GetComponent<MeshRenderer>();
        this.IsOn = false;
    }

    /// <summary>
    /// Turn the button light on or off.
    /// </summary>
    /// <param name="flag">True to turn the button light on</param>
    public void Switch(bool flag) {
        IsOn = flag;
        Material mat = flag ? onColor : offColor;
        buttonRender.material = mat;
        floor.ElevatorBeingCalled = flag;
    }

    /// <summary>
    /// Call The elevator.
    /// </summary>
    public void Call() {
        Switch(true);
        OnElevatorCall(this);
    }
}