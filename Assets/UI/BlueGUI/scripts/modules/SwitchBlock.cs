using UnityEngine;
using System.Collections;

public class SwitchBlock : MonoBehaviour {

    public GameObject stateOn;
    public GameObject stateOff;
	public ParticleSystem particle;

    public bool enabled {
        get {
            return enabled;
        }
        set {
			stateOn.SetActive(value);
            stateOff.SetActive(!value);
			if(value)
				particle.Play();
        }
    }
}
