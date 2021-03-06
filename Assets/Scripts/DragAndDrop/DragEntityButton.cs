﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragEntityButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
	public GameObject EntityToSpawn;

	private GameObject spawnedObject;
	private bool CanEndDrag;
	private GameObject hoveredObject;

	void Awake() {
		transform.Find("Price").GetComponent<Text>().text = EntityToSpawn.GetComponent<DragAndDropEntity>().Price.ToString(); 
        hoveredObject = transform.Find("BackText").gameObject;
		hoveredObject.SetActive(false);
	}

	void Spawn() {
		if (!G.Sys.cameraController.IsSelecting) {
			spawnedObject = Instantiate (EntityToSpawn, new Vector3 (999, 999, 999), Quaternion.identity);
			var dade = spawnedObject.GetComponent<DragAndDropEntity> ();
			G.Sys.selectionManager.Show (dade, false);
			dade.ToggleOutline (true);
            if (spawnedObject.GetComponent<Agent>() != null)
                Event<StartDragAgentEvent>.Broadcast(new StartDragAgentEvent(AgentType.AGENT));
            else Event<StartDragAgentEvent>.Broadcast(new StartDragAgentEvent(AgentType.CLEANER));
		}
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		bool validated = G.Sys.selectionManager.Validate ();
		if (!validated) {
			G.Sys.selectionManager.Delete ();
		}

		if (!G.Sys.cameraController.IsSelecting) {	
			Spawn ();
			var dad = spawnedObject.GetComponent<DragAndDropEntity> ();
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
				spawnedObject.GetComponent<DragAndDropEntity> ().DeleteObject ();
				G.Sys.cameraController.IsSelecting = false;
				G.Sys.cameraController.CanDrag = true;
			} else {
				spawnedObject.GetComponent<DragAndDropEntity> ().OnMouseUp ();
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
