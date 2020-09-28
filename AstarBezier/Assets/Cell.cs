using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Cell {
	
    public bool walkable;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public Cell parent;
    public Vector2 worldPos;
	
    public Cell(bool _walkable, int _gridX, int _gridY, Vector3 worldPosition) {
        walkable = _walkable;
        x = _gridX;
        y = _gridY;
        worldPos = worldPosition;
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public static bool operator == (Cell c1, Cell c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(Cell c1, Cell c2)
    {
        return !(c1 == c2);
    }
}