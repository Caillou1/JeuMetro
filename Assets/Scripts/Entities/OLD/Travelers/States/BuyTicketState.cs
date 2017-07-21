using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NRand;

namespace V1
{
public class BuyTicketState : ATravelerState
{
	Vector3 dest = new Vector3();
	Vector3 shopPos = new Vector3 ();

	public BuyTicketState (Traveler t) : base(t, StateType.TRAVELER_BUY_TICKET)
	{
	}
		
	public override int check()
	{
		if (traveler.altAction == Traveler.ActionType.BUY_TICKET && new Vector3i (dest).equal (new Vector3i (traveler.transform.position)))
			return int.MaxValue;

		if (traveler.datas.HasTicket)
			return 0;

		if (traveler.altAction != Traveler.ActionType.LOST && traveler.altAction != Traveler.ActionType.NONE)
			return 0;

		var shop = G.Sys.tilemap.getNearestSpecialTileOfType (traveler.transform.position, TileID.TICKETDISTRIBUTEUR, 5);
		if (!shop.Second)
			return 0;

		var tile = G.Sys.tilemap.tilesOfTypeAt (shop.First, TileID.TICKETDISTRIBUTEUR);
		if (tile.Count == 0)
			return 0;

		shopPos = shop.First;
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(tile[0].transform.rotation.eulerAngles.y));
		var pos = shopPos + new Vector3 (-dir.y, 0, dir.x);

		traveler.altAction = Traveler.ActionType.BUY_TICKET;
		traveler.altDestination = pos;
		traveler.altWait = false;
		dest = pos;
		traveler.Updatepath ();

		return 0;
	}

	public override void start ()
	{
		if (!G.Sys.tilemap.haveSpecialTileAt (TileID.TICKETDISTRIBUTEUR, shopPos)) {
			traveler.BackToMoveState ();
			return;
		}
		traveler.StartCoroutine (waitCoroutine());
	}

	public IEnumerator waitCoroutine()
	{
		yield return new WaitForSeconds (0.5f);
		traveler.datas.Waste += 0.5f;
		traveler.datas.HasTicket = true;
		traveler.Updatepath ();
		traveler.BackToMoveState ();
		traveler.altAction = Traveler.ActionType.NONE;
	}

	public override void update()
	{
		traveler.rigidbody.velocity = new Vector3 ();
	}

	public override bool canBeStopped()
	{
		return false;
	}
}
}