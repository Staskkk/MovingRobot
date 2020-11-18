using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsTravel : ITravel
{
    private Vector2[] points;

    private int index = 0;

    public PointsTravel(Vector2[] points)
    {
        this.points = points;
        Array.Sort(this.points, (p1, p2) => p1.x.CompareTo(p2.x));
    }

    public IEnumerator<Vector2> GetEnumerator()
    {
        foreach (var point in points)
        {
            yield return point;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
