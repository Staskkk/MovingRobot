using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplineTravel : ITravel
{
    private Vector2[] points;
    private float step;

    public SplineTravel(Vector2[] points, float step)
    {
        this.points = points;
        Array.Sort(this.points, (p1, p2) => p1.x.CompareTo(p2.x));
        this.step = step;
    }

    public IEnumerator<Vector2> GetEnumerator()
    {
        int n = points.Length;
        float[] A = new float[n];
        float[] B = new float[n];
        float[] C = new float[n];
        float[] D = new float[n];

        B[0] = 1;
        C[0] = -1;
        D[0] = 0;

        A[n - 1] = -1;
        B[n - 1] = 1;
        D[n - 1] = 0;

        float[] h = new float[n];
        float[] delta = new float[n];
        for (int i = 0; i < n - 1; i++)
        {
            h[i] = points[i + 1].x - points[i].x;
            delta[i] = (points[i + 1].y - points[i].y) / (points[i + 1].x - points[i].x);
        }

        for (int i = 1; i < n - 1; i++)
        {
            A[i] = h[i - 1];
            B[i] = 2 * (h[i - 1] + h[i]);
            C[i] = h[i];
            D[i] = delta[i] - delta[i - 1];
        }

        float[] sigma = GetProgonka(A, B, C, D);
        for (int i = 0; i < n - 1; i++)
        {
            for (float xx = points[i].x; xx < points[i + 1].x; xx += step)
            {
                float t = (xx - points[i].x) / h[i];
                float tCr = 1 - t;
                float yy = points[i + 1].y * t + points[i].y * tCr + h[i] * h[i] * (sigma[i + 1] * (Mathf.Pow(t, 3) - t) + sigma[i] * (Mathf.Pow(tCr, 3) - tCr));
                yield return new Vector2(xx, yy);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static float[] GetProgonka(float[] A, float[] B, float[] C, float[] D)
    {
        int n = D.Length;
        float[] x = new float[n];

        float[] R = new float[n];
        R[1] = -C[0] / B[0];
        float[] Om = new float[n];
        Om[1] = D[0] / B[0];

        for (int i = 1; i < n - 1; i++)
        {
            R[i + 1] = -C[i] / (A[i] * R[i] + B[i]);
            Om[i + 1] = (D[i] - A[i] * Om[i]) / (A[i] * R[i] + B[i]);
        }

        x[n - 1] = (D[n - 1] - A[n - 1] * Om[n - 1]) / (A[n - 1] * R[n - 1] + B[n - 1]);
        for (int i = n - 2; i >= 0; i--)
        {
            x[i] = x[i + 1] * R[i + 1] + Om[i + 1];
        }

        return x;
    }
}
