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
    public float angleRotatedDegree = 5f;
    public float minAngleToRotateDegree = 2f;

    public bool isMoving;
    public bool isFinished;

    public event EventHandler movingFinished;

    private IEnumerator<Vector2> pointIterator;
    private Vector3 pointDest;

    void Update()
    {
        if (isMoving)
        {
            if (CorrectRotation())
            {
                return;
            }

            for (int i = 0; i < Mathf.RoundToInt(moveSpeed); ++i) {
                transform.Translate((pointDest - transform.position).normalized * Time.deltaTime, Space.World);
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
                }
            }
        }
    }

    private bool CorrectRotation()
    {
        var lookRotation = Quaternion.LookRotation(pointDest - transform.position);
        var lookDirection = lookRotation * Vector3.forward;
        var leftRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotateSpeed * Time.deltaTime, transform.rotation.eulerAngles.z);
        var leftDirection = leftRotation * Vector3.forward;
        var rightRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - rotateSpeed * Time.deltaTime, transform.rotation.eulerAngles.z);
        var rightDirection = rightRotation * Vector3.forward;
        var currentDirection = transform.rotation * Vector3.forward;
        var currentAngle = Vector3.Angle(lookDirection, currentDirection);
        if (currentAngle >= minAngleToRotateDegree)
        {
            transform.rotation = Vector3.Angle(lookDirection, leftDirection) < Vector3.Angle(lookDirection, rightDirection) ?
                leftRotation :
                rightRotation;
        }

        return currentAngle > angleRotatedDegree;
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
