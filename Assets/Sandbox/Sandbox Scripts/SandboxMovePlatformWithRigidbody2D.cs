using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxMovePlatformWithRigidbody2D : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 5;

    Vector3 targetPosition;
    Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        targetPosition = pointA.position;
    }

    private void FixedUpdate()
    {
        Vector3 position = Vector3.MoveTowards(rb2D.position, targetPosition, Time.deltaTime * speed);
        rb2D.MovePosition(position);
        //rb2D.position = position;

        if (rb2D.position == (Vector2)pointA.position)
        {
            targetPosition = pointB.position;
        }

        if (rb2D.position == (Vector2)pointB.position)
        {
            targetPosition = pointA.position;
        }

        /*
         * Conclusión:
         *   Tenemos el objeto player como un rb dinámico, sin gravedad para simplificar la prueba.
         *   Si el player es hijo del objeto "Platform Transform" el cual se mueve en el Update modificando su transform.position,
         *     entonces el objeto player se moverá con él
         *   Pero si el player ahora es hijo del objeto "Platform Rigidbody2D" el cual se mueve en el FixedUpdate usando rb2D.MovePosition,
         *     entonces el objeto player, aunque sea hijo, NO se moverá.
         */
    }
}
