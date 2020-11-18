using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartControlScript : MonoBehaviour
{
    public float ratio = 1f;

    public GameObject pointPrefab;
    public List<GameObject> pointsObjects;

    public void GeneratePoints(Vector2[] points)
    {
        ClearPoints();

        pointsObjects = new List<GameObject>(points.Length);
        foreach (var point in points)
        {
            pointsObjects.Add(Instantiate(pointPrefab, new Vector3(point.x * ratio, 0, point.y * ratio), Quaternion.identity, transform));
        }
    }

    public void ClearPoints()
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
}
