using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum State { Idle, Run, Jump, Dead }

    [Header("Movement Properties")]
    [SerializeField] private float speed = 4f;
    [Range(0.001f, 1f)] // Si usamos fuerza tipo Impulse para mover el player, la aceleración debe estar en este rango de valores
    [SerializeField] private float acceleration = 1f;     // Se podrían usar valores diferentes si está on ground o en el aire, o dependiendo de la superficie
    [SerializeField] private float maxFallSpeed = -25f;   //Max speed player can fall

    [Header("Jump Properties")]
    [SerializeField] private float jumpMaxHeight = 2.5f; // La altura máxima que alcanzará al saltar
    [SerializeField] private float jumpMinHeight = 0.5f; // La altura mínima que alcanzará al saltar
    [SerializeField] private float jumpTimeToMaxHeight = 0.4f; //El tiempo que le tomará en llegar a la altura máxima
    [SerializeField] private float jumpStateMinTime = 0.05f; // Minimo tiempo que durara el estado de Jump: es por condición de borde si FixedUpdate se ejecuta 2 veces en un mismo frame
    [SerializeField] private float fallGravityMultiplier = 2f; // Para que el player caiga con una gravedad diferente
    [SerializeField] private float coyoteDuration = .05f; //How long the player can jump after falling
    [SerializeField] private float jumpBufferDuration = 5f / 60f; // Even if you press the button too early, as long as it is still down when you land, the character will jump.

    public float Acceleration => acceleration;    

    CharacterController2D charactercontroller2D;
    PlayerInput input;
    State currentState;
    int facingDirection = 1;
    float xVelocity;
    bool isOnGround;
    float jumpTime;
    float coyoteTime;
    float jumpBuffer;

    public bool IsOnGround
    {
        get { return isOnGround; }
        set
        {
            if (isOnGround != value)
            {
                isOnGround = value;                
                GameManager.Instance.PlayerIsGroundedChanged(IsOnGround);                
            }
        }
    }
    public State CurrentState
    {
        get { return currentState; }
        private set
        {
            //print("From: " + currentState +  " To: " + value + " | " + Time.frameCount);

            // switch (currentState): aqui pueden ir los métodos IdleExit, RunExit, JumpExit, DeadExit, etc

            currentState = value;

            switch (currentState)
            {
                case State.Idle:
                    IdleEnter();
                    break;
                case State.Run:
                    RunEnter();
                    break;
                case State.Jump:
                    JumpEnter();
                    break;
            }
        }
    }
    
    bool JumpValidation => coyoteTime > Time.time && jumpBuffer > Time.time && input.JumpHeldDown;

    private void Awake()
    {        
        charactercontroller2D = GetComponent<CharacterController2D>();
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        currentState = State.Idle;
        charactercontroller2D.SetPhysicsVariables(jumpMaxHeight, jumpTimeToMaxHeight, jumpMinHeight);
    }

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        isOnGround = false;
    }

    private void OnValidate()
    {
        if (charactercontroller2D)
            charactercontroller2D.SetPhysicsVariables(jumpMaxHeight, jumpTimeToMaxHeight, jumpMinHeight);
    }

    private void FixedUpdate()
    {
        if (currentState == State.Dead) return;

        charactercontroller2D.PhysicsCheck(fallGravityMultiplier);

        if (charactercontroller2D.BeginFalling)
            GameManager.Instance.PlayerBeginFalling();

        IsOnGround = charactercontroller2D.IsOnGround;
        CoyoteTimeValidation();
        JumpBufferValidation();

        switch (CurrentState)
        {
            case State.Idle:
                if (JumpValidation)
                    CurrentState = State.Jump;
                else if (input.Horizontal != 0f)
                    CurrentState = State.Run;
                break;
            case State.Run:
                // Notar que la prioridad en la transición la lleva el Jump: si se hace al revés podemos tener esta condición de borde:
                //  Si CurrentState = Run && input.Horizontal = 0 && Se presionó el botón de salto:
                //    pasaríamos a estado Idle en vez de saltar: es decir, se pierde un salto
                if (JumpValidation)
                    CurrentState = State.Jump;
                else if (input.Horizontal == 0f)
                    CurrentState = State.Idle;
                else
                    RunUpdate();
                break;
            case State.Jump:
                // Condición de borde: si en el frame 100 paso a estado Jump, pero en el frame 101,
                // el motor de física todavía no me ha aplicado la fuerza suficiente para despegarme del suelo
                // entonces el estado jump va a creer que ya volví a estar en la tierra, cuando ni siquiera me he movido.
                //   y además, a veces en un MISMO frame se ejecuta 2 veces el FixedUpdate (por ej, a 30fps)
                // Por eso se usa el jumpTime
                // Esto se hace particularmente evidente cuando en la transición desde Jump solo teníamos Idle:
                //  me llamaba al IdleEnter, que me seteaba la vel.x a 0

                JumpUpdate();

                if (jumpTime > Time.time)
                    break;

                if (IsOnGround && input.Horizontal == 0f)
                    CurrentState = State.Idle;
                else if (IsOnGround && input.Horizontal != 0f)
                    CurrentState = State.Run;
                break;
        }

        charactercontroller2D.MoveWithImpulse(xVelocity, acceleration);
        charactercontroller2D.ClampMinVerticalSpeed(maxFallSpeed);
    }

    private void JumpBufferValidation()
    {
        if (input.JumpPressed)
            jumpBuffer = Time.time + jumpBufferDuration;
    }

    private void CoyoteTimeValidation()
    {
        if (IsOnGround) //If the player is on the ground, extend the coyote time window
            coyoteTime = Time.time + coyoteDuration;
    }

    void IdleEnter()
    {
        xVelocity = 0;
        GameManager.Instance.PlayerStoptMoving();
    }

    private void RunEnter()
    {
        GameManager.Instance.PlayerStartMoving();
        RunUpdate();
    }

    void RunUpdate()
    {
        xVelocity = speed * input.Horizontal;
        if (xVelocity * facingDirection < 0f)
            FlipCharacterDirection();
    }

    private void JumpEnter()
    {
        //if (!IsOnGround)
        //    print("JumpEnter por coyote time");
        //if (!input.JumpPressed)
        //    print("JumpEnter por jump buffer");
        //if (!input.JumpHeldDown)
        //    print("JumpEnter SIN JumpHeldDown");

        jumpTime = Time.time + jumpStateMinTime;
        charactercontroller2D.JumpWithImpulse();
        GameManager.Instance.PlayerJumping();
        coyoteTime = 0f; // En mi implementación actual esto no es estrictamente necesario, porque desde el estado jump no puedo volver a saltar
        jumpBuffer = 0f;
        input.ClearJumpPressed();
    }
    private void JumpUpdate()
    {
        xVelocity = speed * input.Horizontal;
        
        if (input.JumpReleasedUp) // Salto de altura variable
        {
            charactercontroller2D.ClampMaxVerticalSpeed();
        }
    }

    void FlipCharacterDirection()
    {
        facingDirection *= -1;
        /*
         * Gran Detallito con Flip al usar la scale.x a -1
         * Si colocamos el objeto hijo Firepoint dentro del Player y usamos el transform.right de ese objeto 
         * como referencia para generar el disparo, tenemos el detallito que al hacer un scale.x -1 al objeto padre, 
         * si bien el objeto Firepoint se flipea al otro lado, 
         * NO queda apuntando para el otro lado, sigue apuntando hacia el mismo lado! 
         * (No cambia el sentido de su transform.right)
         * Por esto es más seguro rotar.
         */
        transform.Rotate(0f, 180f, 0);
    }

}
