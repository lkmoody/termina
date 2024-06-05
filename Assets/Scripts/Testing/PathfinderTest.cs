using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathfinderTest : MonoBehaviour
{
    private Pathfinding pathfinding;

    private void Start() {
        pathfinding = new Pathfinding(10, 10, 10f, new Vector3(-5 * 10f, -5 * 10f, 0));   
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PathNode node = pathfinding.GetGrid().GetGridObject(mousePosition);
            node.SetIsWalkable(!node.isWalkable);
        }
    }

    public List<Vector2> GetWalkingPath(Vector2 startPos, Vector2 endPos) {
        return pathfinding.GetWalkingPath(startPos, endPos);
    }

    // public void GetPath(Vector2 startPos, Vector2 endPos) {
    //     pathfinding.FindPath(startPos.x, startPos.y, endPos.x, endPos.y);
    // }
}
