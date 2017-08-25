using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class DragAndDropEntity : MonoBehaviour{
	public int Price;

	protected Transform tf;
	protected bool canPlace;
	[HideInInspector]
	public bool CanDrag = false;
	protected bool Dragging = false;
	protected bool bought;
	public bool IsBought {
		get{ return bought; }
		set{
			bought = value;
		}
	}

	public bool CanPlace {
		get {
			return canPlace;
		}
	}

	protected bool isSelected;
	public bool IsSelected {
		get{
			return isSelected;
		}
		set {
			isSelected = value;
		}
	}
	private cakeslice.Outline[] outlines;

	private Vector3 originalScale;

	void Awake() {
		bought = true;
		tf = transform;
		outlines = tf.GetComponentsInChildren<cakeslice.Outline> ();
	}

	void Start() {
		originalScale = tf.localScale;
		CheckCanPlace ();
	}

	protected virtual void CheckCanPlace() {
		canPlace = true; 

		var v = G.Sys.tilemap.at (tf.position);
		var p = GetComponent<PodotactileTile> ();

		if (v.Count == 0 || (p==null && v[0].type != TileID.GROUND && !HasTileOfType(v, TileID.WAIT_ZONE)))
			canPlace = false;
	}

	protected void OnMouseDown() {
		List<RaycastResult> raycastResults = new List<RaycastResult> ();
		PointerEventData ped = new PointerEventData (EventSystem.current);
		ped.position = Input.mousePosition;
		EventSystem.current.RaycastAll (ped, raycastResults);

		if (CanDrag && raycastResults.Count == 0) {
			StartDrag ();
		}
	}

	public void StartDrag() {
		GetComponent<AEntity> ().enabled = false;
		if (!IsBought) {
			G.Sys.selectionManager.Hide (false, true, false);
			G.Sys.cameraController.CanDrag = false;
			Dragging = true;
		}
	}

	protected bool HasTileOfType(List<ATile> list, TileID type) {
		foreach (var l in list) {
			if (l.type == type) {
				return true;
			}
		}
		return false;
	}

	void Update() {
		if (Dragging && CanDrag && !IsBought) {
			Ray ray = G.Sys.MainCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] hits;
			hits = Physics.RaycastAll (ray);
			RaycastHit hit = FindGround (hits);

			if (hit.transform != null && !G.Sys.cameraController.IsOnUI ()) {
				Vector3 objPos = hit.transform.position;
				tf.position = new Vector3 (Mathf.RoundToInt (objPos.x), Mathf.RoundToInt (objPos.y), Mathf.RoundToInt (objPos.z));
			} else if (hit.transform != null) {
				Vector3 objPos = hit.point;
				tf.position = new Vector3 (objPos.x, Mathf.RoundToInt (objPos.y), objPos.z);
			} else {
				tf.position = new Vector3i (tf.position).toVector3 ();
			}

			CheckCanPlace ();

			if (canPlace && !G.Sys.cameraController.IsOnUI ()) {
				cakeslice.OutlineEffect.Instance.lineColor0 = Color.green;
			} else {
				cakeslice.OutlineEffect.Instance.lineColor0 = Color.red;
			}

			if (G.Sys.cameraController.IsOnUI ()) {
				tf.localScale = originalScale / 2;
			} else {
				tf.localScale = originalScale;
			}
		} else if (isSelected && !IsBought) {
			CheckCanPlace ();

			if (canPlace) {
				cakeslice.OutlineEffect.Instance.lineColor0 = Color.green;
			} else {
				cakeslice.OutlineEffect.Instance.lineColor0 = Color.red;
			}
		} else if (isSelected && IsBought) {
			cakeslice.OutlineEffect.Instance.lineColor0 = Color.yellow;
		}
	}

	RaycastHit FindGround(RaycastHit[] hits) {
		hits = hits.OrderBy (h => h.distance).ToArray();

		foreach (var h in hits) {
			if (h.transform.CompareTag ("Ground"))
				return h;
		}

		return new RaycastHit ();
	}

	public void OnMouseUp() {
		if (CanDrag) {
			G.Sys.selectionManager.Show (this, false);
			G.Sys.cameraController.CanDrag = true;

			Dragging = false;
		}
	}

	public void DeleteObject() {
		Destroy (gameObject);
	}

	public bool ValidateObject() {
		tf.localScale = originalScale;
		CheckCanPlace ();

		if ((canPlace && !IsBought && G.Sys.gameManager.HaveEnoughMoney(Price)) || IsBought && canPlace) {
			G.Sys.cameraController.IsSelecting = false;
			if (!IsBought) {
				G.Sys.gameManager.AddMoney (-Price);
				IsBought = true;
			}
			CanDrag = false;
			var e = GetComponent<AEntity> ();
			e.enabled = true;
			e.EnableAgent ();
			G.Sys.selectionManager.Hide (true, true, true);
			return true;
		}
		return false;
	}

	public void DesactivateCollisions() {
		foreach (var c in GetComponentsInChildren<Collider>())
			if (c.transform != tf)
				c.enabled = false;
	}

	public void ActivateCollisions() {
		foreach (var c in GetComponentsInChildren<Collider>())
				c.enabled = true;
	}

	public void ToggleOutline(bool b) {
		if (isSelected && IsBought) {
			cakeslice.OutlineEffect.Instance.lineColor0 = Color.yellow;
		}

		if(outlines == null)
			outlines = transform.GetComponentsInChildren<cakeslice.Outline> ();

		foreach(var o in outlines)
			o.enabled = b;
	}
}
