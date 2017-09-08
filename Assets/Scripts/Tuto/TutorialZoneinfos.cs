using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZoneinfos : MonoBehaviour 
{
    public List<ObjectifType> types = new List<ObjectifType>();

    List<Vector3i> coveredSurface = new List<Vector3i>();

    void Awake()
    {
        var scale = transform.lossyScale;

        var origine = transform.position - scale / 2 + new Vector3(0.1f, 0.1f, 0.1f);

        for (float i = origine.x;  i < scale.x + origine.x; i++)
            for (float j = origine.y; j <= scale.y + origine.y; j++)
                for (float k = origine.z; k < scale.z + origine.z; k++)
                    coveredSurface.Add(new Vector3i(new Vector3(i, j, k)));
    }

    public bool HaveObjective(ObjectifType type)
    {
        foreach (var t in types)
            if (t == type)
                return true;
        return false;
    }

    public bool isOn(Vector3 pos)
    {
        return isOn(new Vector3i(pos));
    }

    public bool isOn(Vector3i pos)
    {
        foreach (var p in coveredSurface)
            if (p.Equals(pos))
                return true;
        return false;
    }
}
