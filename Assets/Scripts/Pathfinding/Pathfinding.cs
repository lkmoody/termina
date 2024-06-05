using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding(int width, int height, float cellSize, Vector2 originPostion)
    {
        grid = new Grid<PathNode>(width, height, cellSize, originPostion, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y), true);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 && y == 0)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 0 && y == 1)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 1 && y == 1)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 1 && y == 2)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 2 && y == 2)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 2 && y == 3)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 3 && y == 3)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 3 && y == 4)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }

                if (x == 4 && y == 4)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }
                if (x == 5 && y == 5)
                {
                    grid.GetGridObject(x, y).SetIsWalkable(false);
                }
            }
        }
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    public List<Vector2> GetWalkingPath(Vector2 startPos, Vector2 endPos)
    {
        GetGrid().GetXY(startPos, out int startX, out int startY);
        GetGrid().GetXY(endPos, out int endX, out int endY);
        List<PathNode> pathNodes = FindPath(startX, startY, endX, endY);

        if (pathNodes != null)
        {
            List<Vector2> walkingPath = new List<Vector2>();
            pathNodes.ForEach(node =>
            {
                Vector2 newPoint = new(node.x * grid.GetCellSize(), node.y * grid.GetCellSize());
                newPoint += grid.GetOriginPosition();
                newPoint += grid.GetCellCenterOffset();
                Debug.Log(newPoint);
                walkingPath.Add(newPoint);
            });
            return walkingPath;
        }

        return null;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }
}
