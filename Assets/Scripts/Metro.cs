using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Metro : MonoBehaviour {
	private Transform tf;
	private Vector3 positionToReach;
	private Material mat;

	void Start () {
		tf = transform;
		mat = tf.Find("Mesh").GetComponent<MeshRenderer> ().material;
		mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 0f);
		positionToReach = tf.position;
		ResetPos ();
		Invoke ("CallMetro", 5f);
	}

	void ResetPos() {
		tf.position = tf.position - tf.forward * 15f;
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
		Reappear ();
		tf.DOMove (positionToReach, G.Sys.constants.MetroComeTime).OnComplete(()=>{
			DOVirtual.DelayedCall(G.Sys.constants.MetroWaitTime, () => {
				Leave();
			});
		});
	}

	public void Leave() {
		Disappear ();
		tf.DOMove (tf.position + tf.forward * 15f, G.Sys.constants.MetroComeTime/2f).OnComplete(()=>{
			ResetPos();
		});
	}
}
