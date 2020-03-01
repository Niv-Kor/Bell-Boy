using Constants;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class CameraIntro : MonoBehaviour
{
    [Header("Transform")]

    [Tooltip("The position of the camera when displaying the logo in the sky.")]
    [SerializeField] private Vector3 initialPosition;

    [Tooltip("The rotation of the camera after hitting the ground.")]
    [SerializeField] private Vector3 endRotation;

    [Tooltip("The speed of the camera's fall.")]
    [SerializeField] private float groundHeight = 0;

    [Header("Force")]

    [Tooltip("The force that's applied to the camera from the top before it falls.")]
    [SerializeField] private float fallForce = 50;

    [Tooltip("The direction from which the camera is pushed down.")]
    [SerializeField] private Vector3 torqueForce;

    [Header("Timing")]

    [Tooltip("The amount of time (in seconds) until the camera falls down.")]
    [SerializeField] private float logoDelay = 5;

    [Tooltip("The time it takes the camera to start rolling after hitting the ground (in seconds).")]
    [SerializeField] private float rollDelay = 1;

    [Tooltip("The of the camera's roll on the ground.")]
    [SerializeField] private float rollSpeed = 1;

    private static readonly int OVERLAP_RESULTS = 32;

    private BGM bgm;
    private Rigidbody rigidBody;
    private Collider[] overlapRes;
    private ScreenFlicker screenFlicker;
    private StartMenuButtons startMenuButtons;
    private Vector3 hitRotation, endPosition, boxExtent;
    private float fallTimer, startRollTimer;
    private bool hitGround, finishAnimation, startRoll;

    private void Awake() {
        transform.position = initialPosition;
    }

    private void Start() {
        this.fallTimer = 0;
        this.startRollTimer = 0;
        this.startRoll = false;
        this.finishAnimation = false;
        this.bgm = FindObjectOfType<BGM>();
        this.rigidBody = GetComponent<Rigidbody>();
        this.overlapRes = new Collider[OVERLAP_RESULTS];
        this.screenFlicker = FindObjectOfType<ScreenFlicker>();
        this.startMenuButtons = FindObjectOfType<StartMenuButtons>();
        this.endPosition = initialPosition;
        endPosition.y = groundHeight;

        BoxCollider boxCol = GetComponent<BoxCollider>();
        this.boxExtent = boxCol.bounds.extents;
    }

    private void Update() {
        if (finishAnimation) return;

        //display logo
        if (fallTimer < logoDelay) fallTimer += Time.deltaTime;
        //fall to the ground
        else if (!hitGround) {
            if (!bgm.IsPlaying()) bgm.Play();

            rigidBody.isKinematic = false;
            rigidBody.AddForce(Vector3.down * fallForce);
            rigidBody.AddTorque(torqueForce * .1f);
        }
        //after hit ground
        else {
            if (!startRoll) {
                //check amount of contacts between the camera and the ground
                int overlaps = Physics.OverlapBoxNonAlloc(transform.position, boxExtent, overlapRes, transform.rotation, Layers.GROUND);

                if (startRollTimer < rollDelay) startRollTimer += Time.deltaTime;
                else if (overlaps > 0) {
                    startMenuButtons.FadeIn();
                    startRoll = true; //camera touches ground
                }
            }
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