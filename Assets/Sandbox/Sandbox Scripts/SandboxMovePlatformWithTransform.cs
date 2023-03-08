using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxMovePlatformWithTransform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 5;

    Vector3 targetPosition;

    private void Start()
    {
        targetPosition = pointA.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime*speed);

        if (transform.position == pointA.position)
        {
            targetPosition = pointB.position;
        }

        if (transform.position == pointB.position)
        {
            targetPosition = pointA.position;
        }
    }
}
