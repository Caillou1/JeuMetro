﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public GameObject ObjectToDrop;

	private Transform InstantiatedObject;
	private int Rotations = 0;
	private bool isRotating = false;


	void IBeginDragHandler.OnBeginDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Vector3 pos = ray.origin + (ray.direction * 1000);
		InstantiatedObject = Instantiate (ObjectToDrop, pos, Quaternion.identity).transform;
	}

	void IDragHandler.OnDrag(PointerEventData data) {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			if (hit.transform.CompareTag ("Ground")) {
				InstantiatedObject.position = new Vector3 (Mathf.RoundToInt (hit.point.x), Mathf.RoundToInt (hit.point.y), Mathf.RoundToInt (hit.point.z)) + Vector3.up;
			}
		} else {
			Vector3 pos = ray.origin + (ray.direction * 1000);
			InstantiatedObject.position = new Vector3 (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y), Mathf.RoundToInt (pos.z));
		}

	}

	void IEndDragHandler.OnEndDrag(PointerEventData data) {
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		List<RaycastResult> results = new List<RaycastResult>();
		
		pointerData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerData, results);

		if (results.Count > 0 && results [0].gameObject == gameObject) {
			Destroy (InstantiatedObject.gameObject);
		} else {
			InstantiatedObject.GetComponent<Collider> ().enabled = true;
		}

		InstantiatedObject = null;
	}

	void Update() {
		if (Input.GetButtonDown ("Rotate")) {
			RotateObject ();
		}
	}

	void RotateObject() {
		if (isRotating) {
			Rotations++;
		} else {
			isRotating = true;
			if(InstantiatedObject != null) InstantiatedObject.DORotate (new Vector3 (0, InstantiatedObject.rotation.eulerAngles.y + 90, 0), .3f, RotateMode.FastBeyond360).OnComplete(() => {
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
}
