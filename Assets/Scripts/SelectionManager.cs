﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectionManager : MonoBehaviour {
	private DragAndDrop obj;
	private DragAndDropEntity ent;
	private Transform tf;

	private GameObject validate;
	private GameObject rotate;
	private GameObject delete;
	private GameObject changeside;
	private Button validateButton;

	private GameObject gameCanvas;
	private GameObject leftButton;
	private GameObject rightButton;
	private GameObject forwardButton;
	private GameObject backButton;

	private bool showArrows;

	private int Price = -1;

	void Awake() {
		G.Sys.selectionManager = this;
		tf = transform;

		gameCanvas = GameObject.Find ("GameCanvas").gameObject;
		leftButton = gameCanvas.transform.Find ("Left").gameObject;
		rightButton = gameCanvas.transform.Find ("Right").gameObject;
		forwardButton = gameCanvas.transform.Find ("Forward").gameObject;
		backButton = gameCanvas.transform.Find ("Back").gameObject;
		gameCanvas.SetActive (false);

		validate = tf.Find ("validate").gameObject;
		rotate = tf.Find ("rotate").gameObject;
		delete = tf.Find ("delete").gameObject;
		changeside = tf.Find ("changeside").gameObject;

		validateButton = validate.GetComponent<Button> ();
	}

	void Start() {
		Hide (false);
	}

	void ShowArrows() {
		showArrows = true;
		var objTf = obj.transform;

		gameCanvas.SetActive (true);
		gameCanvas.transform.position = objTf.position + Vector3.up * .1f;

		leftButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.left));
		leftButton.GetComponent<Button> ().interactable = obj.CanPlace;
		rightButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.right));
		rightButton.GetComponent<Button> ().interactable = obj.CanPlace;
		forwardButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.forward));
		forwardButton.GetComponent<Button> ().interactable = obj.CanPlace;
		backButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.back));
		backButton.GetComponent<Button> ().interactable = obj.CanPlace;
	}

	void HideArrows() {
		showArrows = false;
		gameCanvas.SetActive (false);
	}

	public void Left() {
		if(SpawnPodotactile (obj.transform.position + Vector3.left))
			G.Sys.cameraController.Move (Vector3.left);
	}

	public void Right() {
		if(SpawnPodotactile (obj.transform.position + Vector3.right))
			G.Sys.cameraController.Move (Vector3.right);
	}

	public void Forward() {
		if(SpawnPodotactile (obj.transform.position + Vector3.forward))
			G.Sys.cameraController.Move (Vector3.forward);
	}

	public void Back() {
		if(SpawnPodotactile (obj.transform.position + Vector3.back))
			G.Sys.cameraController.Move (Vector3.back);
	}

	public bool SpawnPodotactile(Vector3 pos) {
		if (obj.CanPlace) {
			obj.ValidateObject ();
			Hide (true);
			var o = Instantiate (G.Sys.gameManager.podotactile, new Vector3i (pos).toVector3 (), Quaternion.identity);
			var dad = o.GetComponent<DragAndDrop> ();
			dad.IsBought = false;
			Show (dad);
            Event<StartDragObjectEvent>.Broadcast(new StartDragObjectEvent(TileID.PODOTACTILE));
			return true;
		} else {
			return false;
		}
	}

	public void Show(DragAndDrop o) {
		if (o != null) {
			var tile = o.GetComponent<ATile> ();
			tile.Unregister ();
			G.Sys.cameraController.IsSelecting = true;

			o.IsSelected = true;
			Price = o.Price;
			obj = o;
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (obj.transform.position);

			validate.SetActive (true);
			rotate.SetActive (true);
			delete.SetActive (true);

			var e = obj.GetComponent<EscalatorTile> ();
			var p = obj.GetComponent<PodotactileTile> ();
			if (e != null) {
				rotate.transform.localPosition = Vector3.zero - Vector3.up * 150;
				changeside.SetActive (true);
			} else if (p != null) {
				rotate.transform.localPosition = Vector3.zero - Vector3.up * 200;
				ShowArrows ();
			} else {
				rotate.transform.localPosition = Vector3.zero - Vector3.up * 200;
			}

			obj.CanDrag = true;
			G.Sys.cameraController.IsSelecting = true;
			obj.DesactivateCollisions ();
			o.ToggleOutline (true);
		}
	}

	public void Show(DragAndDropEntity e, bool StopMovement) {
		if (e != null) {
			G.Sys.cameraController.IsSelecting = true;
			ent = e;
			ent.IsSelected = true;
			Price = e.Price;
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (ent.transform.position);
			if(StopMovement)
				ent.GetComponent<AEntity> ().SetIsStopped (true);
			validate.SetActive (true);
			delete.SetActive (true);

			ent.CanDrag = true;
			G.Sys.cameraController.IsSelecting = true;
			e.ToggleOutline (true);
			//ent.DesactivateCollisions ();
		}
	}

	public void Hide(bool register, bool isEntity, bool ResumeMovement) {
		if (isEntity) {
			Price = -1;
			//ent.ActivateCollisions ();
			var e = ent.GetComponent<AEntity> ();
			if (register) {
				e.enabled = true;
				G.Sys.cameraController.IsSelecting = false;
				ent.IsSelected = false;
				ent.ToggleOutline (false);
			}
			if(ResumeMovement) {
				e.SetIsStopped (false);
			}
			ent = null;

			validate.SetActive (false);
			rotate.SetActive (false);
			delete.SetActive (false);
			changeside.SetActive (false);
		} else {
			Hide (register);
		}
	}

	public void Move(Vector3 v) {
		obj.transform.position = new Vector3i (v).toVector3();
	}

	public void Hide(bool register) {
		if (obj != null) {
			Price = -1;
			obj.ActivateCollisions ();
			if (register) {
				obj.IsSelected = false;
				obj.GetComponent<ATile> ().Register ();
				G.Sys.cameraController.IsSelecting = false;
				obj.GetComponent<DragAndDrop> ().ToggleOutline (false);
			}
		}
		obj = null;

		HideArrows ();
		validate.SetActive (false);
		rotate.SetActive (false);
		delete.SetActive (false);
		changeside.SetActive (false);
	}

	public void Rotate() {
		obj.RotateObject ();
	}

	public void ValidateNoReturn() {
		if (obj != null)
			obj.ValidateObject ();
		if (ent != null)
			ent.ValidateObject ();
	}

	public bool Validate() {
		if (obj != null) {
			return obj.ValidateObject ();
		}
		if (ent != null) {
			return ent.ValidateObject ();
		}
		return false;
	}

	public void Delete() {
		G.Sys.cameraController.IsSelecting = false;
		if (obj != null) {
            Event<ObjectRemovedEvent>.Broadcast(new ObjectRemovedEvent(obj.transform.position, obj.GetComponent<ATile>().type, obj.IsBought));
			obj.DeleteObject ();
			Hide (false);
		}
		if (ent != null) {
            var agent = ent.GetComponent<Agent>();
            if (agent == null)
                Event<AgentRemovedEvent>.Broadcast(new AgentRemovedEvent(ent.transform.position, AgentType.CLEANER, ent.IsBought));
            else Event<AgentRemovedEvent>.Broadcast(new AgentRemovedEvent(ent.transform.position, AgentType.AGENT, ent.IsBought));
			ent.DeleteObject ();
			Hide (false, true, false);
		}
	}

	public void ChangeSide() {
		var e = obj.GetComponent<EscalatorTile> ();
		if (e != null) {
			if (e.side == EscalatorSide.DOWN) {
				e.side = EscalatorSide.UP;
			} else {
				e.side = EscalatorSide.DOWN;
			}
		}
	}

	void Update() {
		if (obj != null) {
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (obj.transform.position);
		}
		if (ent != null) {
			tf.position = G.Sys.MainCamera.WorldToScreenPoint (ent.transform.position);
		}

		if (Price > -1) {
			bool b = (G.Sys.gameManager.GetMoney () >= Price) && ((obj != null && obj.CanPlace) || (ent != null && ent.CanPlace));
            if (obj != null && obj.IsSelected && obj.IsBought && obj.CanPlace)
                b = true;
			if (ent != null && ent.IsSelected && ent.IsBought)
				b = true;
			validateButton.interactable = b;

			if(showArrows) {
				leftButton.GetComponent<Button> ().interactable = b;
				rightButton.GetComponent<Button> ().interactable = b;
				forwardButton.GetComponent<Button> ().interactable = b;
				backButton.GetComponent<Button> ().interactable = b;
			}
		}

		if (showArrows && obj != null) {
			var objTf = obj.transform;
			gameCanvas.transform.position = objTf.position + Vector3.up * .1f;

			leftButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.left));
			//leftButton.GetComponent<Button> ().interactable = obj.CanPlace;
			rightButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.right));
			//rightButton.GetComponent<Button> ().interactable = obj.CanPlace;
			forwardButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.forward));
			//forwardButton.GetComponent<Button> ().interactable = obj.CanPlace;
			backButton.SetActive (G.Sys.tilemap.IsValidTileForPodotactile (objTf.position + Vector3.back));
			//backButton.GetComponent<Button> ().interactable = obj.CanPlace;
		}
	}
}
