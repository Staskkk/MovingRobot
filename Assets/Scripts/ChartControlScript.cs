using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartControlScript : MonoBehaviour
{
    public float ratio = 1f;

    public GameObject pointPrefab;
    public List<GameObject> pointsObjects;
    public GameObject smallPointPrefab;
    public List<GameObject> smallPointsObjects;

    public void GeneratePoints(Vector2[] points)
    {
        ClearPoints();
        pointsObjects = new List<GameObject>(points.Length);
        foreach (var point in points)
        {
            pointsObjects.Add(Instantiate(pointPrefab, new Vector3(point.x * ratio, 0, point.y * ratio), Quaternion.identity, transform));
        }
    }

    public void GenerateSmallPoints(ITravel travel)
    {
        ClearSmallPoints();
        smallPointsObjects = new List<GameObject>(10000);
        foreach (var point in travel)
        {
            smallPointsObjects.Add(Instantiate(smallPointPrefab, new Vector3(point.x * ratio, 0, point.y * ratio), Quaternion.identity, transform));
        }
    }

    public void ClearAllPoints()
    {
        ClearPoints();
        ClearSmallPoints();
    }

    private void ClearPoints()
    {
        if (pointsObjects != null)
        {
            foreach (var pointObject in pointsObjects)
            {
                Destroy(pointObject);
            }
        }

        pointsObjects = null;
    }

    private void ClearSmallPoints()
    {
        if (smallPointsObjects != null)
        {
            foreach (var smallPointObject in smallPointsObjects)
            {
                Destroy(smallPointObject);
            }
        }

        smallPointsObjects = null;
    }
}
