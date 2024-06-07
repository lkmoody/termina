using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameWorld : MonoBehaviour
{
    public static GameWorld Instance;

    [SerializeField]
    private int worldCellDepth = 0;
    [SerializeField]
    private float worldCellSize = 2f;
    [SerializeField]
    private Tilemap ground;
    [SerializeField]
    private Tilemap foreground;
    [SerializeField]
    private bool debugMode = false;

    void Awake() {
        Instance = this;
        transform.position = Vector3.zero;
    }

    void Start()
    {
        ground.CompressBounds();
        foreground.CompressBounds();

        Pathfinding pathfinding = new Pathfinding(ground.size.x, ground.size.y, worldCellSize, new Vector3(-(ground.size.x * .5f * worldCellSize), -(ground.size.y * .5f * worldCellSize), worldCellDepth), debugMode);

        List<Vector3> foregroundTileLocations = GetTileLocations(foreground);
        foreach(Vector3 tileLocation in foregroundTileLocations) {
            PathNode pathNodeAtLocation = pathfinding.GetGrid().GetGridObject(new Vector2(tileLocation.x, tileLocation.y));
            pathNodeAtLocation?.SetIsWalkable(false);
        }
    }

    public bool GetDebugMode() {
        return debugMode;
    }

    public List<Vector3> GetTileLocations(Tilemap tileMap) {
        List<Vector3> tiles = new();
 
        for (int n = tileMap.cellBounds.xMin; n < tileMap.cellBounds.xMax; n++)
        {
            for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = new(n, p, (int)tileMap.transform.position.y);
                Vector3 place = tileMap.CellToWorld(localPlace);
                if (tileMap.HasTile(localPlace))
                {
                    //Tile at "place"
                    tiles.Add(place);
                }
            }
        }

        return tiles;
    }
}
