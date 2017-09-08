using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class DragAndDrop : MonoBehaviour{
	public int Price;
	protected bool IsWalled;
	protected bool HasToCheckWall;
	protected int Space;

	protected bool isSelected;
	public bool IsSelected {
        get { return isSelected; }
		set {
			isSelected = value;
		}
	}

	protected Transform tf;
	protected int Rotations = 0;
	protected bool isRotating = false;
	protected bool canPlace;
	public bool CanPlace {
		get {
			CheckCanPlace ();
			return canPlace;
		}
	}
	//[HideInInspector]
	public bool CanDrag = false;
	protected bool Dragging = false;
	protected bool bought;
	public bool IsBought {
		get{ return bought; }
		set{
			bought = value;
		}
	}

	private Tweener rotTween;
	private Vector3 originalScale;
	private cakeslice.Outline[] outlines;

	void Awake() {
		tf = transform;
		originalScale = tf.localScale;
		GetOutlines ();
		IsWalled = false;
		bought = true;
		OnAwake ();
	}

	public void GetOutlines() {
		var p = GetComponent<PodotactileTile> ();
		if (p == null)
			outlines = tf.GetComponentsInChildren<cakeslice.Outline> ();
		else
			outlines = p.getOutlines ();
	}

	protected virtual void OnAwake() {
		Space = 1;
	}

	void Start() {
		CheckCanPlace ();
		CheckRotation ();
		isRotating = false;
		if(bought)
			ToggleOutline (false);
	}

	protected virtual void CheckCanPlace() {
		canPlace = true;

		var v = G.Sys.tilemap.at (tf.position);
		var p = GetComponent<PodotactileTile> ();
		if (v.Count == 0 || ((p!=null && (((v [0].type != TileID.GROUND) && !HasTileOfType(v, TileID.WAIT_ZONE) && !HasTileOfType(v, TileID.CONTROLELINE)) || HasTileOfType(v, TileID.PODOTACTILE))) || (p==null && v[0].type != TileID.GROUND && !HasTileOfType(v, TileID.WAIT_ZONE))))
			canPlace = false;
	}

	protected bool HasTileOfType(List<ATile> list, TileID type) {
		foreach (var l in list) {
			if (l.type == type) {
				return true;
			}
		}
		return false;
	}

	protected virtual void CheckRotation() {}

	protected void OnMouseDown() {
		List<RaycastResult> raycastResults = new List<RaycastResult> ();
		PointerEventData ped = new PointerEventData (EventSystem.current);
		ped.position = Input.mousePosition;
		EventSystem.current.RaycastAll (ped, raycastResults);

		if (CanDrag && raycastResults.Count == 0) {
			StartDrag ();
		}
	}

	public void StartDrag() {
		if (!IsBought) {
			ToggleOutline (true);
			G.Sys.selectionManager.Hide (false);
			G.Sys.cameraController.CanDrag = false;
			Dragging = true;
		}
	}

	void Update() {
        if (!IsBought && (Dragging || IsSelected))
        {
            if (G.Sys.gameManager.GetMoney() < Price)
                G.Sys.menuManager.MakeMoneyBlink();
            else G.Sys.menuManager.StopBlinkMoney();
        }
        else G.Sys.menuManager.StopMoneyBlink();

        if (Dragging && CanDrag && !IsBought) {
			Ray ray = G.Sys.MainCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] hits;
			hits = Physics.RaycastAll (ray);
			RaycastHit hit = FindGround (hits);

			if (hit.transform != null && !G.Sys.cameraController.IsOnUI ()) {
				Vector3 objPos = hit.transform.position;
				tf.position = new Vector3 (Mathf.RoundToInt (objPos.x), Mathf.RoundToInt (objPos.y), Mathf.RoundToInt (objPos.z));
			} else if (hit.transform != null) {
				Vector3 objPos = hit.point;
				tf.position = new Vector3 (objPos.x, Mathf.RoundToInt (objPos.y), objPos.z);
			} else {
				tf.position = new Vector3i (tf.position).toVector3 ();
			}

			CheckRotation ();
			CheckCanPlace ();

			if (canPlace && !G.Sys.cameraController.IsOnUI ()) {
				if (G.Sys.gameManager.GetMoney() < Price)
					cakeslice.OutlineEffect.Instance.lineColor0 = Color.red;
				else
					cakeslice.OutlineEffect.Instance.lineColor0 = Color.green;
			} else {
				cakeslice.OutlineEffect.Instance.lineColor0 = Color.red;
			}

			if (G.Sys.cameraController.IsOnUI ()) {
				tf.localScale = originalScale / 2;
			} else {
				tf.localScale = originalScale;
			}
		} else if (isSelected && !IsBought) {
			CheckCanPlace ();

			if (canPlace) {
                if(G.Sys.gameManager.GetMoney() < Price)
                    cakeslice.OutlineEffect.Instance.lineColor0 = Color.red;
				else 
                    cakeslice.OutlineEffect.Instance.lineColor0 = Color.green;
			} else {
				cakeslice.OutlineEffect.Instance.lineColor0 = Color.red;
			}
		} else if (isSelected && IsBought) {
			cakeslice.OutlineEffect.Instance.lineColor0 = Color.yellow;
		}
	}

	RaycastHit FindGround(RaycastHit[] hits) {
		hits = hits.OrderBy (h => h.distance).ToArray();

		foreach (var h in hits) {
			if (h.transform.CompareTag ("Ground"))
				return h;
		}

		return new RaycastHit ();
	}

	public void OnMouseUp() {
		if (CanDrag) {
			G.Sys.selectionManager.Show (this);
			G.Sys.cameraController.CanDrag = true;

			Dragging = false;
		}
	}

	public void DeleteObject() {
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
        if(!IsBought)
		    Event<AbortDragObjectEvent>.Broadcast(new AbortDragObjectEvent(GetComponent<ATile>().type));
        G.Sys.tilemap.addSpaceUsed(-Space);
		Destroy (gameObject);
	}

	protected virtual void SendEvent(bool wasBought) {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		var tile = tf.GetComponent<ATile>();
		if (tile != null)
            Event<ObjectPlacedEvent>.Broadcast(new ObjectPlacedEvent(list, tile.type, wasBought));
	}

	public bool ValidateObject() {
		tf.localScale = originalScale;
		Rotations = 0;
		rotTween.Complete ();
		CheckCanPlace ();
		if(HasToCheckWall)
			CheckRotation ();

		if ((canPlace && !IsBought && G.Sys.gameManager.HaveEnoughMoney(Price)) || IsBought && canPlace) {
			G.Sys.cameraController.IsSelecting = false;
            bool wasBought = IsBought;
			if (!IsBought) {
                G.Sys.tilemap.addSpaceUsed(Space);
				G.Sys.gameManager.AddMoney (-Price);
				IsBought = true;
				var tile = GetComponent<ATile> ();
				tile.Register ();
				OnBuy ();
			}
			G.Sys.selectionManager.Hide (true);
			CanDrag = false;
            SendEvent (wasBought);
			DeletePossibleEmptyWalls ();
			StartCoroutine (eventCoroutine ());
			G.Sys.audioManager.PlayConstruct ();
			return true;
		}
		return false;
	}

	protected virtual void OnBuy() {
		G.Sys.AddDisposable (TileID.PODOTACTILE);
	}

	IEnumerator eventCoroutine()
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
	}

	protected virtual void DeletePossibleEmptyWalls () {}
		
	public void RotateObject() {
		if (isRotating) {
			Rotations++;
		} else {
			isRotating = true;
			if (tf != null) {
				rotTween = tf.DORotate (new Vector3 (0, tf.rotation.eulerAngles.y + 90, 0), .3f, RotateMode.FastBeyond360).OnComplete (() => {
					if (Rotations > 0) {
						Rotations--;
						isRotating = false;
						RotateObject ();
					} else {
						isRotating = false;
					}
				});
			}
		}
	}

	public void DesactivateCollisions() {
		foreach (var c in GetComponentsInChildren<Collider>())
			if (c.transform != tf)
				c.enabled = false;
	}

	public void ActivateCollisions() {
		foreach (var c in GetComponentsInChildren<Collider>())
				c.enabled = true;
	}

	protected void RotateObject(float desiredAngle) {
		rotTween = tf.DORotate (new Vector3 (0, desiredAngle, 0), .3f, RotateMode.FastBeyond360);
	}

	public void ToggleOutline(bool b) {
		if (isSelected && IsBought) {
			cakeslice.OutlineEffect.Instance.lineColor0 = Color.yellow;
		}

		if (outlines == null)
			GetOutlines ();

		foreach(var o in outlines)
			o.enabled = b;
	}
}
