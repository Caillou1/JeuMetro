using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
	public List<Tutorial> Tutoriels;
	public bool LaunchOnStart;

	void Start () {
		if (LaunchOnStart) {
			StartCoroutine(TutorialRoutine());
		}
	}

	IEnumerator TutorialRoutine() {
		foreach (var t in Tutoriels) {
			SpawnWave (t);

			yield return new WaitForSeconds (t.TimeBeforeFirstMessages);

			G.Sys.menuManager.ShowMessages (t.MessagesToShowBefore);

			yield return new WaitUntil (() => G.Sys.menuManager.AreAllMessagesRead ());

			G.Sys.menuManager.ShowObjectives (t.Objectives);

			yield return new WaitUntil (() => t.ObjectivesDone);

			G.Sys.menuManager.HideObjectives ();

			yield return new WaitForSeconds (t.TimeBeforeLastMessages);

			G.Sys.menuManager.ShowMessages (t.MessagesToShowAfter);

			yield return new WaitUntil (() => G.Sys.menuManager.AreAllMessagesRead ());

			yield return new WaitForSeconds (1f);

			if (t.NextScene != "") {
				G.Sys.tilemap.clear ();
				SceneManager.LoadScene (t.NextScene);
			}
		}

		//SceneManager.LoadScene (0);

		StartFireAlert ();
	}

	void SpawnWave(Tutorial t) {
		if(t.WaveToSpawn != null) Instantiate (t.WaveToSpawn, t.Entrance.position, Quaternion.identity, transform);
	}

	void StartFireAlert() {
		G.Sys.audioManager.StartFireMusic ();
		G.Sys.gameManager.FireAlert = true;
		Event<StartFireAlertEvent>.Broadcast (new StartFireAlertEvent ());
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
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
	[HideIf("TypeIsNone")]
	public int Amount;

	private int startAmount;
	public int StartAmount {
		get{
			return startAmount;
		}
	}
	public Objectif(ObjectifType t, int amount) {
		Type = t;
		Amount = amount;

		startAmount = ObjectifChecker.GetStartAmount(t);
	}

	private bool TypeIsNone() {
		return Type == ObjectifType.NONE;
	}

	public override string ToString ()
	{
		return ObjectifChecker.GetCorrespondantText (this);
	}
}

[System.Serializable]
public class Tutorial {
	public bool SpawnWave;
	[ShowIf("SpawnWave")]
	public GameObject WaveToSpawn;
	[ShowIf("SpawnWave")]
	public Transform Entrance;
	public List<Objectif> Objectives;
	public float TimeBeforeFirstMessages;
	public List<string> MessagesToShowBefore;
	public float TimeBeforeLastMessages;
	public List<string> MessagesToShowAfter;
	public string NextScene;

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
			return true;
		case ObjectifType.DROP_AGENT:
			return (G.Sys.agentsCount - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_BENCH:
			return (G.Sys.GetDisposableCount (TileID.BENCH) - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_BIN:
			return (G.Sys.GetDisposableCount (TileID.BIN) - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_CLEANER:
			return (G.Sys.cleanerCount - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_ESCALATOR:
			return (G.Sys.GetDisposableCount (TileID.ESCALATOR) - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_FOODDISTRIB:
			return (G.Sys.GetDisposableCount (TileID.FOODDISTRIBUTEUR) - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_INFOPANEL:
			return (G.Sys.GetDisposableCount (TileID.INFOPANEL) - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_PODOTACTILE:
			return (G.Sys.GetDisposableCount (TileID.PODOTACTILE) - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_SPEAKER:
			return (G.Sys.GetDisposableCount (TileID.SPEAKER) - o.StartAmount) >= o.Amount;
		case ObjectifType.DROP_TICKETDISTRIB:
			return (G.Sys.GetDisposableCount (TileID.TICKETDISTRIBUTEUR) - o.StartAmount) >= o.Amount;
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

	public static string GetCorrespondantText(Objectif o) {
		switch (o.Type) {
		case ObjectifType.NONE:
			return "";
		case ObjectifType.DROP_AGENT:
			return "- Poser " + o.Amount + " agent" + ((o.Amount > 1) ? "s" : "") + " de sécurité";
		case ObjectifType.DROP_BENCH:
			return "- Poser " + o.Amount + " banc" + ((o.Amount > 1) ? "s" : "");
		case ObjectifType.DROP_BIN:
			return "- Poser " + o.Amount + " poubelle" + ((o.Amount > 1) ? "s" : "");
		case ObjectifType.DROP_CLEANER:
			return "- Poser " + o.Amount + " agent" + ((o.Amount > 1) ? "s" : "") + " d'entretien";
		case ObjectifType.DROP_ESCALATOR:
			return "- Poser " + o.Amount + " escalator" + ((o.Amount > 1) ? "s" : "");
		case ObjectifType.DROP_FOODDISTRIB:
			return "- Poser " + o.Amount + " distributeur" + ((o.Amount > 1) ? "s" : "") + " de nourriture";
		case ObjectifType.DROP_INFOPANEL:
			return "- Poser " + o.Amount + " panneau" + ((o.Amount > 1) ? "x" : "") + " d'informations";
		case ObjectifType.DROP_PODOTACTILE:
			return "- Poser " + o.Amount + " bande" + ((o.Amount > 1) ? "s" : "") + " podotactile";
		case ObjectifType.DROP_SPEAKER:
			return "- Poser " + o.Amount + " émetteur" + ((o.Amount > 1) ? "s" : "") + " sonore";
		case ObjectifType.DROP_TICKETDISTRIB:
			return "- Poser " + o.Amount + " distributeur" + ((o.Amount > 1) ? "s" : "") + " de tickets";
		default:
			return "";
		}
	}
}
