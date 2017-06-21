using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelerBehaviour : MonoBehaviour {
	public TravelerStats Stats;

	void Update() {
		transform.Translate (Vector3.forward * 5 * Time.deltaTime);
	}
}
