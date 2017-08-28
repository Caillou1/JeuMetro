using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Blinker : MonoBehaviour {
	public float BlinkTime;
	public float AlphaStart;
	public float AlphaEnd;

	private Material mat;

	void Start () {
		mat = GetComponent<MeshRenderer> ().material;

		Blink ();
	}

	void Blink() {
		mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, AlphaStart);
		DOVirtual.Float (AlphaStart, AlphaEnd, BlinkTime / 2f, (float x) => {
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, x);
		}).OnComplete(() => { 
			DOVirtual.Float (AlphaEnd, AlphaStart, BlinkTime / 2f, (float x) => {
				mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, x);
			}).OnComplete(() => { 
				Blink();
			});
		});
	}

	void OnDestroy() {
		DOTween.KillAll (false);
	}
}
