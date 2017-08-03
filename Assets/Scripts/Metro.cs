using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Metro : MonoBehaviour {

	public List<MetroTile> OutTiles;
	[Tooltip("Déplacement du metro quand il arrive et part")]
	public float moveOffset = 12.5f;

	private Transform tf;
	private Vector3 positionToReach;
	private Vector3 startPosition;
	private Vector3 endPosition;

	void Start () {
		tf = transform;
		positionToReach = tf.position;
		startPosition = tf.position - tf.forward * moveOffset;
		endPosition = tf.position + tf.forward * moveOffset;

		tf.position = startPosition;
	}

	float time=15;
	void Update()
	{
		time += Time.deltaTime;
		if (time > 20) {
			CallMetro ();
			time = 0;
		}
	}

	void ResetPos() {
		tf.position = startPosition;
	}

	public void CallMetro() {
		ResetPos ();
		tf.DOMove (positionToReach, G.Sys.constants.MetroComeTime).OnComplete(()=>{
			EnableTiles(true);
			DOVirtual.DelayedCall(G.Sys.constants.MetroWaitTime, () => {
				Leave();
			});
		});
	}

	public void Leave() {
		EnableTiles (false);
		tf.DOMove (endPosition, G.Sys.constants.MetroComeTime / 2f);
	}

	void EnableTiles(bool value)
	{
		foreach(var t in OutTiles)
			t.tileEnabled = value;
	}
}
