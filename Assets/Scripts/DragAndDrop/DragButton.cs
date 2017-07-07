﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public GameObject ObjectToSpawn;

	private GameObject spawnedObject;
	private bool CanEndDrag;

	void Awake() {
		transform.Find ("Price").GetComponent<Text> ().text = ObjectToSpawn.GetComponent<DragAndDrop> ().Price.ToString();
	}

	void Spawn() {
		if (!G.Sys.cameraController.IsSelecting) {
			spawnedObject = Instantiate (ObjectToSpawn, new Vector3i (Camera.main.ScreenToWorldPoint(Input.mousePosition)).toVector3 (), Quaternion.identity);
			spawnedObject.GetComponent<ATile> ().Unregister ();
			var dad = spawnedObject.GetComponent<DragAndDrop> ();
			dad.DesactivateCollisions ();
			G.Sys.selectionManager.Show (dad);
		}
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		bool validated = G.Sys.selectionManager.Validate ();
		if (!validated) {
			G.Sys.selectionManager.Delete ();
		}

		if (!G.Sys.cameraController.IsSelecting) {	
			Spawn ();
			spawnedObject.GetComponent<DragAndDrop> ().StartDrag ();
			spawnedObject.GetComponent<DragAndDrop> ().IsBought = false;
			CanEndDrag = true;
		} else {
			CanEndDrag = false;
		}
	}

	void IDragHandler.OnDrag(PointerEventData eventData) {}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
		if (CanEndDrag) {
			List<RaycastResult> raycastResults = new List<RaycastResult> ();
			PointerEventData ped = new PointerEventData (EventSystem.current);
			ped.position = Input.mousePosition;
			EventSystem.current.RaycastAll (ped, raycastResults);

			if (raycastResults.Count > 0) {
				spawnedObject.GetComponent<DragAndDrop> ().DeleteObject ();
				G.Sys.cameraController.IsSelecting = false;
				G.Sys.cameraController.CanDrag = true;
			} else {
				spawnedObject.GetComponent<DragAndDrop> ().OnMouseUp ();
			}
		}
	}
}
