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
				if (deltaMagnitudeDiff < 0f) {
					G.Sys.menuManager.Zoom (4f);
				} else if (deltaMagnitudeDiff > 0f) {
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
					}
				}
			}

			float dist = Vector3.Distance (Input.mousePosition, dragOrigin);
			if (!pinching && (Input.GetMouseButton (0) || Input.GetMouseButton(2)) &&  dist > 10f /*&& dist <= 500f && !isOnUI*/ && CanDrag && Input.touchCount <= 1) {
				if (canSelect) {
					canSelect = false;
					if(SelectCoroutine != null)
						StopCoroutine (SelectCoroutine);
				}
				var bounds = G.Sys.tilemap.GlobalBounds ();
				cameraTransform.Translate(new Vector3 (-Input.GetAxis ("Mouse X") * Time.deltaTime * DragSpeed * G.Sys.menuManager.GetZoomRatio(), 0, -Input.GetAxis ("Mouse Y") * Time.deltaTime * DragSpeed * G.Sys.menuManager.GetZoomRatio()));
				cameraTransform.position = new Vector3 (Mathf.Clamp(cameraTransform.position.x, bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x), cameraTransform.position.y, Mathf.Clamp(cameraTransform.position.z, bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z));
				dragOrigin = Input.mousePosition;
			}
		}
	}

	IEnumerator DelayedPinch() {
		yield return new WaitForSeconds (.1f);
		if(pinching)
			pinching = false;
	}

	IEnumerator Select() {
		yield return new WaitForSeconds (.1f);

		if (canSelect) {
			Ray ray = G.Sys.MainCamera.ScreenPointToRay (Input.mousePosition);
			var disposable = GetDisposable (Physics.RaycastAll (ray));

			if (disposable != null && !IsSelecting) {
				var dad = disposable.GetComponent<DragAndDrop> ();
				if (dad != null) {
					var tile = disposable.GetComponent<ATile> ();
					G.Sys.selectionManager.Show (dad);
					tile.Unregister ();
				} else {
					var dade = disposable.GetComponent<DragAndDropEntity> ();
					if (dade != null) {
						G.Sys.selectionManager.Show (dade);
					}
				}
				IsSelecting = true;
			}
		}
	}

	GameObject GetDisposable(RaycastHit[] infos) {
		foreach (var info in infos) {
			if (info.transform.CompareTag ("Disposable")) {
				return info.transform.gameObject;
			}
		}
		return null;
	}
}