using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour {
	private DragAndDrop obj;
	private Transform tf;

	private GameObject validate;
	private GameObject rotate;
	private GameObject delete;
	private GameObject changeside;
	private Text changeSideText;

	void Awake() {
		G.Sys.selectionManager = this;
		tf = transform;

		validate = tf.Find ("validate").gameObject;
		rotate = tf.Find ("rotate").gameObject;
		delete = tf.Find ("delete").gameObject;
		changeside = tf.Find ("changeside").gameObject;
		changeSideText = changeside.transform.Find ("Text").GetComponent<Text>();
	}

	void Start() {
		Hide (false);
	}

	public void Show(DragAndDrop o) {
		if (o != null) {
			obj = o;
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (obj.transform.position);

			validate.SetActive (true);
			rotate.SetActive (true);
			delete.SetActive (true);

			var e = obj.GetComponent<EscalatorTile> ();
			if (e != null) {
				changeside.SetActive (true);
				if (e.side == EscalatorSide.DOWN) {
					changeSideText.text = "D";
				} else {
					changeSideText.text = "U";
				}
			}

			obj.CanDrag = true;
			G.Sys.cameraController.IsSelecting = true;
			obj.DesactivateCollisions ();
		}
	}

	public void Move(Vector3 v) {
		obj.transform.position = new Vector3i (v).toVector3();
	}

	public void Hide(bool register) {
		if (obj != null) {
			obj.ActivateCollisions ();
			if (register) {
				obj.GetComponent<ATile> ().Register ();
				G.Sys.cameraController.IsSelecting = false;
			}
		}
		obj = null;

		validate.SetActive (false);
		rotate.SetActive (false);
		delete.SetActive (false);
		changeside.SetActive (false);
	}

	public void Rotate() {
		obj.RotateObject ();
	}

	public void ValidateNoReturn() {
		if (obj != null)
			obj.ValidateObject ();
	}

	public bool Validate() {
		if (obj != null) {
			return obj.ValidateObject ();
		}
		return false;
	}

	public void Delete() {
		G.Sys.cameraController.IsSelecting = false;
		if (obj != null) {
			obj.DeleteObject ();
		}
		Hide (false);
	}

	public void ChangeSide() {
		var e = obj.GetComponent<EscalatorTile> ();
		if (e != null) {
			if (e.side == EscalatorSide.DOWN) {
				e.SetSide (EscalatorSide.UP);
				changeSideText.text = "U";
			} else {
				e.SetSide (EscalatorSide.DOWN);
				changeSideText.text = "D";
			}
		}
	}

	void Update() {
		if (obj != null) {
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (obj.transform.position);
		}
	}
}
