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

    void Blink()
    {
        Color c = mat.color;
        DOVirtual.Float(AlphaStart, AlphaEnd, BlinkTime / 2.0f, (float x) =>
        {
            mat.color = new Color(c.r, c.g, c.b, x);
        }).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InCubic);
	}

	void OnDestroy() {
		DOTween.KillAll (false);
	}
}
