using System;
using UnityEngine;

public class BuyTicketAction : AEntityAction<Traveler>
{
	TicketDistribTile ticketDistrib;
	float time = 0;
	float maxTime = 0;

	public BuyTicketAction (Traveler t, Vector3 pos, TicketDistribTile tile) : base(t, ActionType.BUY_TICKET, pos)
	{
		ticketDistrib = tile;
	}

	protected override bool Start ()
	{
		bool canRun = entity.datas.HasTicket || entity.datas.Fraud;
		if (!canRun)
			ticketDistrib.queue++;
		maxTime = ticketDistrib.queue * 2;
		return canRun;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time > maxTime;
	}

	protected override void End ()
	{
		entity.datas.HasTicket = true;
		ticketDistrib.queue--;
		G.Sys.gameManager.AddMoney (ticketDistrib.price);
		G.Sys.audioManager.PlayBuyTicket ();
	}
}
