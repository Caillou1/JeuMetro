using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Metro : MonoBehaviour {

    struct DoorInfo
    {
        public DoorInfo(Transform _obj, Vector3 _pos, Vector3 _dir)
        {
            obj = _obj;
            originalPos = _pos;
            dir = _dir;
        }

        public Transform obj;
        public Vector3 originalPos;
        public Vector3 dir;
    }

	public List<MetroTile> OutTiles;
	[Tooltip("Déplacement du metro quand il arrive et part")]
	public float moveOffset = 12.5f;

	private Transform tf;
	private Vector3 positionToReach;
	private Vector3 startPosition;
	private Vector3 endPosition;

    List<DoorInfo> doors = new List<DoorInfo>();
    const float doorsOffset = 0.95f;
    const float doorsMoveTime = 0.5f;

	void Awake()
	{
		G.Sys.registerMetro (this);

		tf = transform;
		positionToReach = tf.position;
		startPosition = tf.position - tf.forward * moveOffset;
		endPosition = tf.position + tf.forward * moveOffset;

		tf.position = startPosition;

        for (int i = 1; i <= 8; i++)
        {
            var obj = transform.Find("porte_0" + i);
            if (obj == null)
                continue;
            doors.Add(new DoorInfo(obj, obj.transform.localPosition, ((float)(((i-1)/2)%2) * 2 - 1) * new Vector3(0, 0, 1)));
        }
	}

	void OnDestroy()
	{
		G.Sys.removeMetro (this);
	}

	void ResetPos() {
		tf.position = startPosition;
	}

	public void CallMetro() {
		ResetPos ();
		G.Sys.audioManager.PlayTrainStop ();
		tf.DOMove (positionToReach, G.Sys.constants.MetroComeTime).SetEase(Ease.OutQuad).OnComplete(()=>{
			EnableTiles(true);
            OpenDoors();
			DOVirtual.DelayedCall(G.Sys.constants.MetroWaitTime, () => {
				Leave();
			});
		});
	}

	public void Leave() {
		EnableTiles (false);
		G.Sys.audioManager.PlayTrainStart ();
		tf.DOMove (endPosition, G.Sys.constants.MetroComeTime).SetEase(Ease.InExpo);
        CloseDoors();
	}

	void EnableTiles(bool value)
	{
		foreach(var t in OutTiles)
			t.tileEnabled = value;
	}

    void OpenDoors()
    {
        foreach(var d in doors)
            d.obj.DOLocalMove(d.originalPos + d.dir * doorsOffset, doorsMoveTime);
    }

    void CloseDoors()
    {
        foreach(var d in doors)
            d.obj.DOLocalMove(d.originalPos, doorsMoveTime);
    }
}
