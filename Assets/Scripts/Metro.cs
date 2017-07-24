using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Metro : MonoBehaviour {
	private Transform tf;
	private Vector3 positionToReach;
	private Vector3 startPosition;
	private Material mat;
	private ExitsTile[] exitsTiles;

	void Start () {
		tf = transform;
		mat = tf.Find("Mesh").GetComponent<MeshRenderer> ().material;
		mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 0f);
		positionToReach = tf.position;
		startPosition = tf.position - tf.forward * 15f;
		exitsTiles = GetComponentsInChildren<ExitsTile> ();
		foreach (var et in exitsTiles) {
			et.Unregister ();
		}
	}

	void ResetPos() {
		tf.position = startPosition;
	}

	void Disappear() {
		DOVirtual.Float (1f, 0f, 2f, (float a) => {
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, a);
		});
	}

	void Reappear () {
		DOVirtual.Float (0f, 1f, 2f, (float a) => {
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, a);
		});
	}

	public void CallMetro() {
		ResetPos ();
		Reappear ();
		tf.DOMove (positionToReach, G.Sys.constants.MetroComeTime).OnComplete(()=>{
			foreach (var et in exitsTiles) {
				et.Register ();
			}
			DOVirtual.DelayedCall(G.Sys.constants.MetroWaitTime, () => {
				Leave();
			});
		});
	}

	public void Leave() {
		foreach (var et in exitsTiles) {
			et.Unregister ();
		}
		Disappear ();
		tf.DOMove (tf.position + tf.forward * 15f, G.Sys.constants.MetroComeTime / 2f);
	}
}
