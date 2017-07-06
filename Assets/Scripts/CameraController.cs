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

	private float CurrentDragSpeed;
	private Vector3 dragOrigin;
	private Transform cameraTransform;
	private bool canSelect;
	private bool isOnUI = false;
	public bool pinching = false;

	IEnumerator PinchingCoroutine;
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
		if (Input.touchCount == 0) {
			float scroll = Input.GetAxisRaw ("Mouse ScrollWheel");
			if (scroll > 0) {
				G.Sys.menuManager.Zoom (7.5f);
			} else if (scroll < 0) {
				G.Sys.menuManager.Zoom (-7.5f);
			}
		}

		if (Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			if (deltaMagnitudeDiff != 0) {
				if (PinchingCoroutine != null)
					StopCoroutine (PinchingCoroutine);
				pinching = true;
				if (deltaMagnitudeDiff < 0) {
					G.Sys.menuManager.Zoom (4f);
				} else if (deltaMagnitudeDiff > 0) {
					G.Sys.menuManager.Zoom (-4f);
				}
			}
		} else {
			if (pinching) {
				PinchingCoroutine = DelayedPinch ();
				StartCoroutine (PinchingCoroutine);
			}
			if (CanDrag && Input.touchCount <= 1) {
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
			}

			float dist = Vector3.Distance (Input.mousePosition, dragOrigin);
			if (!pinching && (Input.GetMouseButton (0) || Input.GetMouseButton(2)) &&  dist > 1f /*&& dist <= 500f*/ && !isOnUI && CanDrag && Input.touchCount <= 1) {
				if (canSelect) {
					canSelect = false;
					if(SelectCoroutine != null)
						StopCoroutine (SelectCoroutine);
				}
				var bounds = G.Sys.tilemap.GlobalBounds ();
				var move = new Vector3 (Mathf.Clamp(-Input.GetAxis ("Mouse X") * Time.deltaTime * DragSpeed, bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x), 0, Mathf.Clamp(-Input.GetAxis ("Mouse Y") * Time.deltaTime * DragSpeed, bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z));
				cameraTransform.Translate(new Vector3 (-Input.GetAxis ("Mouse X") * Time.deltaTime * DragSpeed * G.Sys.menuManager.GetZoomRatio(), 0, -Input.GetAxis ("Mouse Y") * Time.deltaTime * DragSpeed * G.Sys.menuManager.GetZoomRatio()));
				cameraTransform.position = new Vector3 (Mathf.Clamp(cameraTransform.position.x, bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x), cameraTransform.position.y, Mathf.Clamp(cameraTransform.position.z, bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z));
				dragOrigin = Input.mousePosition;
			}
		}

		if (Input.GetMouseButtonUp (0))
			isOnUI = false;
	}

	IEnumerator DelayedPinch() {
		yield return new WaitForSeconds (.1f);
		if(pinching)
			pinching = false;
	}

	IEnumerator Select() {
		yield return new WaitForSeconds (.1f);

		if (canSelect) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit info;
			Physics.Raycast (ray, out info);

			if (info.transform != null && info.transform.CompareTag ("Disposable") && !IsSelecting) {
				var dad = info.transform.GetComponent<DragAndDrop> ();
				var tile = dad.GetComponent<ATile> ();
				G.Sys.selectionManager.Show (dad);
				IsSelecting = true;
				tile.Unregister ();
				dad.DesactivateCollisions ();
			} else if (IsSelecting && info.transform != null) {
				G.Sys.selectionManager.Move (info.transform.position);
			}
		}
	}
}