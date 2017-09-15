using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
	[BoxGroup("Tutorial")]
	public List<Tutorial> Tutoriels;
	[BoxGroup("After Tutorial")]
	public bool FireAlertAtTheEnd;
	[BoxGroup("After Tutorial")]
	[ShowIf("FireAlertAtTheEnd")]
	public bool ShowMessagesAtTheEnd;
	[BoxGroup("After Tutorial")]
	[ShowIf("ShowMessagesAtTheEnd")]
	public List<string> MessagesAtTheEnd;
	[BoxGroup("After Tutorial")]
	public float TimeBeforeNextScene;
	[BoxGroup("After Tutorial")]
	public string NextScene;

    int currentTutorialIndex = -1;

    SubscriberList subscriberList = new SubscriberList();

    void Awake()
    {
        subscriberList.Add(new Event<AgentPlacedEvent>.Subscriber(OnPlaceAgent));
        subscriberList.Add(new Event<ObjectPlacedEvent>.Subscriber(onPlaceObject));
        subscriberList.Add(new Event<StartDragAgentEvent>.Subscriber(onStartDragAgent));
        subscriberList.Add(new Event<StartDragObjectEvent>.Subscriber(onStartDragObject));
        subscriberList.Add(new Event<AbortDragAgentEvent>.Subscriber(onAbortDragAgent));
        subscriberList.Add(new Event<AbortDragObjectEvent>.Subscriber(onAbortDragObject));
        subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        subscriberList.Unsubscribe();
    }

    void OnPlaceAgent(AgentPlacedEvent e)
    {
		if (currentTutorialIndex < 0)
			return;
        addOne(ObjectifChecker.AgentTypeToObjectif(e.type), e.pos);
        disableZones();
    }

    void onPlaceObject(ObjectPlacedEvent e)
    {
        if (e.bought)
            return;
        
        if (currentTutorialIndex < 0)
            return;
        addOne(ObjectifChecker.TileTypeToObjectif(e.type), e.points);
        disableZones();
    }

    void onStartDragAgent(StartDragAgentEvent e)
    {
		if (currentTutorialIndex < 0)
			return;
        enableZones(ObjectifChecker.AgentTypeToObjectif(e.type));
    }

    void onStartDragObject(StartDragObjectEvent e)
	{
		if (currentTutorialIndex < 0)
			return;
        enableZones(ObjectifChecker.TileTypeToObjectif(e.type));
    }

    void onAbortDragAgent(AbortDragAgentEvent e)
	{
		if (currentTutorialIndex < 0)
			return;
        disableZones();
    }

    void onAbortDragObject(AbortDragObjectEvent e)
	{
		if (currentTutorialIndex < 0)
			return;
        disableZones();
    }

    void enableZones(ObjectifType type)
    {
        foreach (var z in Tutoriels[currentTutorialIndex].zonesToHighlight)
        {
            if (z.HaveObjective(type))
                z.gameObject.SetActive(true);
        }
    }

    void disableZones()
    {
		foreach (var z in Tutoriels[currentTutorialIndex].zonesToHighlight)
			z.gameObject.SetActive(false);
    }

    void addOne(ObjectifType type, List<Vector3i> points)
	{
		bool isOn = false;
		foreach (var z in Tutoriels[currentTutorialIndex].zonesToHighlight)
		{
            foreach (var p in points)
            {
                if (z.HaveObjective(type) && z.isOn(p))
                {
                    isOn = true;
                    break;
                }
            }
            if (isOn)
                break;
		}
		if (!isOn)
			return;
        
		foreach (var o in Tutoriels[currentTutorialIndex].Objectives)
		{
			if (o.Type == type)
				o.CurrentAmount++;
		}
	}

    void addOne(ObjectifType type, Vector3 pos)
    {
        bool isOn = false;
        foreach(var z in Tutoriels[currentTutorialIndex].zonesToHighlight)
        {
            if(z.HaveObjective(type) && z.isOn(pos))
            {
                isOn = true;
                break;
            }
        }
        if (!isOn)
            return;

		foreach (var o in Tutoriels[currentTutorialIndex].Objectives)
		{
            if (o.Type == type)
                o.CurrentAmount++;
		}
    }

	void Start () 
    {
        var sceneName = SceneManager.GetActiveScene().name;
        while (sceneName[0] <= '0' || sceneName[0] > '9')
            sceneName = sceneName.Remove(0, 1);
        int sceneId = 0;
        if (!int.TryParse(sceneName, out sceneId))
            Debug.LogError("Tutorial scene name is wrongly formated !\nIt must be \"Tutorial[id]\" with [id] the index of the tutorial");
        else Event<StartTutorialEvent>.Broadcast(new StartTutorialEvent(sceneId));


        G.Sys.menuManager.EnableUITutoMode();
		
		StartCoroutine(TutorialRoutine());

        foreach (var t in Tutoriels)
            foreach (var z in t.zonesToHighlight)
                z.gameObject.SetActive(false);
	}

    private void Update()
    {
        if (G.Sys.gameManager.GetMoney() < 500)
            G.Sys.gameManager.AddMoney(500);
    }

    IEnumerator TutorialRoutine() {
		yield return new WaitForSeconds (Time.deltaTime * 5);
        for (int i = 0; i < Tutoriels.Count; i++)
        {
            var t = Tutoriels[i];

			SpawnWave (t);

			yield return new WaitForSeconds (t.TimeBeforeFirstMessages);

			G.Sys.menuManager.ShowMessages (t.MessagesToShowBefore);

			yield return new WaitUntil (() => G.Sys.menuManager.AreAllMessagesRead ());
            currentTutorialIndex = i;

			G.Sys.menuManager.ShowObjectives (t.Objectives);

            yield return new WaitUntil(() => { G.Sys.menuManager.ShowObjectives(t.Objectives); return t.ObjectivesDone; });
            disableZones();
            currentTutorialIndex = -1;

			yield return new WaitForSeconds(1f);
			G.Sys.menuManager.HideObjectives();

			yield return new WaitForSeconds (t.TimeBeforeLastMessages);

			G.Sys.menuManager.ShowMessages (t.MessagesToShowAfter);

			yield return new WaitUntil (() => G.Sys.menuManager.AreAllMessagesRead ());

			yield return new WaitForSeconds (1f);
		}

		if(FireAlertAtTheEnd)
			StartFireAlert ();

		yield return new WaitForSeconds (TimeBeforeNextScene);

		if (ShowMessagesAtTheEnd) {
			G.Sys.menuManager.ShowMessages (MessagesAtTheEnd);

			yield return new WaitUntil (() => G.Sys.menuManager.AreAllMessagesRead ());
		}

		if (NextScene != "") {
            Event<FinishTutorialEvent>.Broadcast(new FinishTutorialEvent());
			G.Sys.tilemap.clear ();
			SceneManager.LoadScene (NextScene);
		}
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

    [HideInInspector]
    public int CurrentAmount = 0;

	public Objectif(ObjectifType t, int amount) {
		Type = t;
		Amount = amount;
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
    public List<TutorialZoneinfos> zonesToHighlight;
	public float TimeBeforeFirstMessages;
	public List<string> MessagesToShowBefore;
	public float TimeBeforeLastMessages;
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
        return o.CurrentAmount >= o.Amount;
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

    public static ObjectifType AgentTypeToObjectif(AgentType agent)
    {
        switch(agent)
        {
            case AgentType.AGENT:
                return ObjectifType.DROP_AGENT;
            case AgentType.CLEANER:
                return ObjectifType.DROP_CLEANER;
        }

        return ObjectifType.NONE;
    }

    public static ObjectifType TileTypeToObjectif(TileID tile)
    {
        switch(tile)
        {
            case TileID.BENCH:
                return ObjectifType.DROP_BENCH;
            case TileID.BIN:
                return ObjectifType.DROP_BIN;
            case TileID.ESCALATOR:
                return ObjectifType.DROP_ESCALATOR;
            case TileID.FOODDISTRIBUTEUR:
                return ObjectifType.DROP_FOODDISTRIB;
            case TileID.INFOPANEL:
                return ObjectifType.DROP_INFOPANEL;
            case TileID.PODOTACTILE:
                return ObjectifType.DROP_PODOTACTILE;
            case TileID.SPEAKER:
                return ObjectifType.DROP_SPEAKER;
            case TileID.TICKETDISTRIBUTEUR:
                return ObjectifType.DROP_TICKETDISTRIB;
        }
        return ObjectifType.NONE;
    }
}
