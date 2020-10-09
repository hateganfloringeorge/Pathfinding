using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvDrone : MonoBehaviour
{
    private List<RaycastHit2D> rays;
    
    [Range(0.0f, 25.0f)]
    public float radius = 25.0f;

    private int Width, Height, CellSize;
    private Cell[,] grid;
    private Vector2 startPos, endPos;
    
    private List<Cell> path;
    private List<Cell> rawAstarPath;
    private Vector3[] intermediatePath;
    private Vector3[] rawAStarintermediatePath;
    
    private LineRenderer lineRenderer;
    private static Vector2 CurrentPositionHolder, startLerpingPosition;
    private int CurrentNode;

    private float MoveSpeed;
    private float Timer;

    public bool _firstPosition = true;
    private bool _isSet = false;
    private int recalculateCount;


    private void Start()
    {
        rays = new List<RaycastHit2D>();
        MoveSpeed = 50.0f;
    }


    public void Set(Vector2 startPos, Vector2 endPos, Cell[,] grid, int Width, int Height, int CellSize)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.grid = grid;
        this.Width = Width;
        this.Height = Height;
        this.CellSize = CellSize;
        
        path = Astar();
        if (path != null)
            CheckNode();
        lineRenderer = GetComponent<LineRenderer>();
        _isSet = true;
        recalculateCount = 0;
    }

    
    private void Update()
    {
        // place on the map for the first time
        if (_firstPosition && _isSet)
        {
            transform.position = GetWorldPosition((int) startPos.x, (int) startPos.y) +
                                 new Vector3(CellSize, CellSize) * .5f;
            _firstPosition = false;
        }

        rays.Clear();
        Vector2 position = transform.position;
        
        if (_isSet)
        {
            if (intermediatePath[path.Count - 1].x != position.x || intermediatePath[path.Count - 1].y != position.y)
            {
                //path is same as intermediatePath but different in size and values from rawAstartIntermediatePath
                
                // Bezier path render
                // lineRenderer.positionCount = path.Count;
                // lineRenderer.SetPositions(intermediatePath);
                lineRenderer.positionCount = rawAStarintermediatePath.Length;
                lineRenderer.SetPositions(rawAStarintermediatePath);
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        for (int i = 0; i < 36; i += 1)
        {
            float rad = i * 10.0f * Mathf.Deg2Rad;
            Vector2 dir = GetDirectionVector(rad);
            
            // Radius for circle (uncomment add lines to use)
            // Vector2 add = new Vector2(2.5f * Mathf.Cos(rad), 2.5f * Mathf.Sin(rad));
            
            rays.Add(
                Physics2D.Raycast(
                    // position + add,
                    position,
                    dir,
                    radius
                )
            );
            
            if (rays[i].collider != null)
            {
                // Debug.DrawRay(position + add, dir * radius, Color.red);
                Debug.DrawRay(position, dir * radius, Color.red);
                
                
                /* Recalculate Astar */
                
                // 1. Gasim celula care e ocupata
                
                // Corner cases
                // When the Ray hits a border / corner
                float hitX = rays[i].point.x;
                float hitY = rays[i].point.y;
                bool hasHitBorderX = false;
                bool hasHitBorderY = false;
                
                /* Move hit point in the center of the cell
                if (FloatEqual(hitX, Math.Round(hitX)))
                {
                    if (position.x < hitX)
                        hitX += CellSize * .5f;
                    else
                        hitX -= CellSize * .5f;
                    hasHitBorderX = true;
                }
                else
                {
                    hasHitBorderX = false;
                }

                if (FloatEqual(hitY, Math.Round(hitY)))
                {
                    if (position.y < hitY)
                        hitY += CellSize * .5f;
                    else
                        hitY -= CellSize * .5f;
                    hasHitBorderY = true;
                }
                else
                {
                    hasHitBorderY = false;
                }

                // hitting corner gives no actual information
                if (hasHitBorderX && hasHitBorderY)
                {
                    //Debug.Log("HIT THE CORNER... [" + rays[i].point.x + ", " + rays[i].point.y + "] ");
                    continue;
                }
                */
                
                Vector2 possibleCell = ConvertToObjectSpace(new Vector2(hitX, hitY));
                Vector2 pozReala = ConvertToObjectSpace(position);            // only used for debug
                
                // 2. Verificam daca celula a fost gasita inainte daca nu, o setam
                int pX = (int) possibleCell.x;
                int pY = (int) possibleCell.y;
                

                if (pX >= Width || pY >= Height || pX < 0 || pY < 0)
                {
                    Debug.Log("Over border with [" + pX + ", " + pY + "] with original " + hitX + ", " +
                              hitY);
                    continue;
                }
                else if (grid[pX, pY].walkable == false)
                    continue;

               Debug.Log("Blocke cell: " + pX + " , " + pY + "  seen at position: [" + pozReala.x + ", " + pozReala.y + "] with original " + hitX + ", " +
                         hitY);

               grid[pX, pY].walkable = false;

                // 3. Recalculam A* din pozitia curenta daca celula de coliziune este la noi in path (A* sau Bezier)
                
                for (int j = 0; j < path.Count; j++)
                {
                    if ((j < rawAstarPath.Count && (rawAstarPath[j].x == pX && rawAstarPath[j].y == pY)) ||
                        (path[j].x == pX && path[j].y == pY))
                    {
                        Vector2 pozitieReala = ConvertToObjectSpace(position);
                        int nowX = (int) pozitieReala.x;
                        int nowY = (int) pozitieReala.y;
                        
                        /* Recalculate A* */
                        startPos = new Vector2(nowX, nowY);
                        CurrentNode = 0;   
                        path = Astar();
                        if (path != null)
                            CheckNode();
                        recalculateCount++;
                        break;
                    }
                }
                
                // 4. Continuam animatia dupa noul path si ca sa facem asta, resetam tot ce inseamna drum
            }
            else
            {
                //Debug.DrawRay(position + add, dir * radius, Color.green);
                Debug.DrawRay(position, dir * radius, Color.green);
            }
        }
        
        if (_isSet)
        {
            /* Move */
            Timer += Time.deltaTime * MoveSpeed;

            if (position != CurrentPositionHolder)
            {
                transform.position = Vector2.Lerp(startLerpingPosition, CurrentPositionHolder, Timer);
            }
            else
            {
                if (CurrentNode < path.Count - 1)
                {
                    CurrentNode++;
                    CheckNode();
                }
                else if (CurrentNode == path.Count - 1)
                {
                    Debug.Log("Path was recomputed " + recalculateCount + " times!");
                    CurrentNode++;
                }
            }
        }
        
    }
    
    
    private List<Cell> Astar()
    {
        Astar solver = new Astar(startPos, endPos, grid, Width, Height);
        List<Cell> path = solver.Process();

        if (path == null)
        {
            Debug.Log("Was not able to find any viable path! Exit!");
            _isSet = false;
            lineRenderer.enabled = false;
        }
        else
        {
            string s = "";
            foreach (var cell in path)
            {
                s += "(" + cell.x + ", " + cell.y + ") ";
            }

            Debug.Log("path: " + s);

            // Raw A* made for object space
            rawAstarPath = path;
            rawAStarintermediatePath = ConvertCellsToVector3(path).ToArray();

            // Checking if there are corners near an obstacle and moving the corner point by Cellsize / 2 away on x and y
            path = CheckingCorners(rawAstarPath);

            // Restart from same position
            if (_isSet)
                path[0].worldPos = transform.position;

            // Aplying Bezier
            float tLength = 0;
            for (int i = 0; i < path.Count - 1; i++)
                tLength += GetEuclidianDistance(path[i].worldPos, path[i + 1].worldPos);

            float step = 1 / tLength;

            List<Vector3> newPath = new List<Vector3>();

            for (float t = 0.0f; t <= 1.0f; t += step)
            {
                newPath.Add(Bezier.Apply(path, t));
            }

            // LineReader needs an array
            intermediatePath = newPath.ToArray();

            // Make Cells to have both object and world coordinates
            path = ConvertVector3ToCell(intermediatePath);
        }
        
        return path;
    }
    
    
    private void CheckNode()
    {
        Timer = 0;
        CurrentPositionHolder = path[CurrentNode].worldPos;
        startLerpingPosition = transform.position;
    }
    
    
    private Vector2 ConvertToObjectSpace(Vector2 point)
    {
        Vector2 possibleCell = new Vector2(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y));
        possibleCell += new Vector2(Width, Height) * (CellSize * .5f);
        possibleCell.x = Mathf.FloorToInt(possibleCell.x / CellSize);
        possibleCell.y = Mathf.FloorToInt(possibleCell.y / CellSize);

        return possibleCell;
    }

    private Vector3[] ConvertCellsToVector3(List<Cell> list)
    {
        Vector3[] myPath = new Vector3[list.Count];
        int i = 0;
        foreach (var cell in list)
        {
            myPath[i] = GetWorldPosition(cell.x, cell.y) + new Vector3(CellSize, CellSize) * .5f;
            i++;
        }
        return myPath;
    }

    private List<Cell> ConvertVector3ToCell(Vector3[] bezPath)
    {
        List<Cell> newPath = new List<Cell>();
        foreach (var point in bezPath)
        {
            Vector2 objPoz = ConvertToObjectSpace(new Vector2(point.x, point.y));
            newPath.Add(new Cell(true, (int)objPoz.x , (int)objPoz.y , point));   
        }
        return newPath;
    }

    
    private List<Cell> CheckingCorners(List<Cell> oldPath)
    {
        List<Cell> newPath = new List<Cell>();
        newPath.Add(oldPath[0]);
        
        for (int i = 0; i < oldPath.Count - 2; i++)
        {
            // Get grups of 3 consecutive Cells
            Cell first = oldPath[i];
            Cell second = oldPath[i + 1];
            Cell third = oldPath[i + 2];
            
            if ((first.x == second.x && first.x == third.x) || (first.y == second.y && first.y == third.y))
            {
                // They are all collinear
                newPath.Add(second);
            }
            else
            {
                // the 3 points make a corner and we need to check if the forth (inside one) is an obstacle
                Vector2 posibleObstacle = new Vector2();
                float offsetX = 0;
                float offsetY = 0;
                
                if (second.x == first.x)
                {
                    posibleObstacle.x = third.x;
                }
                else
                {
                    posibleObstacle.x = first.x;
                }

                if (second.y == first.y)
                {
                    posibleObstacle.y = third.y;
                }
                else
                {
                    posibleObstacle.y = first.y;
                }
                
                if (grid[(int)posibleObstacle.x, (int)posibleObstacle.y].walkable == false)
                {
                    Debug.Log("Found one corner at position : [" + posibleObstacle.x + ", " + posibleObstacle.y + "] ");

                    // Obtinem coltul cel mai indepartat de celula blocata, al celei de-a doua celule
                    if (posibleObstacle.x > second.x)
                        offsetX -= CellSize * .5f;
                    else
                        offsetX += CellSize * .5f;

                    if (posibleObstacle.y > second.y)
                        offsetY -= CellSize * .5f;
                    else
                        offsetY += CellSize * .5f;
                }
                
                // Adaugam coltul cel mai indepartat si mijloacele laturilor ce il formeaza
                Vector3 cornerPos = new Vector3(second.worldPos.x + offsetX, second.worldPos.y + offsetY);
                Vector3 beforeCorner = cornerPos;
                Vector3 afterCorner = cornerPos;
                
                if (first.y == second.y)
                {
                    beforeCorner.x -= offsetX;
                    afterCorner.y -= offsetY;
                }
                else
                {
                    beforeCorner.y -= offsetY;
                    afterCorner.x -= offsetX;
                }
                newPath.Add(new Cell(true, second.x, second.y, beforeCorner));
                newPath.Add(new Cell(true, second.x, second.y, cornerPos));
                newPath.Add(new Cell(true, second.x, second.y, afterCorner));
            }
        }
        newPath.Add(oldPath[oldPath.Count - 1]);
        return newPath;
    }
    
    
    private static bool FloatEqual(double f1, double f2)
    {
        return Math.Abs(f1 - f2) < .001f;
    }

    
    private float GetEuclidianDistance(Vector2 v1, Vector2 v2)
    {
        float x = v1.x - v2.x;
        float y = v1.y - v2.y;
        return Mathf.Sqrt(x * x + y * y);
    }

    
    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x - Width / 2, y - Height / 2) * CellSize;
    }

    
    private Vector2 GetDirectionVector(float radians)
    {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }


    public void ResetData()
    {
        _firstPosition = true;
        _isSet = false;
        CurrentNode = 0;
    }
}
