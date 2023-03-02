using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField] private float delay = 2f;
    [SerializeField] private float timeImpulse = 1f;
    [SerializeField] private float forceVelocity = 2f;
    [SerializeField] private int directionImpulse = -1;
    [SerializeField] private PlayerMovement target;

    Rigidbody2D targetRb2D;

    private void Awake()
    {
        targetRb2D = target.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(ApplyForceToTarget(delay, timeImpulse));
    }


    private IEnumerator ApplyForceToTarget(float delay, float timeImpulse)
    {
        yield return new WaitForSeconds(delay);
        float time = timeImpulse;

        print("ApplyForceToTarget");

        while (time > 0)
        {
            yield return new WaitForFixedUpdate();
            // Nota de AddForce en modo Force: si se aplica una fuerza de 100, eso se traduce
            // en que el player se tratará de mover a una velocidad de 2, porque:
            //   100/50 == 2
            // donde el valore de 1/50 == el valor configurado en settings Time - Fixed Timestep
            float force = forceVelocity / Time.fixedDeltaTime;
            targetRb2D.AddForce(directionImpulse * force * target.Acceleration * Vector2.right, ForceMode2D.Force); 
            time -= Time.deltaTime;
        }

    }

    private IEnumerator ApplyImpulseToTarget(float delay, float timeImpulse)
    {
        // Fuerzas tipo Impulse no sirve con mi PlayerMovement
        yield return new WaitForSeconds(delay);
        float time = timeImpulse;

        print("Apply Impulse ToTarget");

        while (time > 0)
        {
            yield return new WaitForFixedUpdate();           
            float force = forceVelocity / Time.fixedDeltaTime;
            targetRb2D.AddForce(directionImpulse * force * Vector2.right, ForceMode2D.Impulse); 
            time -= Time.deltaTime;
        }

    }

}

