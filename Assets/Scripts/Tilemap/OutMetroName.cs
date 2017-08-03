using System;
using UnityEngine;

public class OutMetroName : MonoBehaviour
{
	public string exitName;

	public static OutMetroName findInParent(Transform obj)
	{
		while (obj.parent != null) {
			obj = obj.parent;
			var comp = obj.GetComponent<OutMetroName> ();
			if (comp != null)
				return comp;
		}

		return null;
	}
}

