using UnityEngine;

public class Lighting : MonoBehaviour
{
    [SerializeField] private bool lighting = false;

    private bool prevLighting;
    private Light[] lights;
	private ReflectionProbe[] probes;
	private Renderer[] rend;

	void Awake() {
		lights = this.GetComponentsInChildren<Light> ();
		probes = this.GetComponentsInChildren<ReflectionProbe> ();
		rend = this.GetComponentsInChildren<Renderer> ();
	}

	// Use this for initialization
	void Start () {
        this.prevLighting = lighting;
    }

	// Update is called once per frame
	void Update () {
        if (lighting == prevLighting) return;

		if (lighting) {
			foreach (Light l in lights) l.enabled = true;
			foreach (ReflectionProbe r in probes) r.intensity = 1;
			foreach (Renderer r in rend) {
				foreach (Material m in r.materials) {
					if (m.name == "Bulb (Instance)") {
						m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
						m.SetColor("_EmissionColor", Color.white);
					}
				}
			}
		}
        else {
			foreach (Light l in lights) l.enabled = false;
			foreach (ReflectionProbe r in probes) r.intensity = 0;
			foreach (Renderer r in rend) {
				foreach (Material m in r.materials) {
					if (m.name == "Bulb (Instance)") {
						m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
						m.SetColor("_EmissionColor", Color.black);
					}
				}
			}
		}

        prevLighting = lighting;
    }
}
