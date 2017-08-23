using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class StairsFallAction : AEntityAction<Traveler>
{
	public StairsFallAction (Traveler t, StairsTile tile) : base(t, ActionType.FAINT, t.transform.position, 100)
	{
		traveler = t;
		stairsTile = tile;
		entity.transform.DOMove (t.transform.position, .1f);
	}

	protected override bool Start ()
	{
		if (entity.datas.Tiredness <= 0.95f)
			return true;
		entity.GetComponent<NavMeshAgent> ().enabled = false;
		//entity.transform.DORotate (new Vector3(90, entity.transform.rotation.eulerAngles.y, 0), 1f, RotateMode.Fast);
		entity.transform.DOMove (stairsTile.GetDownOfStairs (), .75f).SetEase (Ease.Linear).OnComplete(() => {
			Event<FaintEvent>.Broadcast (new FaintEvent (traveler));
		});
		G.Sys.audioManager.PlayStairsFall ();
		return false;
	}

	protected override bool Update ()
	{
		traveler.anim.SetBool ("Falling", true);
		return entity.datas.Tiredness <= 0.95f;
	}

	protected override void End ()
	{
		//entity.transform.DORotate (new Vector3(0, entity.transform.rotation.eulerAngles.y, 0), .5f, RotateMode.Fast);
		entity.GetUp ();
		traveler.anim.SetBool ("Falling", false);
		entity.GetComponent<NavMeshAgent> ().enabled = true;
	}

	Traveler traveler;
	StairsTile stairsTile;
}