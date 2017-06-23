using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropEscalator : DragAndDrop, IBeginDragHandler, IDragHandler, IEndDragHandler {


	void IBeginDragHandler.OnBeginDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Vector3 pos = ray.origin + (ray.direction * 1000);
		InstantiatedObject = Instantiate (ObjectToDrop, pos, Quaternion.identity).transform;
	}

	void IDragHandler.OnDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		List<Orientation> PossibleOrientations = new List<Orientation> ();

		var v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 1, 2));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.DOWN);
		
		v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 1, -2));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.UP);
		
		v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(-2, 1, 0));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.RIGHT);
		
		v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(2, 1, 0));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.LEFT);
			
		if(PossibleOrientations.Count > 0 && !PossibleOrientations.Contains(Orienter.angleToOrientation(InstantiatedObject.rotation.eulerAngles.y))) {
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);
			if(InstantiatedObject.rotation.eulerAngles.y != desiredAngle)
				RotateObject (desiredAngle);
		}

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

	//Le sens de l'escalator est définit par le bas de celui-ci
	void IEndDragHandler.OnEndDrag(PointerEventData data) {
		Orientation or = Orienter.angleToOrientation (InstantiatedObject.rotation.eulerAngles.y);
		Debug.Log (or);
		bool canPlace = true;

		var v = G.Sys.tilemap.at (InstantiatedObject.position);
		if (v.Count == 0 || v [0].type != TileID.GROUND)
			canPlace = false;

		if (or == Orientation.UP || or == Orientation.DOWN) {
			v = G.Sys.tilemap.at (InstantiatedObject.position + Vector3.forward);
			if (v.Count == 0 || v [0].type != TileID.GROUND)
				canPlace = false;

			v = G.Sys.tilemap.at (InstantiatedObject.position + Vector3.back);
			if (v.Count == 0 || v [0].type != TileID.GROUND)
				canPlace = false;

			if (or == Orientation.UP) {
				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 0, 2));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;

				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 1, -2));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;
			} else {
				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 1, 2));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;

				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 0, -2));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;
			}
		} else if (or == Orientation.LEFT || or == Orientation.RIGHT) {
			v = G.Sys.tilemap.at (InstantiatedObject.position + Vector3.left);
			if (v.Count == 0 || v [0].type != TileID.GROUND)
				canPlace = false;

			v = G.Sys.tilemap.at (InstantiatedObject.position + Vector3.right);
			if (v.Count == 0 || v [0].type != TileID.GROUND)
				canPlace = false;

			if (or == Orientation.LEFT) {
				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(-2, 0, 0));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;

				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(2, 1, 0));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;
			} else {
				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(-2, 1, 0));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;

				v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(2, 0, 0));
				if (v.Count == 0 || v [0].type != TileID.GROUND)
					canPlace = false;
			}
		}





		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		List<RaycastResult> results = new List<RaycastResult>();
		
		pointerData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerData, results);

		if ((results.Count > 0 && results [0].gameObject == gameObject) || !canPlace) {
			Destroy (InstantiatedObject.gameObject);
		} else {
			var col = InstantiatedObject.GetComponent<Collider> ();
			if(col!=null)
				col.enabled = true;
			foreach (var c in transform.GetComponentsInChildren<Collider>()) {
				c.enabled = true;
			}
		}

		InstantiatedObject = null;
	}

	void RotateObject(float desiredAngle) {
		InstantiatedObject.DORotate (new Vector3 (0, desiredAngle, 0), .3f, RotateMode.FastBeyond360);
	}
}
