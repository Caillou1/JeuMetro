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
				G.Sys.menuManager.ZoomIn ();
			} else if (scroll < 0) {
				G.Sys.menuManager.ZoomOut ();
			}
		} if (Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			if (deltaMagnitudeDiff < 0) {
				G.Sys.menuManager.ZoomIn ();
			} else if (deltaMagnitudeDiff > 0) {
				G.Sys.menuManager.ZoomOut ();
			}
		}


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

			if ((Input.GetMouseButton (0) || Input.GetMouseButton(2)) && Vector3.Distance (Input.mousePosition, dragOrigin) > 1f && !isOnUI) {
				if (canSelect) {
					canSelect = false;
					if(SelectCoroutine != null)
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