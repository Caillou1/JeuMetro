using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDrop : MonoBehaviour{
	public int Price;

	protected Transform tf;
	protected int Rotations = 0;
	protected bool isRotating = false;
	protected bool canPlace;
	[HideInInspector]
	public bool CanDrag = false;
	protected bool Dragging = false;
	public bool IsBought;

	void Awake() {
		tf = transform;
		isRotating = false;
		CheckCanPlace ();
		CheckRotation ();
		IsBought = true;
	}

	protected virtual void CheckCanPlace() {
		canPlace = true; 

		var v = G.Sys.tilemap.at (tf.position);
		var p = GetComponent<PodotactileTile> ();
		if (v.Count == 0 || (p!=null && v [0].type != TileID.GROUND && !HasTileOfType(v, TileID.ESCALATOR) && !HasTileOfType(v, TileID.STAIRS)) || (p==null && v[0].type != TileID.GROUND))
			canPlace = false;
	}

	protected bool HasTileOfType(List<ATile> list, TileID type) {
		foreach (var l in list) {
			if (l.type == type) {
				return true;
			}
		}
		return false;
	}

	protected virtual void CheckRotation() {}

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
		G.Sys.selectionManager.Hide (false);
		G.Sys.cameraController.CanDrag = false;
		Dragging = true;
	}

	void Update() {
		if (Dragging && CanDrag) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] hit;
			hit = Physics.RaycastAll (ray);

			if (hit.Length>0) {
				if (hit[0].transform.CompareTag ("Ground")) {
					Vector3 objPos = hit[0].transform.position;
					tf.position = new Vector3 (Mathf.RoundToInt (objPos.x), Mathf.RoundToInt (objPos.y), Mathf.RoundToInt (objPos.z));
				} else if (hit[0].transform.gameObject == gameObject && hit.Length > 1 && hit[1].transform.CompareTag ("Ground")) {
					Vector3 objPos = hit[1].transform.position;
					tf.position = new Vector3 (Mathf.RoundToInt (objPos.x), Mathf.RoundToInt (objPos.y), Mathf.RoundToInt (objPos.z));
				}
			} else {
				Vector3 pos = ray.origin + (ray.direction * 1000);
				tf.position = new Vector3 (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y), Mathf.RoundToInt (pos.z));
			}

			CheckCanPlace ();
			CheckRotation ();
		}
	}

	public void OnMouseUp() {
		if (CanDrag) {
			G.Sys.selectionManager.Show (this);
			G.Sys.cameraController.CanDrag = true;

			Dragging = false;
		}
	}

	public void DeleteObject() {
		G.Sys.gameManager.AddMoney (Price / 2);
		Destroy (gameObject);
	}

	protected virtual void SendEvent() {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}

	public void ValidateObject() {
		CheckCanPlace ();
		CheckRotation ();
		if (canPlace && (G.Sys.gameManager.HaveEnoughMoney(Price) || IsBought)) {
			G.Sys.cameraController.IsSelecting = false;
			if (!IsBought) {
				G.Sys.gameManager.AddMoney (-Price);
				IsBought = true;
			}
			G.Sys.selectionManager.Hide (true);
			GetComponent<ATile> ().Register ();
			CanDrag = false;
			SendEvent ();
			ActivateCollisions ();
		}
	}
		
	public void RotateObject() {
		if (isRotating) {
			Rotations++;
		} else {
			isRotating = true;
			if(tf != null) tf.DORotate (new Vector3 (0, tf.rotation.eulerAngles.y + 90, 0), .3f, RotateMode.FastBeyond360).OnComplete(() => {
				if(Rotations > 0) {
					Rotations--;
					isRotating = false;
					RotateObject();
				} else {
					isRotating = false;
				}
			});
		}
	}

	public void DesactivateCollisions() {
		foreach (var c in GetComponentsInChildren<Collider>())
			if(c.transform.parent == tf)
				c.enabled = false;
	}

	public void ActivateCollisions() {
		foreach (var c in GetComponentsInChildren<Collider>())
			if(c.transform.parent == tf)
				c.enabled = true;
	}

	protected void RotateObject(float desiredAngle) {
		tf.DORotate (new Vector3 (0, desiredAngle, 0), .3f, RotateMode.FastBeyond360);
	}
}
