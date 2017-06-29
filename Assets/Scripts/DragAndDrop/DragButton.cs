using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public GameObject ObjectToSpawn;

	private GameObject spawnedObject;
	private bool CanEndDrag;

	void Spawn() {
		if (!G.Sys.cameraController.IsSelecting) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo;
			Physics.Raycast (ray, out hitInfo);
			spawnedObject = Instantiate (ObjectToSpawn, new Vector3i (hitInfo.transform.position).toVector3 (), Quaternion.identity);
			spawnedObject.GetComponent<ATile> ().Unregister ();
			G.Sys.selectionManager.Show (spawnedObject.GetComponent<DragAndDrop> ());;
		}
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		if (!G.Sys.cameraController.IsSelecting) {	
			Spawn ();
			spawnedObject.GetComponent<DragAndDrop> ().StartDrag ();
			CanEndDrag = true;
		} else {
			CanEndDrag = false;
		}
	}

	void IDragHandler.OnDrag(PointerEventData eventData) {}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
		if (CanEndDrag) {
			List<RaycastResult> raycastResults = new List<RaycastResult> ();
			PointerEventData ped = new PointerEventData (EventSystem.current);
			ped.position = Input.mousePosition;
			EventSystem.current.RaycastAll (ped, raycastResults);

			if (raycastResults.Count > 0) {
				spawnedObject.GetComponent<DragAndDrop> ().DeleteObject ();
			} else {
				spawnedObject.GetComponent<DragAndDrop> ().OnMouseUp ();
			}
		}
	}
}
