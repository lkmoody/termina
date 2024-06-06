using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathfinderTest : MonoBehaviour
{
    [SerializeField]
    private PathfindingDebug pathfindingDebug;

    private Pathfinding pathfinding;

    private void Awake()
    {
        transform.position = Vector3.zero;
    }

    private void Start()
    {
        pathfinding = new Pathfinding(10, 10, 10f, new Vector3(-5 * 10f, -5 * 10f, 0), true);
        pathfindingDebug.SetGrid(pathfinding.GetGrid());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PathNode node = pathfinding.GetGrid().GetGridObject(mousePosition);
            node.SetIsWalkable(!node.isWalkable);
        }
    }
}
