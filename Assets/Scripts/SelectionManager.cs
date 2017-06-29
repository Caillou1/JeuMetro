using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {
	private DragAndDrop obj;
	private Transform tf;

	void Awake() {
		G.Sys.selectionManager = this;
	}

	void Start() {
		tf = transform;
		Hide (false);
	}

	public void Show(DragAndDrop o) {
		if (o != null) {
			obj = o;
			tf.position = Camera.main.WorldToScreenPoint (obj.transform.position);
			for (int i = 0; i < tf.childCount; i++) {
				tf.GetChild (i).gameObject.SetActive (true);
			}
			obj.CanDrag = true;
			G.Sys.cameraController.IsSelecting = true;
		}
	}

	public void Move(Vector3 v) {
		obj.transform.position = new Vector3i (v).toVector3();
	}

	public void Hide(bool register) {
		if (obj != null && register) {
			obj.GetComponent<ATile> ().Register ();
			G.Sys.cameraController.IsSelecting = false;
		}
		obj = null;
		for (int i = 0; i < tf.childCount; i++) {
			tf.GetChild (i).gameObject.SetActive (false);
		}
	}

	public void Rotate() {
		var dade = obj.GetComponent<DragAndDropEscalator> ();
		if (dade == null) {
			obj.RotateObject ();
		} else {
			var e = obj.GetComponent<EscalatorTile> ();
			if (e.side == EscalatorSide.DOWN)
				e.side = EscalatorSide.UP;
			else
				e.side = EscalatorSide.DOWN;
		}
	}

	public void Validate() {
		obj.ValidateObject ();
	}

	public void Delete() {
		obj.DeleteObject ();
		Hide (false);
	}

	void Update() {
		if (obj != null) {
			tf.position = Camera.main.WorldToScreenPoint (obj.transform.position);
		}
	}
}
