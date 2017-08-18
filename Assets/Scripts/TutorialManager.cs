using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {


	void Start () {
		
	}
}

public enum Objectif {
	NONE
}

[System.Serializable]
public class Tutorial {
	public List<string> MessagesToShow;
	public List<Objectif> Objectives;

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
