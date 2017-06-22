using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileInfos
{
	public TileInfos(ATile _tile, bool _canBeConnected, bool _preventConnexions, int _priority)
	{
		priority = _priority;
		canBeConnected = _canBeConnected;
		preventConnexions = _preventConnexions;
		tile = _tile;
	}

	public int priority;
	public bool canBeConnected;
	public bool preventConnexions;
	public ATile tile;
}

public class TilemapInfo
{
	public TilemapInfo(Vector3i _pos)
	{
		pos = _pos;
	}

	public readonly Vector3i pos;
	public List<TileInfos> tiles = new List<TileInfos> ();
}

public class Tilemap
{
	public const int GROUND_PRIORITY = 0;
	public const int ESCALATOR_PRIORITY = 10;

	private List<TilemapInfo> tiles = new List<TilemapInfo> ();
	//private Dictionary<Vector3i, List<TileInfos>> tiles = new Dictionary<Vector3i, List<TileInfos>> ();

	/// <summary>
	/// Ajoute une tile connectable à la position demandé.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	/// <param name="priority">Priority.</param>
	public void addTile(Vector3 pos, ATile tile, int priority = 0)
	{
		addTile (new Vector3i (pos), tile, priority);
	}

	/// <summary>
	/// Ajoute une tile connectable à la position demandé.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	/// <param name="priority">Priority.</param>
	public void addTile(Vector3i pos, ATile tile, int priority = 0)
	{
		addTile (pos, tile, true, false, priority);
	}

	/// <summary>
	/// Ajoute une tile qui n'est pas connectable si canBConnected est faux.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	/// <param name="canBeConnected">If set to <c>true</c> can be connected.</param>
	/// <param name="priority">Priority.</param>
	public void addTile(Vector3 pos, ATile tile, bool canBeConnected, int priority = 0)
	{
		addTile (new Vector3i (pos), tile, canBeConnected, priority);
	}

	/// <summary>
	/// Ajoute une tile qui n'est pas connectable si canBConnected est faux.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	/// <param name="canBeConnected">If set to <c>true</c> can be connected.</param>
	/// <param name="priority">Priority.</param>
	public void addTile(Vector3i pos, ATile tile, bool canBeConnected, int priority = 0)
	{
		addTile (pos, tile, canBeConnected, false, priority);
	}

	/// <summary>
	/// Si preventConnexion est a vrais, toutes les tiles connectable à cette position ne pourrons pas etre connecté.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	/// <param name="canBeConnected">If set to <c>true</c> can be connected.</param>
	/// <param name="preventConnexions">If set to <c>true</c> prevent connexions.</param>
	/// <param name="priority">Priority.</param>
	public void addTile(Vector3 pos, ATile tile, bool canBeConnected, bool preventConnexions, int priority = 0)
	{
		addTile (new Vector3i (pos), tile, canBeConnected, preventConnexions, priority);
	}

	/// <summary>
	/// Si preventConnexion est a vrais, toutes les tiles connectable à cette position ne pourrons pas etre connecté.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	/// <param name="canBeConnected">If set to <c>true</c> can be connected.</param>
	/// <param name="preventConnexions">If set to <c>true</c> prevent connexions.</param>
	/// <param name="priority">Priority.</param>
	public void addTile(Vector3i pos, ATile tile, bool canBeConnected, bool preventConnexions, int priority = 0)
	{
		var t = map (pos);
		if(!t.tiles.Exists(it => it.tile == tile))
			t.tiles.Add(new TileInfos(tile, canBeConnected, preventConnexions, priority));
	}

	/// <summary>
	/// Suprime la tile spécifiée.
	/// </summary>
	/// <returns><c>true</c>, if tile was deleted, <c>false</c> otherwise.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	public bool delTile(Vector3 pos, ATile tile)
	{
		return delTile (new Vector3i (pos), tile);
	}

	/// <summary>
	/// Suprime la tile spécifiée.
	/// </summary>
	/// <returns><c>true</c>, if tile was deleted, <c>false</c> otherwise.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="tile">Tile.</param>
	public bool delTile(Vector3i pos, ATile tile)
	{
		return map (pos).tiles.RemoveAll (it => it.tile == tile) > 0;
	}

	/// <summary>
	/// Retourne toutes les tiles à la position spécifiée.
	/// </summary>
	/// <param name="pos">Position.</param>
	public List<ATile> at(Vector3 pos)
	{
		return at (new Vector3i (pos));
	}

	/// <summary>
	/// Retourne toutes les tiles à la position spécifiée.
	/// </summary>
	/// <param name="pos">Position.</param>
	public List<ATile> at(Vector3i pos)
	{
		List<ATile> list = new List<ATile> ();
		foreach (var t in map(pos).tiles)
			list.Add (t.tile);
		return list;
	}

	/// <summary>
	/// Retourne toutes les tiles d'un certain type à la position demandé
	/// </summary>
	/// <returns>The <see cref="System.Collections.Generic.List`1[[ATile]]"/>.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="id">Identifier.</param>
	public List<ATile> tilesOfTypeAt(Vector3 pos, TileID id)
	{
		return tilesOfTypeAt (new Vector3i (pos), id);
	}

	/// <summary>
	/// Retourne toutes les tiles d'un certain type à la position demandé
	/// </summary>
	/// <returns>The <see cref="System.Collections.Generic.List`1[[ATile]]"/>.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="id">Identifier.</param>
	public List<ATile> tilesOfTypeAt(Vector3i pos, TileID id)
	{
		var list = at (pos);
		List<ATile> returnlist = new List<ATile> ();
		foreach (var t in list)
			if (t.type == id)
				returnlist.Add (t);
		return returnlist;
	}

	/// <summary>
	/// Retourne vrais si une tile à cette position est connectable.
	/// </summary>
	/// <param name="pos">Position.</param>
	public bool connectable(Vector3 pos)
	{
		return connectable (new Vector3i (pos));
	}

	/// <summary>
	/// Retourne vrais si une tile à cette position est connectable.
	/// </summary>
	/// <param name="pos">Position.</param>
	public bool connectable(Vector3i pos)
	{
		bool isConnectable = false;
		foreach (var tile in map(pos).tiles) {
			if (tile.preventConnexions)
				return false;
			if (tile.canBeConnected)
				isConnectable = true;
		}
		return isConnectable;
	}

	/// <summary>
	/// Retourne toutes les tiles connectables à la position spécifiée.
	/// </summary>
	/// <returns>The tiles.</returns>
	/// <param name="pos">Position.</param>
	public List<ATile> connectableTiles(Vector3 pos)
	{
		return connectableTiles (new Vector3i (pos));
	}

	/// <summary>
	/// Retourne toutes les tiles connectables à la position spécifiée.
	/// </summary>
	/// <returns>The tiles.</returns>
	/// <param name="pos">Position.</param>
	public List<ATile> connectableTiles(Vector3i pos)
	{
		List<ATile> list = new List<ATile> ();
		foreach (var tile in map(pos).tiles) {
			if (tile.preventConnexions)
				return new List<ATile> ();
			if (tile.canBeConnected)
				list.Add (tile.tile);
		}
		return list;
	}

	/// <summary>
	/// Retourne la tile connectable avec la priorité la plus elevé.
	/// Retourne null si aucune tile n'est trouvable
	/// </summary>
	/// <returns>The tile.</returns>
	/// <param name="pos">Position.</param>
	public ATile connectableTile(Vector3 pos)
	{
		return connectableTile (new Vector3i (pos));
	}

	/// <summary>
	/// Retourne la tile connectable avec la priorité la plus elevé.
	/// Retourne null si aucune tile n'est trouvable
	/// </summary>
	/// <returns>The tile.</returns>
	/// <param name="pos">Position.</param>
	public ATile connectableTile(Vector3i pos)
	{
		int bestValue = int.MinValue;
		bool bestCanBeConnected = false;
		ATile bestTile = null;
		foreach (var tile in map(pos).tiles) {
			if (tile.preventConnexions)
				return null;
			if (tile.priority > bestValue) {
				bestTile = tile.tile;
				bestCanBeConnected = tile.canBeConnected;
				bestValue = tile.priority;
			}
		}
		return bestCanBeConnected ? bestTile : null;
	}

	/// <summary>
	/// Retourne le tileinfo associé à la tile et à la position spécifiée.
	/// </summary>
	/// <returns>The infos of.</returns>
	/// <param name="tile">Tile.</param>
	/// <param name="pos">Position.</param>
	public TileInfos tileInfosOf(ATile tile, Vector3 pos)
	{
		return tileInfosOf (tile, new Vector3i (pos));
	}

	/// <summary>
	/// Retourne le tileinfo associé à la tile et à la position spécifiée.
	/// </summary>
	/// <returns>The infos of.</returns>
	/// <param name="tile">Tile.</param>
	/// <param name="pos">Position.</param>
	public TileInfos tileInfosOf(ATile tile, Vector3i pos)
	{
		return map (pos).tiles.Find (it => it.tile == tile);
	}

	private TilemapInfo map(Vector3i pos)
	{
		foreach (var t in tiles) {
			if (t.pos.x == pos.x && t.pos.y == pos.y && t.pos.z == pos.z)
				return t;
		}
		tiles.Add(new TilemapInfo(pos));
		return sort (tiles [tiles.Count - 1]);
	}

	private static List<TileInfos> sort(List<TileInfos> list)
	{
		list.Sort (delegate(TileInfos a, TileInfos b) {
			if(a.priority > b.priority)
				return 1;
			else if (a.priority < b.priority)
				return -1;
			else return 0;
		});
		return list.Reverse ();
	}
}
