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
		InstantiatedObject = Instantiate (VirtualObjectToDrop, pos, Quaternion.identity).transform;
	}

	void IDragHandler.OnDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		List<Orientation> PossibleOrientations = new List<Orientation> ();

		var v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 2, 2));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.DOWN);
		
		v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(0, 2, -2));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.UP);
		
		v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(-2, 2, 0));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.RIGHT);
		
		v = G.Sys.tilemap.at (InstantiatedObject.position + new Vector3(2, 2, 0));
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

		bool canPlace = true;
		Vector3 dir = Orienter.orientationToDir3 (or);

		//Case centrale
		var v = G.Sys.tilemap.at (InstantiatedObject.position);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(InstantiatedObject.position, TileID.ESCALATOR).Count > 0 || G.Sys.tilemap.at (InstantiatedObject.position + Vector3.up * 2).Count > 0)
			canPlace = false;

		//Case  plus basse
		v = G.Sys.tilemap.at (InstantiatedObject.position + dir);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(InstantiatedObject.position + dir, TileID.ESCALATOR).Count > 0 || G.Sys.tilemap.at (InstantiatedObject.position + dir + Vector3.up * 2).Count > 0)
			canPlace = false;

		//Case plus haute
		v = G.Sys.tilemap.at (InstantiatedObject.position - dir);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(InstantiatedObject.position - dir, TileID.ESCALATOR).Count > 0 || G.Sys.tilemap.at (InstantiatedObject.position - dir + Vector3.up * 2).Count > 0)
			canPlace = false;

		//Case en bas de l'escalator
		v = G.Sys.tilemap.at (InstantiatedObject.position + 2 * dir);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.at (InstantiatedObject.position + 2 * dir + Vector3.up * 2).Count > 0)
			canPlace = false;

		//Case en haut de l'escalator
		v = G.Sys.tilemap.at (InstantiatedObject.position - 2 * dir + Vector3.up * 2);
		if (v.Count == 0 || v [0].type != TileID.GROUND)
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
