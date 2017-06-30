using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDrop : MonoBehaviour{
	protected Transform tf;
	protected int Rotations = 0;
	protected bool isRotating = false;
	protected bool canPlace;
	[HideInInspector]
	public bool CanDrag = false;
	protected bool Dragging = false;

	void Awake() {
		tf = transform;
		isRotating = false;
		CheckCanPlace ();
		CheckRotation ();
	}

	protected virtual void CheckCanPlace() {
		canPlace = true; 

		var v = G.Sys.tilemap.at (tf.position);
		if (v.Count == 0 || (v [0].type != TileID.GROUND && v.Count == 1))
			canPlace = false;
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

		foreach (var c in tf.GetComponentsInChildren<Collider>()) {
			c.enabled = false;
		}
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

			foreach (var c in tf.GetComponentsInChildren<Collider>()) {
				c.enabled = true;
			}

			Dragging = false;
		}
	}

	public void DeleteObject() {
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
		if (canPlace) {
			G.Sys.cameraController.IsSelecting = false;
			G.Sys.selectionManager.Hide (true);
			CanDrag = false;
			SendEvent ();
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

	protected void RotateObject(float desiredAngle) {
		tf.DORotate (new Vector3 (0, desiredAngle, 0), .3f, RotateMode.FastBeyond360);
	}
}
