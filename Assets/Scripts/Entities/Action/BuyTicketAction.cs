using System;
using UnityEngine;

public class BuyTicketAction : AEntityAction<Traveler>
{
	TicketDistribTile ticketDistrib;
	float time = 0;

	public BuyTicketAction (Traveler t, Vector3 pos, TicketDistribTile tile) : base(t, ActionType.BUY_TICKET, pos)
	{
		ticketDistrib = tile;
	}

	protected override bool Start ()
	{
		return entity.datas.HasTicket || entity.datas.Fraud;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time > 2;
	}

	protected override void End ()
	{
		entity.datas.HasTicket = true;
		G.Sys.gameManager.AddMoney (ticketDistrib.price);
	}
}
