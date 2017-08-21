using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
	public List<Tutorial> Tutoriels;
	public bool LaunchOnStart;

	void Start () {
		if (Tutoriels.Count > 0 && LaunchOnStart) {
			StartCoroutine(TutorialRoutine());
		}
	}

	IEnumerator TutorialRoutine() {
		foreach (var t in Tutoriels) {
			G.Sys.menuManager.ShowMessages (t.MessagesToShowBefore);

			yield return new WaitUntil (() => G.Sys.menuManager.AreAllMessagesRead ());

			G.Sys.menuManager.ShowObjectives (t.Objectives);

			yield return new WaitUntil (() => t.ObjectivesDone);

			G.Sys.menuManager.ShowMessages (t.MessagesToShowAfter);

			yield return new WaitUntil (() => G.Sys.menuManager.AreAllMessagesRead ());
		}
	}
}

public enum ObjectifType {
	NONE,
	DROP_INFOPANEL,
	DROP_PODOTACTILE,
	DROP_BENCH,
	DROP_BIN,
	DROP_TICKETDISTRIB,
	DROP_FOODDISTRIB,
	DROP_ESCALATOR,
	DROP_SPEAKER,
	DROP_AGENT,
	DROP_CLEANER,
}

[System.Serializable]
public class Objectif {
	public ObjectifType Type;
	public int Amount;

	private int startAmount;

	public Objectif(ObjectifType t, int amount) {
		Type = t;
		Amount = amount;

		startAmount = ObjectifChecker.GetStartAmount(t);
	}
}

[System.Serializable]
public class Tutorial {
	public List<string> MessagesToShowBefore;
	public List<Objectif> Objectives;
	public List<string> MessagesToShowAfter;

	public bool ObjectivesDone {
		get{
			foreach (var o in Objectives) {
				if (!ObjectifChecker.Check (o)) {
					return false;
				}
			}

			return true;
		}
	}
}

public static class ObjectifChecker {
	public static bool Check(Objectif o) {
		switch (o.Type) {
		case ObjectifType.NONE:
			return false;

		default:
			return true;
		}
	}

	public static int GetStartAmount(ObjectifType type) {
		switch (type) {
		case ObjectifType.NONE:
			return 0;
		case ObjectifType.DROP_AGENT:
			return G.Sys.agentsCount;
		case ObjectifType.DROP_BENCH:
			return G.Sys.GetDisposableCount (TileID.BENCH);
		case ObjectifType.DROP_BIN:
			return G.Sys.GetDisposableCount (TileID.BIN);
		case ObjectifType.DROP_CLEANER:
			return G.Sys.cleanerCount;
		case ObjectifType.DROP_ESCALATOR:
			return G.Sys.GetDisposableCount (TileID.ESCALATOR);
		case ObjectifType.DROP_FOODDISTRIB:
			return G.Sys.GetDisposableCount (TileID.FOODDISTRIBUTEUR);
		case ObjectifType.DROP_INFOPANEL:
			return G.Sys.GetDisposableCount (TileID.INFOPANEL);
		case ObjectifType.DROP_PODOTACTILE:
			return G.Sys.GetDisposableCount (TileID.PODOTACTILE);
		case ObjectifType.DROP_SPEAKER:
			return G.Sys.GetDisposableCount (TileID.SPEAKER);
		case ObjectifType.DROP_TICKETDISTRIB:
			return G.Sys.GetDisposableCount (TileID.TICKETDISTRIBUTEUR);
		default:
			return 0;
		}
	}
}
