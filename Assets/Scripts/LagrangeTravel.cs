using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LagrangeTravel : ITravel
{
    private Vector2[] points;

    private float startX;
    private float endX;
    private float step;

    public LagrangeTravel(Vector2[] points, float step)
    {
        this.points = points;
        Array.Sort(this.points, (p1, p2) => p1.x.CompareTo(p2.x));
        this.startX = this.points.Min(p => p.x);
        this.endX = this.points.Max(p => p.x);
        this.step = step;
    }

    public IEnumerator<Vector2> GetEnumerator()
    {
        int n = points.Length;
        for (float xx = startX; xx <= endX; xx += step)
        {
            float yy = 0;
            for (int i = 0; i < n; i++)
            {
                float mult = points[i].y;
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        mult *= (xx - points[j].x) / (points[i].x - points[j].x);
                    }
                }
                yy += mult;
            }

            yield return new Vector2(xx, yy);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
