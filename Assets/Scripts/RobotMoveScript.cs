using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMoveScript : MonoBehaviour
{
    public ITravel travel;

    public float moveSpeed = 1f;
    public float rotateSpeed = 1f;

    public float pointReachedDist = 0.01f;
    public float angleRotatedDegree = 1f;

    public bool isMoving;
    //public bool isRotating;
    public bool isFinished;

    public event EventHandler movingFinished;

    private IEnumerator<Vector2> pointIterator;
    private Vector3 pointDest;

    void Update()
    {
        if (isMoving)
        {
            //var lookRotation = Quaternion.LookRotation(pointDest - transform.position);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pointDest - transform.position), rotateSpeed * Time.deltaTime);
            //if (isRotating && Quaternion.Angle(transform.rotation, lookRotation) > angleRotatedDegree)
            //{
            //    return;
            //}
            //else
            //{
            //    isRotating = false;
            //}

            transform.Translate((pointDest - transform.position).normalized * moveSpeed * Time.deltaTime, Space.World);
            if (Vector3.Distance(transform.position, pointDest) <= pointReachedDist)
            {
                do
                {
                    if (!pointIterator.MoveNext())
                    {
                        StopMovement(true);
                        return;
                    }

                    pointDest = PointToPosition(pointIterator.Current);
                } while (Vector3.Distance(transform.position, pointDest) <= pointReachedDist);
                transform.LookAt(pointDest);
                //isRotating = true;
            }
        }
    }

    public void StartMovement()
    {
        if (isMoving)
        {
            StopMovement();
        }

        isFinished = false;
        pointIterator = travel.GetEnumerator();
        if (!pointIterator.MoveNext())
        {
            StopMovement(true);
            return;
        }

        transform.position = PointToPosition(pointIterator.Current);
        if (!pointIterator.MoveNext())
        {
            StopMovement(true);
            return;
        }

        pointDest = PointToPosition(pointIterator.Current);
        transform.LookAt(pointDest);

        isMoving = true;
    }

    private Vector3 PointToPosition(Vector2 point)
    {
        return new Vector3(point.x, transform.position.y, point.y);
    }

    public void ResumeMovement()
    {
        if (isMoving || isFinished)
        {
            return;
        }

        isMoving = true;
    }

    public void StopMovement(bool isFinished = false)
    {
        if (this.isFinished)
        {
            return;
        }

        isMoving = false;
        if (isFinished)
        {
            this.isFinished = true;
            movingFinished?.Invoke(this, new EventArgs());
        }
    }
}
