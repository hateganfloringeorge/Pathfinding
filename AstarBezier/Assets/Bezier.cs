using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    private static double Fact(int n)
    {
        if (n == 0 || n == 1)
            return 1;

        return n * Fact(n - 1);
    }
    
    private static float Bernstein(int i, int n, float t)
    {
        double a1 = Fact(n) / Fact(i);
        double a2 = 1 / Fact(n - i);
        
        float t1 = (float)(a1 * a2);
        float t2 = Mathf.Pow(t, i);
        float t3 = Mathf.Pow(1 - t, n - i);
        
        return t1 * t2 * t3;
    }
    
    public static Vector3 Apply(List<Cell> points, float t)
    {
        Vector3 res = Vector3.zero;
        int n = points.Count - 1;

        for (int i = 0; i <= n; i++)
        {
            res.x += points[i].worldPos.x * Bernstein(i, n, t);
            res.y += points[i].worldPos.y * Bernstein(i, n, t);
        }

        return res;
    }
}