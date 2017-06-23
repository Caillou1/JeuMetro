using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour 
{
	public Vector3 startPos;
	public Vector3 endPos;
	public GameObject pathObj;

	void Start () 
	{
		var path = PathFinder.Path (startPos, endPos);
		
		foreach (var p in path) {
			Instantiate (pathObj, p, new Quaternion());
		}

		var startObj = Instantiate (pathObj, startPos, new Quaternion ()) as GameObject;
		startObj.transform.Find("Cube").GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 255);
		startObj.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);
		var endObj = Instantiate (pathObj, endPos, new Quaternion ()) as GameObject;
		endObj.transform.Find ("Cube").GetComponent<MeshRenderer> ().material.color = new Color (255, 0, 0);
		endObj.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);

		var tile = G.Sys.tilemap.at (startPos);
		if (tile .Count == 0)
			Debug.Log ("Poop");
			
	}

	void Update () 
	{
		
	}
}
