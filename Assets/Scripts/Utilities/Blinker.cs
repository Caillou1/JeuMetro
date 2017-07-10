using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Blinker : MonoBehaviour {

	public float BlinkSpeed;
	public Color BlinkColor;
	public bool BlinkAtStart;

	private Material mat;

	private IEnumerator BlinkRoutine;

	void Start () {
		mat = GetComponent<MeshRenderer> ().material;

		if (BlinkAtStart)
			StartBlink ();
	}

	public void StartBlink() {
		mat.EnableKeyword("_EMISSION");
		BlinkRoutine = Blink ();
		StartCoroutine (BlinkRoutine);
	}

	public void StopBlink() {
		mat.DisableKeyword ("_EMISSION");
		if (BlinkRoutine != null)
			StopCoroutine (BlinkRoutine);
	}

	IEnumerator Blink() {
		while (true) {
			float emission = Mathf.PingPong (Time.time * BlinkSpeed, 1.0f);

			Color finalColor = BlinkColor * Mathf.LinearToGammaSpace (emission);

			mat.SetColor ("_EmissionColor", finalColor);

			yield return new WaitForEndOfFrame ();
		}
	}
}
