using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour {
	public GameObject ArrowHead;
	public GameObject ArrowBody;
	public Transform[] Waypoints;
	public float TimeToCompleteAnimation = 10;
	public float ScaleDownTime = 5;
	public float TimeBetweenSquares = .25f;
	public bool MoveAtStart = true;
	public bool Loops = true;


	private Transform tf;
	private Vector3[] points;
	private IEnumerator tailroutine;
	private Tweener pathTween;
	private GameObject head;
	private Vector3 origin;

	void Start () {
		tf = transform;
		origin = tf.position;

		points = new Vector3[Waypoints.Length];
		for (int i = 0; i < Waypoints.Length; i++) {
			points [i] = Waypoints [i].position;
		}

		if (MoveAtStart)
			StartMove ();
	}

	public void StartMove() {
		head = Instantiate (ArrowHead, tf.position, Quaternion.identity, tf);
		tailroutine = SpawnTail ();
		StartCoroutine (tailroutine);
		pathTween = tf.DOPath (points, TimeToCompleteAnimation, PathType.CatmullRom, PathMode.Full3D, 5, new Color (Random.value, Random.value, Random.value, 1f)).SetLookAt (.01f , Vector3.forward, Vector3.up);
		pathTween.OnComplete (() => {
			StopMove();
			if(Loops)
				StartMove();
		});
	}

	public void StopMove() {
		tf.position = origin;

		if (pathTween != null)
			pathTween.Pause ();

		if (tailroutine != null)
			StopCoroutine (tailroutine);

		if(head != null)
			Destroy (head);
	}

	IEnumerator SpawnTail() {
		while (true) {
			yield return new WaitForSeconds (TimeBetweenSquares);
			var obj = Instantiate (ArrowBody, tf.position, Quaternion.identity);
			obj.transform.DOScale (Vector3.zero, ScaleDownTime).OnComplete (() => Destroy(obj));
		}
	}
}
