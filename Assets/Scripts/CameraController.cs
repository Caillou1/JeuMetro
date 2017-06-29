using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {
	public float DragSpeed;
	[HideInInspector]
	public bool CanDrag;
	[HideInInspector]
	public bool IsSelecting;

	private Vector3 dragOrigin;
	private Transform cameraTransform;
	private bool canSelect;
	private bool isOnUI = false;

	IEnumerator SelectCoroutine;

	void Awake() {
		G.Sys.cameraController = this;
	}

	void Start() {
		cameraTransform = transform;
		canSelect = true;
		CanDrag = true;
		IsSelecting = false;
	}

	void LateUpdate() {
		if (CanDrag) {
			List<RaycastResult> raycastResults = new List<RaycastResult> ();
			if (Input.GetMouseButtonDown (0)) {
				PointerEventData ped = new PointerEventData (EventSystem.current);
				ped.position = Input.mousePosition;
				EventSystem.current.RaycastAll (ped, raycastResults);

				if (raycastResults.Count == 0) {
					dragOrigin = Input.mousePosition;
					canSelect = true;
					SelectCoroutine = Select ();
					StartCoroutine (SelectCoroutine);
				} else {
					isOnUI = true;
				}
			}

			if (Input.GetMouseButton (0) && Vector3.Distance (Input.mousePosition, dragOrigin) > 1f && !isOnUI) {
				if (canSelect) {
					canSelect = false;
					StopCoroutine (SelectCoroutine);
				}
				cameraTransform.Translate (new Vector3 (-Input.GetAxis ("Mouse X") * Time.deltaTime * DragSpeed, 0, -Input.GetAxis ("Mouse Y") * Time.deltaTime * DragSpeed));
			}
		}

		if (Input.GetMouseButtonUp (0))
			isOnUI = false;
	}

	IEnumerator Select() {
		yield return new WaitForSeconds (.1f);

		if (canSelect) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit info;
			Physics.Raycast (ray, out info);

			if (info.transform != null && info.transform.CompareTag ("Disposable") && !IsSelecting) {
				var dad = info.transform.GetComponent<DragAndDrop> ();
				G.Sys.selectionManager.Show (dad);
				IsSelecting = true;
			} else if (IsSelecting && info.transform != null) {
				G.Sys.selectionManager.Move (info.transform.position);
			}
		}
	}
}
