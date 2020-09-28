using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System;

public class Grid
{
    public int Width, Height, CellSize;
    public Cell[,] grid;
    public Vector2 startPos, endPos;
    public InvDrone drone_script;

    public Grid(int width, int height, int cellSize)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        grid = new Cell[width, height];
        startPos = endPos = Vector2.negativeInfinity;
        drone_script = GameObject.Find("Drone").GetComponent<InvDrone>();
        
        GenerateGrid();
    }
    
    public void GenerateGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                grid[x, y] = new Cell(true, x, y, GetWorldPosition(x, y) + new Vector3(CellSize, CellSize) * .5f);
                Utils.CreateWorldText(null, ("(" + grid[x, y].x + ", " + grid[x,y].y + ")").ToString(), GetWorldPosition(x, y) + new Vector3(CellSize, CellSize) * .5f, 20, Color.white,
                  TextAnchor.MiddleCenter, TextAlignment.Center, 1);
                
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
            }
        }
        
        Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.white, 100f);
    }

    public void SetStartBlock(float mouseX, float mouseY)
    {
        ConvertToObjectSpace(mouseX, mouseY, out int x, out int y);
        
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            Debug.Log("StartBlock has been selected " + x + " " + y);
            startPos = new Vector2(x, y);
        }
    }

    public void SetEndBlock(float mouseX, float mouseY)
    {
        ConvertToObjectSpace(mouseX, mouseY, out int x, out int y);
        
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            Debug.Log("EndBlock has been selected " + x + " " + y);
            if (startPos != Vector2.negativeInfinity)
            {
                endPos = new Vector2(x, y);
                drone_script.Set(startPos, endPos, grid, Width, Height, CellSize);
            }
                
        }
    }
    private void ConvertToObjectSpace(float x, float y, out int newX, out int newY)
    {
        newX = Mathf.FloorToInt(x / CellSize);
        newY = Mathf.FloorToInt(y / CellSize);
    }
    
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x - Width / 2, y - Height / 2) * CellSize;
    }
}
