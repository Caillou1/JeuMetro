using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tilemap
{
    private Dictionary<Vector3i, ATile> tiles = new Dictionary<Vector3i, ATile>();
    private Dictionary<Vector3i, List<AObject>> objects = new Dictionary<Vector3i, List<AObject>>();

    public ATile At(Vector3 pos)
    {
        return At(new Vector3i(pos));
    }

    public ATile At(Vector3i pos)
    {
        if (tiles.ContainsKey(pos))
            return tiles[pos];
        return null;
    }

    public void SetAt(Vector3 pos, ATile value)
    {
        SetAt(new Vector3i(pos), value);
    }

    public void SetAt(Vector3i pos, ATile value)
    {
        if (!tiles.ContainsKey(pos))
            tiles.Add(pos, value);
        else tiles[pos] = value;
    }

    public void ConnectAll()
    {
        foreach (var tile in tiles)
            tile.Value.Connect();
    }

    public void Clear()
    {
        tiles.Clear();
        objects.Clear();
    }

    public List<AObject> ObjectsAt(Vector3 pos)
    {
        return ObjectsAt(new Vector3i(pos));
    }

    public List<AObject> ObjectsAt(Vector3i pos)
    {
        if (objects.ContainsKey(pos))
            return objects[pos];
        return new List<AObject>();
    }

    public void AddObjectAt(Vector3 pos, AObject o)
    {
        AddObjectAt(new Vector3i(pos), o);
    }

    public void AddObjectAt(Vector3i pos, AObject o)
    {
        if (!objects.ContainsKey(pos))
            objects[pos] = new List<AObject>();
        objects[pos].Add(o);
    }

    public bool DelObjectAt(Vector3 pos, AObject o)
    {
        return DelObjectAt(new Vector3i(pos), o);
    }

    public bool DelObjectAt(Vector3i pos, AObject o)
    {
        if (!objects.ContainsKey(pos))
            return false;
        return objects[pos].Remove(o);
    }
}
