﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropEntity : MonoBehaviour{
	public int Price;

	protected Transform tf;
	protected Rigidbody rb;
	protected bool canPlace;
	[HideInInspector]
	public bool CanDrag = false;
	[HideInInspector]
	public bool IsSelected = false;
	protected bool Dragging = false;
	protected bool bought;
	public bool IsBought {
		get{ return bought; }
		set{
			bought = value;
		}
	}

	void Awake() {
		bought = true;
		tf = transform;
		rb = GetComponent<Rigidbody> ();
	}

	void Start() {
		CheckCanPlace ();
	}

	protected virtual void CheckCanPlace() {
		canPlace = true; 

		var v = G.Sys.tilemap.at (tf.position);
		var p = GetComponent<PodotactileTile> ();

		if (v.Count == 0 || (p==null && v[0].type != TileID.GROUND))
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
			G.Sys.selectionManager.Hide (false, true);
			G.Sys.cameraController.CanDrag = false;
			Dragging = true;
		}
	}

	void Update() {
		if(IsSelected)
			rb.velocity = Vector3.zero;
		if (Dragging && CanDrag && !IsBought) {
			Ray ray = G.Sys.MainCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] hits;
			hits = Physics.RaycastAll (ray);
			RaycastHit hit = FindGround (hits);

			if (hit.transform != null) {
				Vector3 objPos = hit.transform.position;
				tf.position = new Vector3 (Mathf.RoundToInt (objPos.x), Mathf.RoundToInt (objPos.y), Mathf.RoundToInt (objPos.z));
			} else {
				Vector3 pos = ray.origin + (ray.direction * 1000);
				tf.position = new Vector3 (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y), Mathf.RoundToInt (pos.z));
			}

			CheckCanPlace ();
		}
	}

	RaycastHit FindGround(RaycastHit[] hits) {
		foreach (var h in hits) {
			if (h.transform.CompareTag ("Ground"))
				return h;
		}

		return new RaycastHit ();
	}

	public void OnMouseUp() {
		if (CanDrag) {
			G.Sys.selectionManager.Show (this);
			G.Sys.cameraController.CanDrag = true;

			Dragging = false;
		}
	}

	public void DeleteObject() {
		Destroy (gameObject);
	}

	public bool ValidateObject() {
		CheckCanPlace ();

		if ((canPlace && !IsBought && G.Sys.gameManager.HaveEnoughMoney(Price)) || IsBought && canPlace) {
			G.Sys.cameraController.IsSelecting = false;
			if (!IsBought) {
				G.Sys.gameManager.AddMoney (-Price);
				IsBought = true;
			}
			G.Sys.selectionManager.Hide (true, true);
			CanDrag = false;
			GetComponent<AEntity> ().enabled = true;
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
}