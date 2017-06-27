using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropPodotactile : DragAndDrop, IBeginDragHandler, IDragHandler, IEndDragHandler {


	void IBeginDragHandler.OnBeginDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Vector3 pos = ray.origin + (ray.direction * 1000);
		InstantiatedObject = Instantiate (VirtualObjectToDrop, pos, Quaternion.identity).transform;
	}

	void IDragHandler.OnDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			if (hit.transform.CompareTag ("Ground")) {
				Vector3 objPos = hit.transform.position;
				InstantiatedObject.position = new Vector3 (Mathf.RoundToInt (objPos.x), Mathf.RoundToInt (objPos.y), Mathf.RoundToInt (objPos.z));
			}
		} else {
			Vector3 pos = ray.origin + (ray.direction * 1000);
			InstantiatedObject.position = new Vector3 (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y), Mathf.RoundToInt (pos.z));
		}
	}

	void IEndDragHandler.OnEndDrag(PointerEventData data) {

		bool canPlace = true;

		//Case centrale
		var v = G.Sys.tilemap.at (InstantiatedObject.position);
		if (v.Count == 0 || v [0].type != TileID.GROUND || v[v.Count-1].type == TileID.PODOTACTILE)
			canPlace = false;

		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		List<RaycastResult> results = new List<RaycastResult>();

		pointerData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerData, results);

		if ((results.Count > 0 && results [0].gameObject == gameObject) || !canPlace) {
			Destroy (InstantiatedObject.gameObject);
		} else {
			Instantiate (ObjectToDrop, InstantiatedObject.position, InstantiatedObject.rotation);
			Destroy (InstantiatedObject.gameObject);
		}

		InstantiatedObject = null;
	}
}
