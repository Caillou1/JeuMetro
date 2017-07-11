using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectionManager : MonoBehaviour {
	private DragAndDrop obj;
	private Transform tf;
	private Transform DragSpriteTransform;
	private Image DragSprite;
	private bool ShowingSprite;

	void Awake() {
		G.Sys.selectionManager = this;
		ShowingSprite = false;
	}

	void Start() {
		tf = transform;
		Hide (false);
		DragSpriteTransform = GameObject.Find ("DragSprite").transform;
		DragSprite = DragSpriteTransform.GetComponent<Image> ();
	}

	public void ShowSprite(Sprite s) {
		DragSprite.sprite = s;
		DragSprite.color = new Color (DragSprite.color.r, DragSprite.color.g, DragSprite.color.b, 1f);
		ShowingSprite = true;
	}

	public void HideSprite() {
		DragSprite.color = new Color (DragSprite.color.r, DragSprite.color.g, DragSprite.color.b, 0f);
		ShowingSprite = false;
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
			obj.ActivateCollisions ();
		}
	}

	public void Move(Vector3 v) {
		obj.transform.position = new Vector3i (v).toVector3();
	}

	public void Hide(bool register) {
		if (obj != null) {
			obj.DesactivateCollisions ();
			if (register) {
				obj.GetComponent<ATile> ().Register ();
				G.Sys.cameraController.IsSelecting = false;
			}
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
				e.SetSide (EscalatorSide.UP);
			else
				e.SetSide (EscalatorSide.DOWN);
		}
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

	void Update() {
		if (obj != null) {
			tf.position = Camera.main.WorldToScreenPoint (obj.transform.position);
		}

		if (ShowingSprite) {
			DragSpriteTransform.position = Camera.main.WorldToScreenPoint (Input.mousePosition);
		}
	}
}
