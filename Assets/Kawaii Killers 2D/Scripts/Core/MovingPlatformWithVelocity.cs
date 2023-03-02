using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformWithVelocity : MonoBehaviour
{
    // Este script ya no se usa para mover a la plataforma

    enum PlatformType { Loop } // Por ahora solo se implementa el loop entre los waypoints. Otras opciones: PingPong, NoLoop

    [SerializeField] private float speed = 1f;
    [SerializeField] private PlatformType platformType = PlatformType.Loop;
    [SerializeField] private float waitTimeInWaypoint = 1f;
    [SerializeField] private float delayValidationPoint = 0.1f; // No es necesario validar en cada frame si ya llegamos al punto target
    [Range(0f, 1f)]
    [SerializeField] private float dotErrorMargin = 0.8f; // // Notar que este valor es solo para vectores normalizados
    [SerializeField] private Vector2[] localWaypoints;


    Rigidbody2D rb2D;
    Vector2[] worldWaypoints;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        worldWaypoints = new Vector2[localWaypoints.Length + 1]; // El primer punto siempre ser? la posici?n inicial 
    }

    private void Start()
    {
        worldWaypoints[0] = transform.position;
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            worldWaypoints[i + 1] = transform.TransformPoint(localWaypoints[i]);
        }

        if (localWaypoints.Length > 1)
            StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        var waitValidation = new WaitForSeconds(delayValidationPoint);
        var waitTime = new WaitForSeconds(waitTimeInWaypoint);
        int currentIndex = 0;
        int targetIndex;

        while (true)
        {
            targetIndex = GetNextIndex(currentIndex, platformType, worldWaypoints.Length);
            Vector2 currentPosition = worldWaypoints[currentIndex];
            Vector2 targetPosition = worldWaypoints[targetIndex];
            Vector2 currentDirection = GetDirectionToTarget(targetPosition);

            // La dirección de la vel NO se calcula c/r a la position actual
            //  sino que por el vector que va desde el waypoint anterior al waypoint actual.            
            Vector2 velocityDirection = (targetPosition - currentPosition).normalized;
            rb2D.velocity = velocityDirection * speed;

            Vector2 newDirection = currentDirection;

            while (AlmostSameDirection(currentDirection, newDirection))
            {
                yield return waitValidation;
                newDirection = GetDirectionToTarget(targetPosition);
            }

            rb2D.velocity = Vector2.zero;
            yield return waitTime;
            currentIndex = targetIndex;
        }

    }

    bool AlmostSameDirection(Vector2 A, Vector2 B)
    {
        float dot = Vector2.Dot(A, B);
        return dot > dotErrorMargin;
    }

    Vector2 GetDirectionToTarget(Vector2 targetPosition)
    {
        return (targetPosition - (Vector2)transform.position).normalized;
    }

    int GetNextIndex(int currentIndex, PlatformType platformType, int length)
    {
        int nextIndex = 0;
        switch (platformType)
        {
            case PlatformType.Loop:
                nextIndex = (currentIndex + 1) % length;
                break;
        }
        return nextIndex;
    }

    private void OnDrawGizmos()
    {
        foreach (var localPoint in localWaypoints)
        {
            Vector2 worldPoint = transform.TransformPoint(localPoint);
            Gizmos.color = Color.green;
            float radius = 0.1f;
            Gizmos.DrawSphere(worldPoint, radius);
        }
    }
}