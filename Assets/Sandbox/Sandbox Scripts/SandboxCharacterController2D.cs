using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxCharacterController2D : MonoBehaviour, IMoveableByCatcher
{
    enum MovingByPlatformType { MoveByCatcher , Emparenting}
    [Header("Environment Check Properties")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundSize;
    [SerializeField] private int raycastCount = 3; // Cantidad de rayos a lanzar para validar on ground
    [SerializeField] private MovingByPlatformType movingByPlatformType = MovingByPlatformType.MoveByCatcher;

    Rigidbody2D rb2D;
    float jumpVelocityToMaxHeight;
    float jumpVelocityToMinHeight;
    Vector2 prevVelocity;
    float[] raycastRelativePositionsX;
    
    public Collider2D ColliderOnGround { get; private set; }
    public bool IsOnGround { get; private set; }
    public bool BeginFalling { get; private set; }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        raycastRelativePositionsX = new float[raycastCount];
    }

    private void Start()
    {
        prevVelocity = rb2D.velocity;

        InitRaycastRelativePositionsX();
    }

    void InitRaycastRelativePositionsX()
    {
        if (raycastCount == 1)
        {
            raycastRelativePositionsX[0] = 0;
        }
        else if (raycastCount > 1)
        {
            float deltaX = groundSize.x / (raycastCount - 1);
            raycastRelativePositionsX[0] = -groundSize.x / 2f;
            for (int i = 1; i < raycastCount; i++)
            {
                raycastRelativePositionsX[i] = raycastRelativePositionsX[i - 1] + deltaX;
            }

        }
    }

    public void SetPhysicsVariables(float jumpMaxHeight, float jumpTimeToMaxHeight, float jumpMinHeight)
    {
        jumpVelocityToMaxHeight = 2f * jumpMaxHeight / jumpTimeToMaxHeight;
        float gravity = -2 * jumpMaxHeight / (jumpTimeToMaxHeight * jumpTimeToMaxHeight);
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, gravity); // Otra opción es dejar la gravedad por defecto y tocar la escala
        jumpVelocityToMinHeight = Mathf.Sqrt(-2f * gravity * jumpMinHeight);
    }

    public void MoveWithForce(float xVelocity, float acceleration)
    {
        // Si queremos aplicar una fuerza, se podría hacer de esta forma
        // Pero ojo, que si se usa una aceleración muy grande (100), el player se comporta mal, queda tiritando
        // En este caso para que quede tight se debe usar una aceleración de 50, con valores menores queda resbaladizo
        rb2D.AddForce(new Vector2((xVelocity - rb2D.velocity.x) * acceleration, 0), ForceMode2D.Force);
    }   

    public void MoveWithImpulse(float xVelocity, float acceleration)
    {        
        // No queremos moverlo modificando directamente la vel con: 
        //rb2D.velocity = new Vector2(xVelocity, rb2D.velocity.y);
        rb2D.AddForce(new Vector2((xVelocity - rb2D.velocity.x) * acceleration, 0), ForceMode2D.Impulse); // Usar valores de acceleration rango 0-1, con 1 NO resbala
        // Si aplicamos la fuerza con ForceMode2D.Impulse, entonces SÍ funcionará que otros scripts apliquen fuerza con modo Force
        // Pero NO funcionará si tratan de setear con modo Impulse, porque aquí yo estoy calculando el valor a aplicar
        // c/r a la vel en x       
    }

    public void JumpWithImpulse()
    {
        rb2D.gravityScale = 1f;
        // Se usa AddForce en vez de: //rb2D.velocity = new Vector2(rb2D.velocity.x, jumpVelocityToMaxHeight);
        // Al implementar el coyote time, se ve que la fuerza de impulso no esta fuerte como para que se note
        //  por eso se prefiere setear su vel.y a 0 antes de lanzarlo hacia arriba
        rb2D.velocity = new Vector2(rb2D.velocity.x, 0f);
        rb2D.AddForce(Vector2.up * jumpVelocityToMaxHeight, ForceMode2D.Impulse);
    }

    public void MoveByCatcher(Vector2 move)
    {
        if (movingByPlatformType != MovingByPlatformType.MoveByCatcher)
            return;
        //rb2D.MovePosition(rb2D.position + move); // El player se mueve con la plataforma, pero él no avanza correctamente sobre la plataforma
        rb2D.position += move; // Con esto sí funciona el mov del player por la plataforma Y sobre la plataforma
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("MovingPlatform"))
            return;

        if (movingByPlatformType != MovingByPlatformType.Emparenting)
            return;

        transform.parent = collision.transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("MovingPlatform"))
            return;

        if (movingByPlatformType != MovingByPlatformType.Emparenting)
            return;

        transform.parent = null;
    }

    // Notar que estas validaciones aplican a este controller, porque cuando el player está sobre la plataforma sin moverse
    //  su rb2D.velocity == 0, lo que físicamente no es la realidad, pero esto simplifica y hace más preciso el movimiento del player
    bool CharacterIsGoingUp => rb2D.velocity.y >= 0 && prevVelocity.y > 0;
    bool CharacterIsGoingDown => rb2D.velocity.y < 0 && prevVelocity.y <= 0;

    public void PhysicsCheck(float fallGravityMultiplier)
    {
        BeginFalling = prevVelocity.y >= 0 && rb2D.velocity.y < 0;

        //print(prevVelocity.y + " | " + rb2D.velocity.y + " | " + (ColliderOnGround != null));

        // Condición de borde: como el player puede saltar desde abajo de una plataforma, habrá colisión OnGround, pero en rigor
        //  el player va subiendo: en ese caso no se debería considerar como onground
        if (CharacterIsGoingUp || CharacterIsGoingDown)
        {
            ColliderOnGround = null; // Recordar que esta var public se usa en script PlatformCatcher
            IsOnGround = false;
        }        
        else
            IsOnGround = PhysicsIsOnGround();

        // Better Jumping in Unity: Optimizations        
        if (rb2D.velocity.y < 0)
            rb2D.gravityScale = fallGravityMultiplier;
        else
            rb2D.gravityScale = 1;        

        prevVelocity = rb2D.velocity;
    }

    bool PhysicsIsOnGround()
    {
        // El Raycast es más preciso que el OverlapBox: el overlap a veces detecta que está onground cuando la plataforma
        //  ya está más arriba que la box
        //ColliderOnGround = Physics2D.OverlapBox(groundCheckPosition.position, groundSize, 0, groundLayer);


        for (int i = 0; i < raycastRelativePositionsX.Length; i++)
        {
            Vector2 pos = GetRaycastPosition(raycastRelativePositionsX[i]);
            ColliderOnGround = Raycast(pos);
            if (ColliderOnGround != null)
                return true;
        }
                
        return false;
    }

    Collider2D Raycast(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, groundSize.y, groundLayer);
        return hit.collider;
    }

    public void ClampMaxVerticalSpeed()
    {
        if (rb2D.velocity.y > jumpVelocityToMinHeight)
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpVelocityToMinHeight);
    }

    public void ClampMinVerticalSpeed(float maxFallSpeed)
    {
        // La idea es que maxFallSpeed sea un valor negativo
        if (rb2D.velocity.y < maxFallSpeed)
            rb2D.velocity = new Vector2(rb2D.velocity.x, maxFallSpeed);
    }

    private void OnDrawGizmos()
    {
        Color c = IsOnGround ? Color.green : Color.red;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawCube(groundCheckPosition.position, groundSize);

        // Raycast
        Gizmos.color = Color.green;
        if (raycastRelativePositionsX != null)
        {            
            for (int i = 0; i < raycastRelativePositionsX.Length; i++)
            {
                Vector2 position = GetRaycastPosition(raycastRelativePositionsX[i]);                
                Gizmos.DrawRay(position, Vector2.down * groundSize.y);
            }
        }

    }

    Vector2 GetRaycastPosition(float relativeX)
    {
        Vector2 position = groundCheckPosition.position;
        position.y += groundSize.y / 2f;
        position.x += relativeX;
        return position;
    }

}
