using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingDebug : MonoBehaviour
{
    private Grid<PathNode> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        transform.position = Vector3.zero;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(Grid<PathNode> grid)
    {
        this.grid = grid;
        UpdateDebugVisuals();

        grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
    }

    private void Grid_OnGridObjectChanged(object sender, Grid<PathNode>.OnGridObjectChangedEventArgs e)
    {
        UpdateDebugVisuals();
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateDebugVisuals();
        }
    }

    private void UpdateDebugVisuals()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector2 quadSize = new Vector2(1, 1) * grid.GetCellSize();

                PathNode pathNode = grid.GetGridObject(x, y);

                if (pathNode.isWalkable)
                {
                    quadSize = Vector2.zero;
                }

                Vector3 calculatedPos = grid.GetWorldPosition(x, y) + quadSize * .5f;
                if (!pathNode.isWalkable)
                {
                    Debug.Log(x);
                    Debug.Log(y);
                    Debug.Log(grid.GetWorldPosition(x, y));
                    Debug.Log(calculatedPos);
                }

                //MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y), 0f, quadSize, Vector2.zero, Vector2.zero);
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, calculatedPos, 0f, quadSize, Vector2.zero, Vector2.zero);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
