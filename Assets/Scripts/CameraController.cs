using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CameraController : MonoBehaviour {
	public float DragSpeed;
	[HideInInspector]
	public bool CanDrag;
	[HideInInspector]
	public bool IsSelecting;

	private Vector3 dragOrigin;
	private Transform cameraTransform;
	private bool canSelect;
	public bool pinching = false;

	private Tweener dragCameraTweener;

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
					G.Sys.menuManager.Zoom (2f);
				} else if (deltaMagnitudeDiff > 0f) {
					G.Sys.menuManager.Zoom (-2f);
				}
			}
		} else {
			if (pinching) {
				PinchingCoroutine = DelayedPinch ();
				StartCoroutine (PinchingCoroutine);
			}
			if (CanDrag && Input.touchCount <= 1) {
				if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown(2)) {
					if (!IsOnUI()) {
						dragOrigin = Input.mousePosition;
						if (Input.GetMouseButtonDown (0)) {
							canSelect = true;
							SelectCoroutine = Select ();
							StartCoroutine (SelectCoroutine);
						}
					}
				}
			}

			float dist = Vector3.Distance (Input.mousePosition, dragOrigin);

			if ((Input.GetMouseButton (0) || Input.GetMouseButton(2)) && !pinching && !IsOnUI() && dist > 0 && CanDrag && Input.touchCount <= 1) {
				if (canSelect) {
					canSelect = false;
					if(SelectCoroutine != null)
						StopCoroutine (SelectCoroutine);
				}
				var bounds = G.Sys.tilemap.GlobalBounds ();

				var originRay = G.Sys.MainCamera.ScreenPointToRay (dragOrigin);
				var currentRay = G.Sys.MainCamera.ScreenPointToRay (Input.mousePosition);

				var hit1 = GetBackgroundHit (Physics.RaycastAll (originRay));
				var hit2 = GetBackgroundHit (Physics.RaycastAll (currentRay));

				Vector3 vec = hit1.point - hit2.point;
				vec.y = 0;

				if (dragCameraTweener != null)
					dragCameraTweener.Pause ();
				dragCameraTweener = cameraTransform.DOMove(cameraTransform.position + vec, Time.deltaTime);

				float oldX, oldZ, newX, newY, newZ;

				oldX = cameraTransform.position.x;
				oldZ = cameraTransform.position.z;

				newX = Mathf.Clamp (oldX, bounds.min.x, bounds.max.x);
				newY = cameraTransform.position.y;
				newZ = Mathf.Clamp (oldZ, bounds.min.z, bounds.max.z);

				if (newX != oldX || newZ != oldZ)
					dragCameraTweener.Pause ();

				cameraTransform.position = new Vector3 (newX, newY, newZ);

				dragOrigin = Input.mousePosition;
			}
		}
	}

	public bool IsOnUI() {
		List<RaycastResult> raycastResults = new List<RaycastResult> ();
		PointerEventData ped = new PointerEventData (EventSystem.current);
		ped.position = Input.mousePosition;
		EventSystem.current.RaycastAll (ped, raycastResults);

		return raycastResults.Count > 0;
	}

	RaycastHit GetBackgroundHit(RaycastHit[] hits) {
		foreach (var h in hits)
			if (h.transform.CompareTag ("UNDERGROUND"))
				return h;

		return new RaycastHit();
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
					G.Sys.selectionManager.Show (dad);
				} else {
					var dade = disposable.GetComponent<DragAndDropEntity> ();
					if (dade != null) {
						G.Sys.selectionManager.Show (dade, true);
					}
				}
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

	public void Move(Vector3 v) {
		cameraTransform.DOMove(cameraTransform.position + v, .5f);
	}
}