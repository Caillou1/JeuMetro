using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
	public GameObject ObjectToSpawn;

	private GameObject spawnedObject;
	private bool CanEndDrag;
    private GameObject hoveredObject;

	void Awake() {
		transform.Find ("Price").GetComponent<Text> ().text = ObjectToSpawn.GetComponent<DragAndDrop> ().Price.ToString();
        hoveredObject = transform.Find("BackText").gameObject;
        hoveredObject.SetActive(false);
	}

	void Spawn() {
		if (!G.Sys.cameraController.IsSelecting) {
			spawnedObject = Instantiate (ObjectToSpawn, new Vector3 (999, 999, 999), Quaternion.identity);
            var tile = spawnedObject.GetComponent<ATile>();
            tile.Unregister ();
			var dad = spawnedObject.GetComponent<DragAndDrop> ();
			G.Sys.selectionManager.Show (dad);
			dad.ToggleOutline (true);
            Event<StartDragObjectEvent>.Broadcast(new StartDragObjectEvent(tile.type));
		}
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		bool validated = G.Sys.selectionManager.Validate ();
		if (!validated) {
			G.Sys.selectionManager.Delete ();
		}

		if (!G.Sys.cameraController.IsSelecting) {	
			Spawn ();
			var dad = spawnedObject.GetComponent<DragAndDrop> ();
			dad.IsBought = false;
			dad.StartDrag ();
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
				G.Sys.cameraController.IsSelecting = false;
				G.Sys.cameraController.CanDrag = true;
			} else {
				spawnedObject.GetComponent<DragAndDrop> ().OnMouseUp ();
			}
		}
	}

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        hoveredObject.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        hoveredObject.SetActive(false);
    }
}
