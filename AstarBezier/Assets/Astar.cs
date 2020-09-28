using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class Astar
{
    private Vector2 startPos;
    private Vector2 endPos;
    private Cell[,] grid;
    private int height, width;
    
    public Astar(Vector2 startPos, Vector2 endPos, Cell[,] grid, int width, int height)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.grid = grid;
        this.width = width;
        this.height = height;
    }

    private bool CheckBoundaries(int j, int i)
    {
        return i >= 0 && i < height && j >= 0 && j < width;
    }
    
    private List<Cell> GetNeighbours(Cell origin)
    {
        List<Cell> neighbours = new List<Cell>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;
                /*
                 // Only X and Y
                if ((Math.Abs(i) + Math.Abs(j)) != 1)
                    continue;
                */
                
                int newX = j + origin.x;
                int newY = i + origin.y;
                if (CheckBoundaries(newX, newY))
                {
                    neighbours.Add(grid[newX, newY]);   
                }   
            }
        }
        
        
        return neighbours;
    }
    
    int GetDistance(Cell nodeA, Cell nodeB) {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX-dstY);
        return 14 * dstX + 10 * (dstY-dstX);
    }
    
    
    public List<Cell> Process()
    {
        Cell startCell = grid[(int)startPos.x, (int)startPos.y];
        Cell endCell = grid[(int)endPos.x, (int)endPos.y];
        
        MinHeap openSet = new MinHeap(width * height);
        HashSet<Cell> closedSet = new HashSet<Cell>();
        
        openSet.Add(startCell);
        int count = 0;
        while (!openSet.IsEmpty())
        {
            count++;
            Cell current = openSet.Pop();
            closedSet.Add(current);

            if (current == endCell)
                break;
            
            foreach (var neighbour in GetNeighbours(current))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCostToNeighbour = current.gCost + GetDistance(current, neighbour);
                if (!openSet.Contains(neighbour) || newCostToNeighbour < neighbour.gCost)
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endCell);
                    neighbour.parent = current;
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        if (openSet.IsEmpty())
        {
            return null;   
        }
        return RetracePath(startCell, endCell);
    }

    public List<Cell> RetracePath(Cell startCell, Cell endCell)
    {
        Cell current = endCell;
        List<Cell> path = new List<Cell>();
        while (current != startCell)
        {
            path.Add(current);
            current = current.parent;
        }
        
        path.Add(startCell);
        
        path.Reverse();
        return path;
    }
}
