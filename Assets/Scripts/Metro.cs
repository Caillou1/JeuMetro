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

	void Awake()
	{
		G.Sys.registerMetro (this);

		tf = transform;
		positionToReach = tf.position;
		startPosition = tf.position - tf.forward * moveOffset;
		endPosition = tf.position + tf.forward * moveOffset;

		tf.position = startPosition;
	}

	void OnDestroy()
	{
		G.Sys.removeMetro (this);
	}

	void ResetPos() {
		tf.position = startPosition;
	}

	public void CallMetro() {
		ResetPos ();
		G.Sys.audioManager.PlayTrainStop ();
		tf.DOMove (positionToReach, G.Sys.constants.MetroComeTime).SetEase(Ease.OutQuad).OnComplete(()=>{
			EnableTiles(true);
			DOVirtual.DelayedCall(G.Sys.constants.MetroWaitTime, () => {
				Leave();
			});
		});
	}

	public void Leave() {
		EnableTiles (false);
		G.Sys.audioManager.PlayTrainStart ();
		tf.DOMove (endPosition, G.Sys.constants.MetroComeTime).SetEase(Ease.InExpo);
	}

	void EnableTiles(bool value)
	{
		foreach(var t in OutTiles)
			t.tileEnabled = value;
	}
}
