﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDrop : MonoBehaviour{
	public int Price;
	protected bool IsWalled;
	protected bool HasToCheckWall;
	protected int Space;

	protected Transform tf;
	protected int Rotations = 0;
	protected bool isRotating = false;
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

	private Tweener rotTween;

	private cakeslice.Outline[] outlines;

	void Awake() {
		tf = transform;
		outlines = tf.GetComponentsInChildren<cakeslice.Outline> ();
		IsWalled = false;
		bought = true;
		OnAwake ();
	}

	protected virtual void OnAwake() {
		Space = 1;
	}

	void Start() {
		CheckCanPlace ();
		CheckRotation ();
		ToggleOutline (false);
		isRotating = false;
	}

	protected virtual void CheckCanPlace() {
		canPlace = true; 

		var v = G.Sys.tilemap.at (tf.position);
		var p = GetComponent<PodotactileTile> ();
		if (v.Count == 0 && ((p!=null && v [0].type != TileID.GROUND && !HasTileOfType(v, TileID.ESCALATOR) && !HasTileOfType(v, TileID.STAIRS) && !HasTileOfType(v, TileID.WAIT_ZONE)) || (p==null && v[0].type != TileID.GROUND)))
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
		if (!IsBought) {
			G.Sys.selectionManager.Hide (false);
			G.Sys.cameraController.CanDrag = false;
			Dragging = true;
		}
	}

	void Update() {
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
			CheckRotation ();
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
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
        G.Sys.tilemap.addSpaceUsed(-Space);
		Destroy (gameObject);
	}

	protected virtual void SendEvent() {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}

	public bool ValidateObject() {
		Rotations = 0;
		rotTween.Complete ();
		CheckCanPlace ();
		if(HasToCheckWall)
			CheckRotation ();

		if ((canPlace && !IsBought && G.Sys.gameManager.HaveEnoughMoney(Price)) || IsBought && canPlace) {
			G.Sys.cameraController.IsSelecting = false;
			if (!IsBought) {
                G.Sys.tilemap.addSpaceUsed(Space);
				G.Sys.gameManager.AddMoney (-Price);
				IsBought = true;
				var tile = GetComponent<ATile> ();
				tile.Register ();
			}
			G.Sys.selectionManager.Hide (true);
			CanDrag = false;
			SendEvent ();
			DeletePossibleEmptyWalls ();
			StartCoroutine (eventCoroutine ());
			G.Sys.audioManager.PlayConstruct ();
			return true;
		}
		return false;
	}

	IEnumerator eventCoroutine()
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
	}

	protected virtual void DeletePossibleEmptyWalls () {}
		
	public void RotateObject() {
		if (isRotating) {
			Rotations++;
		} else {
			isRotating = true;
			if (tf != null) {
				rotTween = tf.DORotate (new Vector3 (0, tf.rotation.eulerAngles.y + 90, 0), .3f, RotateMode.FastBeyond360).OnComplete (() => {
					if (Rotations > 0) {
						Rotations--;
						isRotating = false;
						RotateObject ();
					} else {
						isRotating = false;
					}
				});
			}
		}
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

	protected void RotateObject(float desiredAngle) {
		rotTween = tf.DORotate (new Vector3 (0, desiredAngle, 0), .3f, RotateMode.FastBeyond360);
	}

	public void ToggleOutline(bool b) {
		foreach(var o in outlines)
			o.enabled = b;
	}
}
