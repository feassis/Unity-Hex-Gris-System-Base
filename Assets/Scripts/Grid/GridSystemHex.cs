using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemHex<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private float floorHeight;
    private int floor;
    private TGridObject[,] gridObjectArray;

    private const float HEX_VERTICAL_OFFSET_MODIFIER = 0.75f;

    public int GetFloor() => floor;

    public GridSystemHex(int width, int height, float cellSize, int floor, float floorHeight, 
        Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floor = floor;
        this.floorHeight = floorHeight;
        gridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridObjectArray[x, z] = createGridObject(this, new GridPosition(x, z, floor));
            }
        }      
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, 0) *  cellSize  + 
            (gridPosition. z % 2 == 1 ? new Vector3(1, 0, 0)* cellSize * .5f : Vector3.zero)+
            new Vector3(0, 0, gridPosition.z) * cellSize * HEX_VERTICAL_OFFSET_MODIFIER
            + new Vector3(0, floor,0) * floorHeight;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        GridPosition roughXZ = new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize / HEX_VERTICAL_OFFSET_MODIFIER),
            floor);

        bool oddRow = roughXZ.z % 2 == 1;

        List<GridPosition> neighbours = new List<GridPosition> {
            roughXZ + new GridPosition(-1, 0, 0),
            roughXZ + new GridPosition(1, 0, 0),
            roughXZ + new GridPosition(0, -1, 0),
            roughXZ + new GridPosition(0, 1, 0),
            roughXZ + new GridPosition(oddRow ? +1 : -1, -1, 0),
            roughXZ + new GridPosition(oddRow ? +1 : -1, 1, 0)

        };

        GridPosition closestGridPosition = roughXZ;

        foreach (GridPosition neighbor in neighbours)
        {
            if (Vector3.Distance(worldPosition, GetWorldPosition(neighbor)) < Vector3.Distance(worldPosition, GetWorldPosition(closestGridPosition)))
            {
                closestGridPosition = neighbor;
            }  
        }

        return closestGridPosition;
    }

    public void CreateDebugObjects(GridDebugObject debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, floor);
                GridDebugObject debugObj = GameObject.Instantiate(debugPrefab, 
                    GetWorldPosition(gridPosition), Quaternion.identity);

                debugObj.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.z >= 0
            && gridPosition.x < width && gridPosition.z < height
            && gridPosition.floor == floor;
    }

    public (int width, int height) GetGridSize() => (width, height);
}
