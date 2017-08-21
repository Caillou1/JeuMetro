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
	NONE
}

[System.Serializable]
public class Objectif {
	public ObjectifType Type;
	public int Amount;
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
		return true;
	}
}
