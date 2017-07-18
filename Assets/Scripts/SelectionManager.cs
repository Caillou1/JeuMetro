using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour {
	private DragAndDrop obj;
	private DragAndDropEntity ent;
	private Transform tf;

	private GameObject validate;
	private GameObject rotate;
	private GameObject delete;
	private GameObject changeside;

	void Awake() {
		G.Sys.selectionManager = this;
		tf = transform;

		validate = tf.Find ("validate").gameObject;
		rotate = tf.Find ("rotate").gameObject;
		delete = tf.Find ("delete").gameObject;
		changeside = tf.Find ("changeside").gameObject;
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
			}

			obj.CanDrag = true;
			G.Sys.cameraController.IsSelecting = true;
			obj.DesactivateCollisions ();
		}
	}

	public void Show(DragAndDropEntity e) {
		if (e != null) {
			ent = e;
			ent.IsSelected = true;
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (ent.transform.position);

			validate.SetActive (true);
			delete.SetActive (true);

			ent.CanDrag = true;
			G.Sys.cameraController.IsSelecting = true;
			//ent.DesactivateCollisions ();
		}
	}

	public void Hide(bool register, bool isEntity) {
		if (isEntity) {
			//ent.ActivateCollisions ();
			if (register) {
				ent.GetComponent<AEntity> ().enabled = true;
				G.Sys.cameraController.IsSelecting = false;
				ent.IsSelected = false;
			}

			ent = null;

			validate.SetActive (false);
			rotate.SetActive (false);
			delete.SetActive (false);
			changeside.SetActive (false);
		} else {
			Hide (register);
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
		if (ent != null)
			ent.ValidateObject ();
	}

	public bool Validate() {
		if (obj != null) {
			return obj.ValidateObject ();
		}
		if (ent != null) {
			return ent.ValidateObject ();
		}
		return false;
	}

	public void Delete() {
		G.Sys.cameraController.IsSelecting = false;
		if (obj != null) {
			obj.DeleteObject ();
			Hide (false);
		}
		if (ent != null) {
			ent.DeleteObject ();
			Hide (false, true);
		}
	}

	public void ChangeSide() {
		var e = obj.GetComponent<EscalatorTile> ();
		if (e != null) {
			if (e.side == EscalatorSide.DOWN) {
				e.SetSide (EscalatorSide.UP);
			} else {
				e.SetSide (EscalatorSide.DOWN);
			}
		}
	}

	void Update() {
		if (obj != null) {
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (obj.transform.position);
		}
		if (ent != null) {
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (ent.transform.position);
		}
	}
}
