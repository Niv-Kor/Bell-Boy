using UnityEngine;

public class CameraIntro : MonoBehaviour
{
    [Header("Transform")]

    [Tooltip("The position of the camera when displaying the logo in the sky.")]
    [SerializeField] private Vector3 initialPosition;

    [Tooltip("The rotation of the camera after hitting the ground.")]
    [SerializeField] private Vector3 endRotation;

    [Tooltip("The direction from which the camera is pushed down.")]
    [SerializeField] private Vector3 torqueForce;

    [Tooltip("The speed of the camera's fall.")]
    [SerializeField] private float groundHeight = 0;

    [Header("Timing")]

    [Tooltip("The amount of time (in seconds) until the camera falls down.")]
    [SerializeField] private float logoDelay = 5;

    [Tooltip("The time it takes the camera to start rolling after hitting the ground (in seconds).")]
    [SerializeField] private float rollDelay = 1;

    [Tooltip("The of the camera's roll on the ground.")]
    [SerializeField] private float rollSpeed = 1;

    private Rigidbody rigidBody;
    private ScreenFlicker screenFlicker;
    private Vector3 hitRotation, endPosition;
    private float fallTimer, startRollTimer;
    private bool hitGround, finishAnimation;

    private void Start() {
        this.fallTimer = 0;
        this.startRollTimer = 0;
        this.finishAnimation = false;
        this.rigidBody = GetComponent<Rigidbody>();
        this.screenFlicker = FindObjectOfType<ScreenFlicker>();
        this.endPosition = initialPosition;
        endPosition.y = groundHeight;
        transform.position = initialPosition;
    }

    private void Update() {
        if (finishAnimation) return;

        //display logo
        if (fallTimer < logoDelay) fallTimer += Time.deltaTime;
        //fall to the ground
        else if (!hitGround) {
            rigidBody.isKinematic = false;
            rigidBody.AddForce(Vector3.down * 100);
            rigidBody.AddTorque(torqueForce * .1f);
        }
        //after hit ground
        else {
            if (startRollTimer < rollDelay) startRollTimer += Time.deltaTime;
            else {
                rigidBody.isKinematic = true;

                Quaternion initialRot = transform.rotation;
                Quaternion endRot = Quaternion.Euler(endRotation);
                transform.rotation = Quaternion.Lerp(initialRot, endRot, Time.deltaTime * rollSpeed);

                if (VectorSensitivity.EffectivelyReached(hitRotation, endRotation, .01f)) finishAnimation = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        screenFlicker.Flicker();
        hitGround = true;
    }
}