using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    enum PlatformType { Loop } // Por ahora solo se implementa el loop entre los waypoints. Otras opciones: PingPong, NoLoop

    [SerializeField] private float speed = 1f;
    [SerializeField] private PlatformType platformType = PlatformType.Loop;
    [SerializeField] private float waitTimeInWaypoint = 1f;
    [SerializeField] private Vector2[] localWaypoints;    

    Rigidbody2D rb2D;
    Vector2[] worldWaypoints;
    float waitTime;
    int currentIndex;
    int targetIndex;
    PlatformCatcher platformCatcher;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        platformCatcher = GetComponent<PlatformCatcher>();
    }

    private void Start()
    {
        InitWorldWaypoints();
        InitPlatform();        
    }

    void InitWorldWaypoints()
    {
        worldWaypoints = new Vector2[localWaypoints.Length + 1]; // El primer punto siempre será la posición inicial del gameObject
        worldWaypoints[0] = transform.position;
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            worldWaypoints[i + 1] = transform.TransformPoint(localWaypoints[i]);
        }
    }

    void InitPlatform()
    {
        currentIndex = 0;
        targetIndex = GetNextIndex(currentIndex, platformType, worldWaypoints.Length);
        waitTime = 0;
    }

    private void OnValidate()
    {
        InitWorldWaypoints();
    }

    private void FixedUpdate()
    {
        if (localWaypoints.Length == 0) 
            return;

        if (speed == 0)
            return;

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }

        MovePlatform();
    }

    void MovePlatform()
    {
        Vector2 targetPosition = worldWaypoints[targetIndex];
        Vector2 directionToTarget = targetPosition - (Vector2)transform.position;

        float distance = GetDistanceToMoveTowardsTarget(directionToTarget, out bool lastStepToTarget);
        Vector2 movement = directionToTarget.normalized * distance;

        // Nos movemos este frame

        //rb2D.MovePosition(rb2D.position + movement);       
        // El problema de mover la plataforma con MovePosition, es que cuando suba, el player empezará a ser empujado hacia arriba
        //  y se puede ver cómo se desplaza un poco en el aire. Esto es así, porque estamos usando la gravedad del sistema de físicas.
        // Si solo queremos mover la plataforma, pero que no empuje al player, sino que solo lo desplace, tenemos que
        // mover la plataforma directamente con su position
        rb2D.position += movement;

        // Y ahora movemos al Player si está sobre la plataforma.
        // Nota: para que esto funcione correctamente, el script PlayerMovement se debe configurar para que se ejecute antes que 
        //  los otros scripts, es decir, primero el player se mueve, y luego movemos las plataformas.
        platformCatcher.MoveCaughtObjects(movement);        

        // Validamos si llegamos al target
        if (lastStepToTarget)
        {
            currentIndex = targetIndex;
            targetIndex = GetNextIndex(currentIndex, platformType, worldWaypoints.Length);
            waitTime = waitTimeInWaypoint;
        }

    }

    float GetDistanceToMoveTowardsTarget(Vector2 directionToTarget, out bool lastStepToTarget)
    {
        float distanceToGo = speed * Time.deltaTime;

        // Si la distancia que me pretendo mover este frame (distanceToGo) es MAYOR a la distancia a la que estamos del target
        //  entonces quiere decir que ya llegamos al target en este frame
        if (directionToTarget.sqrMagnitude < distanceToGo * distanceToGo)
        {
            lastStepToTarget = true;
            return directionToTarget.magnitude;
        }
        else
        {
            lastStepToTarget = false;
            return distanceToGo;
        }
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
        foreach (var point in worldWaypoints)
        {
            Gizmos.color = Color.green;
            float radius = 0.1f;
            Gizmos.DrawSphere(point, radius);
        }
    }
}
