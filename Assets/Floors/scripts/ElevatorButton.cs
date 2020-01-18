using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [Tooltip("The material to apply on the button's frame when it's on.")]
    [SerializeField] private Material onColor;

    [Tooltip("The material to apply on the button's frame when it's off.")]
    [SerializeField] private Material offColor;

    [Tooltip("The button frame objet.")]
    [SerializeField] private GameObject buttonFrame;

    public delegate void OnElevatorCall();
    private event OnElevatorCall OnCall;
    private MeshRenderer buttonRender;
    private SoundMixer soundMixer;

    public bool IsOn { get; private set; }

    private void Start() {
        this.soundMixer = GetComponent<SoundMixer>();
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
    }

    /// <summary>
    /// Call The elevator.
    /// </summary>
    public void Call() {
        Switch(true);
        OnCall?.Invoke();
        soundMixer.Activate("click");
    }

    /// <summary>
    /// Subscribe to the elevator call event.
    /// </summary>
    /// <param name="ev">A method to invoke when the event occurs</param>
    public void SubscribeCall(OnElevatorCall ev) { OnCall += ev; }

    /// <summary>
    /// Unubscribe from the elevator call event.
    /// </summary>
    /// <param name="ev">The method to remove from the event</param>
    public void UnsubscribeCall(OnElevatorCall ev) { OnCall -= ev; }
}