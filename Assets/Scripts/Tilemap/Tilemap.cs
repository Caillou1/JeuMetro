using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using NRand;

public class TileInfos
{
	public TileInfos(ATile _tile, int _priority)
	{
		priority = _priority;
		tile = _tile;
	}

	public int priority;
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
	public const int LOW_PRIORITY = -10;
	public const int GROUND_PRIORITY = 0;
	public const int EXITS_PRIORITY = 5;
	public const int INFOPANEL_PRIORITY = 10;
	public const int BENCH_PRIORITY = 10;
	public const int DISTRIBUTEUR_PRIORITY = 10;
	public const int ESCALATOR_PRIORITY = 10;
    public const int BIN_PRIORITY = 10;
	public const int STAIRS_PRIORITY = 10;
	public const int ELEVATOR_PRIORITY = 10;
	public const int SPEAKER_PRIORITY = 10;
	public const int WASTE_PRIORITY = 10;
	public const int CONTROLE_LINE_PRIORITY = 15;

	private Dictionary<Vector3i, List<TileInfos>> tiles = new Dictionary<Vector3i, List<TileInfos>>();

	private Dictionary<TileID, List<Vector3i>> specialTiles = new Dictionary<TileID, List<Vector3i>> ();

	private Bounds bounds;

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
		var t = map (pos);
		if(!t.Exists(it => it.tile == tile))
			t.Add(new TileInfos(tile, priority));
		sort (t);
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
		return map (pos).RemoveAll (it => it.tile == tile) > 0;
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
		foreach (var t in map(pos))
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
		List<ATile> returnlist = new List<ATile> ();
		foreach (var t in map(pos))
			if (t.tile.type == id)
				returnlist.Add (t.tile);
		return returnlist;
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
		return map (pos).Find (it => it.tile == tile);
	}

	private List<TileInfos> map(Vector3i pos)
	{
		if (!tiles.ContainsKey (pos))
			tiles.Add (pos, new List<TileInfos> ());
		return tiles [pos];
	}

	private static void sort(List<TileInfos> list)
	{
		list.Sort (delegate(TileInfos a, TileInfos b) {
			if(a.priority < b.priority)
				return 1;
			else if (a.priority > b.priority)
				return -1;
			else return 0;
		});
	}

	/// <summary>
	/// Clear the tilemap
	/// </summary>
	public void clear()
	{
		tiles.Clear ();
		specialTiles.Clear ();
        currentUsedSpace = 0;
        maxUsedSpace = 0;
        G.Sys.clear();
	}

	/// <summary>
	/// Ajoute une tile speciale à une position pour l'identifier plus facilement.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="pos">Position.</param>
	public void addSpecialTile(TileID id, Vector3 pos)
	{
		addSpecialTile (id, new Vector3i (pos));
	}

	/// <summary>
	/// Ajoute une tile speciale à une position pour l'identifier plus facilement.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="pos">Position.</param>
	public void addSpecialTile(TileID id, Vector3i pos)
	{
		if (!specialTiles.ContainsKey (id))
			specialTiles.Add (id, new List<Vector3i> ());
		foreach (var t in specialTiles[id]) 
		{
			if (t.x == pos.x && t.y == pos.y && t.z == pos.z) 
				return;
		}
		specialTiles [id].Add (pos);
	}

	/// <summary>
	/// Suprimme la tile speciale,
	/// </summary>
	/// <returns><c>true</c>, if special tile was deled, <c>false</c> otherwise.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="pos">Position.</param>
	public bool delSpecialTile(TileID id, Vector3 pos)
	{
		return delSpecialTile (id, new Vector3i (pos));
	}

	/// <summary>
	/// Suprimme la tile speciale,
	/// </summary>
	/// <returns><c>true</c>, if special tile was deled, <c>false</c> otherwise.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="pos">Position.</param>
	public bool delSpecialTile(TileID id, Vector3i pos)
	{
		if (!specialTiles.ContainsKey (id))
			return false;
		return specialTiles [id].RemoveAll (p => p.x == pos.x && p.y == pos.y && p.z == pos.z) > 0; 
	}

	/// <summary>
	/// Récupere toutes les tiles spéciales d'un certain type.
	/// </summary>
	/// <returns>The special tiles.</returns>
	/// <param name="id">Identifier.</param>
	public List<Vector3> getSpecialTiles(TileID id)
	{
		List<Vector3> poss = new List<Vector3> ();
		foreach (var p in getSpecialTilesI(id)) {
			poss.Add (p.toVector3 ());
		}
		return poss;
	}

	/// <summary>
	/// Récupere toutes les tiles spéciales d'un certain type en Vector3i.
	/// </summary>
	/// <returns>The special tiles.</returns>
	/// <param name="id">Identifier.</param>
	public List<Vector3i> getSpecialTilesI(TileID id)
	{
		if(!specialTiles.ContainsKey(id))
			return new List<Vector3i>();
		return specialTiles [id];
	}

	/// <summary>
	/// Test si une tile spéciale existe à cette position
	/// </summary>
	/// <returns>The <see cref="System.Boolean"/>.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="pos">Position.</param>
	public bool haveSpecialTileAt(TileID id, Vector3 pos)
	{
		return haveSpecialTileAt (id, new Vector3i (pos));
	}

	/// <summary>
	/// Test si une tile spéciale existe à cette position
	/// </summary>
	/// <returns>The <see cref="System.Boolean"/>.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="pos">Position.</param>
	public bool haveSpecialTileAt(TileID id, Vector3i pos)
	{
		if (!specialTiles.ContainsKey (id))
			return false;
		foreach (var p in specialTiles[id])
			if (p.x == pos.x && p.y == pos.y && p.z == pos.z)
				return true;
		return false;
	}


	/// <summary>
	/// Retourne une tile aléatoire à une position contenant uniquement une tile GROUND
	/// </summary>
	/// <returns>The random ground tile.</returns>
	public ATile getRandomGroundTile() 
	{
		List<ATile> validTiles = new List<ATile> ();
		foreach (var t in tiles) {
			if (t.Value.Count != 1 || t.Value [0].tile.type != TileID.GROUND)
				continue;
			validTiles.Add (t.Value[0].tile);
		}
		return validTiles [new UniformIntDistribution (validTiles.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];

	}

	/// <summary>
	/// Retourne une liste de tile speciale dans un rayon autour d'un point.
	/// </summary>
	/// <returns>The surronding special tile.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="radius">Radius.</param>
	/// <param name="verticalAmplification">Multiplieur sur le poid vertical.</param>
	public List<Vector3> getSurrondingSpecialTile(Vector3 pos, TileID id, float radius, float verticalAmplification = 1)
	{
		var list = getSurrondingSpecialTile (new Vector3i (pos), id, radius, verticalAmplification);
		List<Vector3> returnList = new List<Vector3> ();
		foreach (var t in list)
			returnList.Add (t.toVector3 ());
		return returnList;
	}

	/// <summary>
	/// Retourne une liste de tile speciale dans un rayon autour d'un point.
	/// </summary>
	/// <returns>The surronding special tile.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="radius">Radius.</param>
	/// <param name="verticalAmplification">Multiplieur sur le poid vertical.</param>
	public List<Vector3i> getSurrondingSpecialTile(Vector3i pos, TileID id, float radius, float verticalAmplification = 1)
	{
		List<Vector3i> validTiles = new List<Vector3i> ();
		foreach (var t in getSpecialTilesI (id)) {
			var dir = pos.toVector3 () - t.toVector3 ();
			if (new Vector2 (dir.x, dir.z).magnitude + dir.y * verticalAmplification < radius)
				validTiles.Add (t);
		}
		return validTiles;
	}

	public ATile GetTileOfTypeAt(Vector3 pos, TileID id) {
		var tiles = at (pos);
		foreach (var t in tiles) {
			if (t.type == id) {
				return t;
			}
		}
		return null;
	}

	public ATile GetTileOfTypeAt(Vector3i pos, TileID id)
	{
		var tiles = at(pos);
		foreach (var t in tiles)
		{
			if (t.type == id)
			{
				return t;
			}
		}
		return null;
	}

	/// <summary>
	/// Retourne la tile spéciale la plus proche du type spécifiée.
	/// La seconde valeur de la paire est à faux si aucune tile n'a été trouvée.
	/// </summary>
	/// <returns>The nearest special tile of type.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="verticalAmplification">Vertical amplification.</param>
	public Pair<Vector3, bool> getNearestSpecialTileOfType(Vector3 pos, TileID id, float verticalAmplification = 1, float maxDistance = float.MaxValue)
	{
		var tile = getNearestSpecialTileOfType (new Vector3i (pos), id, verticalAmplification, maxDistance);
		return new Pair<Vector3, bool> (tile.First.toVector3 (), tile.Second);
	}

	// <summary>
	/// Retourne la tile spéciale la plus proche du type spécifiée.
	/// La seconde valeur de la paire est à faux si aucune tile n'a été trouvée.
	/// </summary>
	/// <returns>The nearest special tile of type.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="verticalAmplification">Vertical amplification.</param>
	public Pair<Vector3i, bool>  getNearestSpecialTileOfType(Vector3i pos, TileID id, float verticalAmplification = 1, float maxDistance = float.MaxValue)
	{
		var list = getSpecialTilesI (id);
		if (list.Count == 0)
			return new Pair<Vector3i, bool> (new Vector3i (0, 0, 0), false);

		Vector3i bestTile = new Vector3i (0, 0, 0);
		float bestDist = float.MaxValue;
		bool found = false;
		foreach (var t in list) {
			var dir = pos.toVector3 () - t.toVector3 ();
			var dist = new Vector2 (dir.x, dir.z).magnitude + Mathf.Abs(dir.y * verticalAmplification);
			if (dist > maxDistance)
				continue;
			if (dist < bestDist) {
				bestDist = dist;
				bestTile = t;
				found = true;
			}
		}
		return new Pair<Vector3i, bool>(bestTile, found);
	}

	/// <summary>
	/// Met a jour les dimentions de la map.
	/// A appeler si la map à changé de taille.
	/// </summary>
	public void UpdateGlobalBounds()
	{
		Vector3 min = new Vector3 (float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 max = new Vector3 (float.MinValue, float.MinValue, float.MinValue);

		foreach (var t in tiles) {
			var pos = t.Key.toVector3 ();
			min = new Vector3 (Mathf.Min (min.x, pos.x), Mathf.Min (min.y, pos.y), Mathf.Min (min.z, pos.z));
			max = new Vector3 (Mathf.Max (max.x, pos.x), Mathf.Max (max.y, pos.y), Mathf.Max (max.z, pos.z));
		}

		bounds = new Bounds ((min + max) / 2.0f, (max - min) * 	2f);
	}

	/// <summary>
	/// Retourne la surface prise par la map.
	/// </summary>
	/// <returns>The bounds.</returns>
	public Bounds GlobalBounds()
	{
		return bounds;
	}

	public bool HasGroundAt(Vector3 pos) {
		var tiles = at (pos);

		foreach (var tile in tiles) {
			if (tile.type == TileID.GROUND || tile.type == TileID.OUT)
				return true;
		}

		return false;
	}

	public bool HasEmptyWallAt(Vector3 pos) {
		var tiles = at (pos);

		foreach (var tile in tiles) {
			if (tile.type == TileID.EMPTYWALL)
				return true;
		}

		return false;
	}

	public bool IsEmptyGround(Vector3 pos)
	{
		var validTiles = new TileID[]{ TileID.GROUND, TileID.PODOTACTILE, TileID.WASTE };

		var tiles = at (pos);
		if (tiles.Count == 0)
			return false;

		if (!tiles.Exists (t => t.type == TileID.GROUND))
			return false;
		
		foreach (var t in tiles) {
			if (!validTiles.Contains (t.type))
				return false;
		}
		return true;
	}

	public bool IsEmpty(Vector3 pos) {
		var tiles = at (pos);

		foreach (var tile in tiles) {
			if (tile.type != TileID.EMPTYWALL)
				return false;
		}

		return true;
	}

    private class ElevatorConnexionsInfos
    {
        public ElevatorConnexionsInfos(ElevatorTile _tile)
        {
            tile = _tile;
            exits = new Dictionary<int, List<ElevatorTile>>();
        }

        public ElevatorTile tile;
        public Dictionary<int, List<ElevatorTile>> exits;
    }

    private class VisitedElevators
    {
        public VisitedElevators(int _parent, ElevatorTile _child, int _floor)
        {
            parent = _parent;
            child = _child;
            floor = _floor;
        }

        public int parent;
        public ElevatorTile child;
        public int floor;
    }

    private List<ElevatorConnexionsInfos> elevatorConnections2 = new List<ElevatorConnexionsInfos>();

    public void CreateElevatorsConnections()
    {
        elevatorConnections2.Clear();

		NavMeshPath path = new NavMeshPath();

		var elevators = getSpecialTilesI(TileID.ELEVATOR);

        foreach(var pos in elevators)
        {
            var tile = GetTileOfTypeAt(pos, TileID.ELEVATOR) as ElevatorTile;
            if (tile == null)
                continue;

            var connexionsInfos = new ElevatorConnexionsInfos(tile);
            elevatorConnections2.Add(connexionsInfos);

            foreach (var floor in tile.GetFloors())
            {
                List<ElevatorTile> exits = new List<ElevatorTile>();
                connexionsInfos.exits.Add(floor, exits);

                foreach(var pos2 in elevators)
                {
                    if (pos2.Equals(pos))
                        continue;
                    
					var tile2 = GetTileOfTypeAt(pos, TileID.ELEVATOR) as ElevatorTile;
					if (tile2 == null)
						continue;

                    if (!tile2.FloorExists(floor))
                        continue;

                    var startPos = tile.GetWaitZone(floor);
                    var endPos = tile2.GetWaitZone(floor);


                    NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);
                    if (path.status != NavMeshPathStatus.PathComplete)
                        continue;
                    bool pathValid = true;
                    foreach (var c in path.corners)
                    {
                        if (Mathf.RoundToInt(c.y) != floor)
                        {
                            pathValid = false;
                            break;
                        }
                    }
                    if (!pathValid)
                        continue;
                    exits.Add(tile2);
                }
            }
        }

    }

    /*private Dictionary<int, Dictionary<int, List<List<Pair<int, ElevatorTile>>>>> elevatorsConnections;

	public void CreateElevatorsConnections() {
		//System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
		//watch.Start ();

		//INITIALISATION
		elevatorsConnections = new Dictionary<int, Dictionary<int, List<List<Pair<int, ElevatorTile>>>>>();
		var positions = getSpecialTiles (TileID.ELEVATOR);
		List<int> Floors = new List<int> ();

		//Détermine l'étage min et max
		foreach (var pos in positions) {
			var fl = Mathf.RoundToInt (pos.y);
			if (!Floors.Contains (fl))
				Floors.Add (fl);
		}

		//Parcourt les étages du min au max pour établir les chemins possibles
		foreach (var floor in Floors) {
			var posAtFloor = GetElevatorsPosAtFloor (floor);

			if (posAtFloor.Count > 0) {
				elevatorsConnections.Add (floor, new Dictionary<int, List<List<Pair<int, ElevatorTile>>>> ());
				var elevatorsAtFloor = new List<ElevatorTile> ();

				//Référence tous les ascenseurs à cet étage
				foreach (var pos in posAtFloor) {
					var et = GetTileOfTypeAt (pos, TileID.ELEVATOR) as ElevatorTile;
					if (!elevatorsAtFloor.Contains (et)) {
						elevatorsAtFloor.Add (et);
					}
				}

				foreach(var potentialFloor in Floors) {
					elevatorsConnections [floor].Add (potentialFloor, null);
				}

				//Parcourt les ascenseurs pour ajouter les étages desservis
				foreach (var elevator in elevatorsAtFloor) {
					foreach (var f in elevator.GetFloors()) {
						if (floor != f) {
							if (elevatorsConnections [floor] [f] == null) {
								elevatorsConnections [floor] [f] = new List<List<Pair<int, ElevatorTile>>> ();
							}

							elevatorsConnections [floor] [f].Add (new List<Pair<int, ElevatorTile>> ());
							var pair = new Pair<int, ElevatorTile> ();
							pair.First = f;
							pair.Second = elevator;
							elevatorsConnections [floor] [f].Last ().Add (pair);
						}
					}
				}
			}
		}

		//Parcourt des chemins possibles pour ajouter les ascenseurs intermédiaires
		bool HasChanged = true;

		while (HasChanged) {
			HasChanged = false;

			foreach (var originFloorConnections in elevatorsConnections.ToList()) {
				foreach (var connectedFloor in originFloorConnections.Value.ToList()) {
					if (connectedFloor.Value != null && connectedFloor.Value.Count > 0) {
						foreach (var connectedFloorConnections in elevatorsConnections[connectedFloor.Key].ToList()) {
							if (connectedFloorConnections.Value != null && connectedFloorConnections.Value.Count > 0) {
								if (originFloorConnections.Value [connectedFloorConnections.Key] == null || originFloorConnections.Value [connectedFloorConnections.Key].Count == 0) {
									List<List<Pair<int, ElevatorTile>>> list = new List<List<Pair<int, ElevatorTile>>> ();

									foreach (var el1 in connectedFloor.Value.ToList()) {
										foreach (var el2 in connectedFloorConnections.Value.ToList()) {
											var pos1 = el1.Last ().Second.GetWaitZone (el1.Last ().First);
											var pos2 = el2.First ().Second.GetWaitZone (el1.Last ().First);

											if (IsReachable (pos1, pos2)) {
												var l = el1.Concat (el2).ToList ();
												if (l [0].Second != l.Last ().Second) {
													list.Add (l);
													HasChanged = true;
												}
											}
										}
									}

									originFloorConnections.Value [connectedFloorConnections.Key] = list;
								} else {
									List<List<Pair<int, ElevatorTile>>> list = new List<List<Pair<int, ElevatorTile>>> ();

									foreach (var el1 in connectedFloor.Value.ToList()) {
										foreach (var el2 in connectedFloorConnections.Value.ToList()) {
											var pos1 = el1.Last ().Second.GetWaitZone (el1.Last ().First);
											var pos2 = el2.First ().Second.GetWaitZone (el1.Last ().First);

											if (IsReachable (pos1, pos2)) {
												var l = el1.Concat (el2).ToList ();
												if (l [0].Second != l.Last ().Second) {
													bool canAdd = true;

													foreach(var path in originFloorConnections.Value[l.Last().First]) {
														var firstO = l.First ().Second.GetWaitZone (originFloorConnections.Key);
														var firstD = path.First ().Second.GetWaitZone (originFloorConnections.Key);
														var lastO = l.Last ().Second.GetWaitZone (l.Last ().First);
														var lastD = path.Last ().Second.GetWaitZone (l.Last ().First);

														if (IsReachable (firstO, firstD) && IsReachable (lastO, lastD)) {
															canAdd = false;
														}
													}

													if (canAdd) {
														foreach (var pair in l) {
															if(l.Exists(e => {
																return (pair != e && e.Second == pair.Second);
															})) {
																canAdd = false;
															}
														}

														if (canAdd) {
															list.Add (l);
															HasChanged = true;
														}
													}
												}
											}
										}
									}

									originFloorConnections.Value [connectedFloorConnections.Key] = originFloorConnections.Value [connectedFloorConnections.Key].Concat (list).ToList ();
								}
							}
						}
					}
				}
			}
		}

		//ShowConnections ();
		//Debug.Log ("CreateElevatorsConnections execution time : " + watch.ElapsedMilliseconds + " ms");
		//watch.Stop ();
	}*/

    /*private void ShowConnections() {
		foreach (var a in elevatorsConnections) {
			int fr = a.Key;
			foreach (var b in a.Value) {
				int to = b.Key;
				foreach (var c in b.Value) {
					string str = "From " + fr + " to " + to + " : ";
					foreach (var d in c) {
						str += "(" + d.First + "," + d.Second.transform.name + ") - ";
					}
					Debug.Log (str);
				}
			}
		}
	}*/

    /*public List<Pair<int, ElevatorTile>> GetElevatorsToFloor(Vector3 Origin, Vector3 Destination) {
		//System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
		//watch.Start ();

		var list = new List<Pair<int, ElevatorTile>> ();

		if (elevatorsConnections != null && elevatorsConnections.ContainsKey (Mathf.RoundToInt (Origin.y)) && elevatorsConnections [Mathf.RoundToInt (Origin.y)].ContainsKey (Mathf.RoundToInt (Destination.y))) {
			var potentialList = elevatorsConnections [Mathf.RoundToInt (Origin.y)] [Mathf.RoundToInt (Destination.y)];

			foreach (var path in potentialList) {
				Vector3 o, d;
				d = path [0].Second.transform.position;
				d.y = Origin.y;

				o = path.Last ().Second.transform.position;
				o.y = Destination.y;

				if (IsReachable (Origin, d) && IsReachable (o, Destination)) {
					if (list.Count == 0 || Vector3.Distance (Origin, d) < Vector3.Distance (Origin, path.First ().Second.GetWaitZone (Mathf.RoundToInt (Origin.y)))) {
						list = path;
					}
				}
			}
		}

		//Debug.Log ("GetElevatorsToFloor execution time : " + watch.ElapsedMilliseconds + " ms");
		//watch.Stop ();

		return list;
	}*/

    public List<Pair<int, ElevatorTile>> GetElevatorsToFloor(Vector3 Origin, Vector3 Destination)
    {
        var returnList = new List<Pair<int, ElevatorTile>>();
        var elevatorsIn = elevatorsConectedTo(Origin);
        var elevatorsOut = elevatorsConectedTo(Destination);
        if (elevatorsIn.Count() == 0 || elevatorsOut.Count() == 0)
            return returnList;

        foreach (var eIn in elevatorsIn)
        {
            foreach (var eOut in elevatorsOut)
            {
                if (eIn == eOut)
                {
                    if (Mathf.RoundToInt(Origin.y) != Mathf.RoundToInt(Destination.y))
                        returnList.Add(new Pair<int, ElevatorTile>(Mathf.RoundToInt(Destination.y), eIn));
                    return returnList;
                }
            }
        }

        var visited = new List<VisitedElevators>();
        var toVisit = new List<int>();

        foreach(var e in elevatorsIn)
        {
            toVisit.Add(visited.Count());
            visited.Add(new VisitedElevators(-1, e, Mathf.RoundToInt(Origin.y)));
        }
        bool found = false;



        while (!found && toVisit.Count() > 0)
        {
            var id = toVisit.First();
            toVisit.RemoveAt(0);

            var e = visited[id];
            foreach(var c in connexionsInfosOf(e.child).exits)
            {
                if (c.Key == e.floor)
                    continue;
                foreach(var item in c.Value)
                {
                    if (isVisited(item, visited))
                        continue;
                    toVisit.Add(visited.Count());
                    visited.Add(new VisitedElevators(id, item, c.Key));
                    if (isOn(item, elevatorsOut))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
        }

        if (!found)
            return returnList;

        int currentIndex = visited.Count()-1;
        int currentFloor = Mathf.RoundToInt(Destination.y);

        while(currentIndex > 0)
        {
            returnList.Insert(0, new Pair<int, ElevatorTile>(currentFloor, visited[currentIndex].child));
            currentFloor = visited[currentIndex].floor;
            currentIndex = visited[currentIndex].parent;
        }

        return returnList;
    }

    bool isOn(ElevatorTile tile, List<ElevatorTile> list)
    {
        foreach (var t in list)
            if (tile == t)
                return true;
        return false;
    }

    bool isVisited(ElevatorTile tile, List<VisitedElevators> list)
    {
        foreach (var t in list)
            if (t.parent > 0 && tile == list[t.parent].child)
                return true;
        return false;
    }

    List<ElevatorTile> elevatorsAtFloor(int floor)
    {
        List<ElevatorTile> elevators = new List<ElevatorTile>();

        foreach (var e in elevatorConnections2)
            if (e.tile.FloorExists(floor))
                elevators.Add(e.tile);
        return elevators;
    }

    List<ElevatorTile> elevatorsConectedTo(Vector3 pos)
    {
        int floor = Mathf.RoundToInt(pos.y);
        var potentialElevators = elevatorsAtFloor(floor);
        List<ElevatorTile> elevators = new List<ElevatorTile>();

        NavMeshPath path = new NavMeshPath();
        foreach(var e in potentialElevators)
        {
            NavMesh.CalculatePath(pos, e.GetWaitZone(floor), NavMesh.AllAreas, path);
			if (path.status != NavMeshPathStatus.PathComplete)
				continue;
			bool pathValid = true;
			foreach (var c in path.corners)
			{
				if (Mathf.RoundToInt(c.y) != floor)
				{
					pathValid = false;
					break;
				}
			}
			if (!pathValid)
				continue;
            elevators.Add(e);
        }
        return elevators;
    }

    ElevatorConnexionsInfos connexionsInfosOf(ElevatorTile tile)
    {
        foreach (var c in elevatorConnections2)
            if (c.tile == tile)
                return c;
        return null;
    }

	private bool IsReachable(Vector3 origin, Vector3 destination) {
		NavMeshPath path = new NavMeshPath();
		if (NavMesh.CalculatePath (origin, destination, NavMesh.AllAreas, path)) {
			foreach (var c in path.corners) {
				if (Mathf.RoundToInt (c.y) != Mathf.RoundToInt (origin.y)) {
					return false;
				}
			}
			bool b = path.status == NavMeshPathStatus.PathComplete;
			return b;
		} else {
			return false;
		}
	}

	private List<Vector3> GetElevatorsPosAtFloor(int f) {
		var positions = getSpecialTiles (TileID.ELEVATOR);

		foreach (var pos in positions.ToList()) {
			if (Mathf.RoundToInt (pos.y) != f)
				positions.Remove (pos);
		}

		return positions;
	}

    private int maxUsedSpace;
    private int currentUsedSpace;
    public void addSpaceUsed(int value)
    {
        currentUsedSpace += value;
        if (currentUsedSpace > maxUsedSpace)
            maxUsedSpace = currentUsedSpace;
    }
    public int getMaxUsedSpace()
    {
        return maxUsedSpace;
    }

	public bool HasTileOfTypeAt(TileID id, Vector3 pos) {
		var tiles = at (pos);

		foreach (var t in tiles) {
			if (t.type == id)
				return true;
		}

		return false;
	}

	public bool IsValidTileForPodotactile(Vector3 pos) {
		var v = at (pos);

		bool valid = true;

		if (v.Count == 0 || ((v [0].type != TileID.GROUND && v[0].type != TileID.CONTROLELINE && !haveSpecialTileAt(TileID.WAIT_ZONE, pos))|| HasTileOfTypeAt(TileID.PODOTACTILE, pos))) {
			valid = false;
		}

		return valid;
	}
}
 