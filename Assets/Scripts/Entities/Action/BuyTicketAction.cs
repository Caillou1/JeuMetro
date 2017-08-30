using System;
using UnityEngine;

public class BuyTicketAction : AEntityAction<Traveler>
{
	TicketDistribTile ticketDistrib;
	float time = 0;
	float maxTime = 0;

	public BuyTicketAction (Traveler t, Vector3 pos, TicketDistribTile tile) : base(t, ActionType.BUY_TICKET, pos, 1)
	{
		ticketDistrib = tile;
	}

	protected override bool Start ()
	{
		bool canRun = entity.stats.HaveTicket || entity.datas.Fraud;
		if (!canRun)
			ticketDistrib.queue++;
        maxTime = ticketDistrib.queue * G.Sys.constants.BuyTime;
		return canRun;
	}

	protected override bool Update ()
	{
        if (ticketDistrib == null)
            return true;
        
		time += Time.deltaTime;
		return time > maxTime;
	}

	protected override void End ()
	{
        entity.stats.HaveTicket = true;
		ticketDistrib.queue--;
		G.Sys.gameManager.AddMoney (ticketDistrib.price);
		G.Sys.audioManager.PlayBuyTicket ();
	}
}
