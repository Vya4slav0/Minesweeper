using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private int mineCount;
    [SerializeField] private int fieldSizeX;
    [SerializeField] private int fieldSizeY;
    [SerializeField] private GameObject cellPrefab;
    private MeshRenderer cellMeshRenderer;
    private Cell[,] cellsMap;

    // Start is called before the first frame update
    void Start()
    {
        cellsMap = new Cell[fieldSizeX, fieldSizeY];
        for (int i = 0; i < cellsMap.GetLength(0); i++)
        {
            for (int j = 0; j < cellsMap.GetLength(1); j++)
                cellsMap[i, j] = new Cell();
        }
        cellMeshRenderer = cellPrefab.GetComponent<MeshRenderer>();
        GenerateField(false);
    }

    public bool CheckMineAtCoordinate(int x, int y)
    {
        return GetCellAtCoordinate(x, y).HasMine;
    } 

    public Cell GetCellAtCoordinate(int x, int y)
    {
        if (x < 0 || y < 0 || x >= fieldSizeX || y >= fieldSizeY) return null;
        return cellsMap[x, y];
    }

    void GenerateField(bool deleteOld)
    {
        System.Random rand = new System.Random();
        int i = 0;
        while (i < mineCount)
        {
            int xIndex = rand.Next(fieldSizeX);
            int yIndex = rand.Next(fieldSizeY);
            if (cellsMap[xIndex, yIndex].HasMine) continue;
            cellsMap[xIndex, yIndex].HasMine = true;
            i++;
        }

        if (deleteOld)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.GameObject());
            }
        }

        for (int xIndex = 0; xIndex < fieldSizeX; xIndex++)
        {
            for (int yIndex = 0; yIndex < fieldSizeY; yIndex++)
            {
                GameObject newCell = Instantiate(
                    cellPrefab, 
                    new Vector3(xIndex * cellMeshRenderer.bounds.size.x, 0, yIndex * cellMeshRenderer.bounds.size.z), 
                    cellPrefab.transform.rotation, 
                    transform);
                newCell.GetComponent<Renderer>().material.mainTexture = Resources.Load("CellTextures/CellClosedTexture") as Texture2D;
                Cell newCellController = newCell.GetComponent<Cell>();
                newCellController.Field = this;
                newCellController.XIndex = xIndex;
                newCellController.YIndex = yIndex;
                newCellController.HasMine = cellsMap[xIndex, yIndex].HasMine;
                cellsMap[xIndex, yIndex] = newCellController;
            }
        }
    }
}
