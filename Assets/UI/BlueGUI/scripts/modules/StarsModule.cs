using UnityEngine;
using System.Collections;

public class StarsModule : MonoBehaviour {

	public void Show(int starsAmount){
		SwitchBlock[] stars = gameObject.GetComponentsInChildren<SwitchBlock> ();
		for (int i = 0; i < starsAmount; i++) {
			stars [i].enabled = true;
		}
	}
	public void Hide(){
		SwitchBlock[] stars = gameObject.GetComponentsInChildren<SwitchBlock> ();
		for (int i = 0; i < stars.Length; i++) {
			stars [i].enabled = false;
		}
	}
}
