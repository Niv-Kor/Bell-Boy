using UnityEngine;

[RequireComponent(typeof(SoundMixer))]
public class MadSeagull : MonoBehaviour
{
    [Tooltip("Amount of seconds it takes the seagull to hit the camera.")]
    [SerializeField] private float hitAfter;

    [Tooltip("Hit points' offset relative to the camera.")]
    [SerializeField] private Vector3 camHitOffset;

    private static readonly string CALL_SFX = "seagull call";
    private static readonly string SLAM_SFX = "slam";

    private SoundMixer soundMixer;
    private ScreenFlicker screenFlicker;
    private Vector3 origin, target;
    private float timeLerped;
    private bool madeSound, hitCamera;

    private void Start() {
        this.soundMixer = GetComponent<SoundMixer>();
        this.screenFlicker = FindObjectOfType<ScreenFlicker>();
        this.origin = transform.position;
        Vector3 camPos = Camera.main.transform.position + camHitOffset;
        Vector3 direction = Vector3.Normalize(camPos - origin);
        float camDist = Vector3.Distance(origin, camPos);
        this.target = camPos + direction * camDist;
        this.timeLerped = 0;
        this.hitCamera = false;
        this.madeSound = false;

        transform.LookAt(camPos);
    }

    private void Update() {
        timeLerped += Time.deltaTime;
        transform.position = Vector3.Lerp(origin, target, timeLerped / (hitAfter * 2));

        //make seagull sound right before hitting the camera
        if (!madeSound && timeLerped >= hitAfter * .9f) {
            soundMixer.Activate(CALL_SFX);
            madeSound = true;
        }
        //make slam sound when hitting the camera
        else if (!hitCamera && timeLerped >= hitAfter) {
            soundMixer.Activate(SLAM_SFX);
            screenFlicker.Flicker();
            hitCamera = true;
        }
        //finish - destroy object
        else if (timeLerped >= hitAfter * 2) Destroy(gameObject);
    }
}