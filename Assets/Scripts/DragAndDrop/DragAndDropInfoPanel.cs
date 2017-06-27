using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropInfoPanel : DragAndDrop, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public GameObject VirtualWalledObjectToDrop;
    public GameObject WalledObjectToDrop;

    private bool IsWalled;

	void IBeginDragHandler.OnBeginDrag(PointerEventData data) {
        IsWalled = false;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Vector3 pos = ray.origin + (ray.direction * 1000);
		InstantiatedObject = Instantiate (VirtualObjectToDrop, pos, Quaternion.identity).transform;
	}

	void IDragHandler.OnDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		Orientation or = Orienter.angleToOrientation (InstantiatedObject.rotation.eulerAngles.y);
		List<Orientation> PossibleOrientations = new List<Orientation> ();

		if (G.Sys.tilemap.at (InstantiatedObject.position + Vector3.forward).Count == 0)
			PossibleOrientations.Add (Orientation.LEFT);
        if (G.Sys.tilemap.at(InstantiatedObject.position + Vector3.back).Count == 0)
            PossibleOrientations.Add(Orientation.RIGHT);
		if (G.Sys.tilemap.at (InstantiatedObject.position + Vector3.right).Count == 0)
			PossibleOrientations.Add (Orientation.UP);
        if (G.Sys.tilemap.at(InstantiatedObject.position + Vector3.left).Count == 0)
			PossibleOrientations.Add (Orientation.DOWN);

        if (PossibleOrientations.Count > 0 && (!PossibleOrientations.Contains(or) || !IsWalled))
        {
            float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);

            if (!IsWalled)
            {
                IsWalled = true;
                var tf = Instantiate(VirtualWalledObjectToDrop, InstantiatedObject.position, Quaternion.Euler(0, desiredAngle, 0)).transform;
                Destroy(InstantiatedObject.gameObject);
                InstantiatedObject = tf;
            }
            if (InstantiatedObject.rotation.eulerAngles.y != desiredAngle)
                    RotateObject(desiredAngle);
        }
        else if (PossibleOrientations.Count == 0)
        {
            if (IsWalled)
            {
                IsWalled = false;
                var tf = Instantiate(VirtualObjectToDrop, InstantiatedObject.position, Quaternion.identity).transform;
                Destroy(InstantiatedObject.gameObject);
                InstantiatedObject = tf;
            }
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

		bool canPlace = true;

		//Case centrale
		var v = G.Sys.tilemap.at (InstantiatedObject.position);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(InstantiatedObject.position, TileID.ESCALATOR).Count > 0)
			canPlace = false;

		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		List<RaycastResult> results = new List<RaycastResult>();

		pointerData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerData, results);

		if ((results.Count > 0 && results [0].gameObject == gameObject) || !canPlace) {
			Destroy (InstantiatedObject.gameObject);
		} else {
            if (IsWalled)
            {
                Instantiate(WalledObjectToDrop, InstantiatedObject.position, InstantiatedObject.rotation);
            }
            else
            {
                Instantiate(ObjectToDrop, InstantiatedObject.position, InstantiatedObject.rotation);
            }
			Destroy (InstantiatedObject.gameObject);
		}

		InstantiatedObject = null;
	}

	void Update() {
		if (Input.GetButtonDown ("Rotate")) {
			RotateObject ();
		}
	}
}
