using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private float forceVelocity = 5f;

    Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Invoke(nameof(ApplyImpulse), 3f);
    }

    void ApplyImpulse()
    {
        float force = forceVelocity / Time.fixedDeltaTime;
        rb2D.AddForce(Vector2.right * forceVelocity, ForceMode2D.Impulse);
    }
}
