using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float delayActivatePlayer = 2f;
    [SerializeField] private Transform playerInitialPosition;

    PlayerInput input;
    PlayerMovement playerMovement;
    Rigidbody2D rb2D;
    SpriteRenderer spriteRenderer;
    Animator anim;
    Collider2D[] collider2DArray;

    private void Awake()
    {        
        input = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        collider2DArray = GetComponentsInChildren<Collider2D>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameIntro += GameIntroHandler;
        GameManager.Instance.OnGameStart += GameStartHandler;
        GameManager.Instance.OnGameExit += GameExitHandler;
    }    

    private void OnDisable()
    {
        GameManager.Instance.OnGameExit -= GameExitHandler;
        GameManager.Instance.OnGameStart -= GameStartHandler;
        GameManager.Instance.OnGameIntro -= GameIntroHandler;
    }

    private void GameIntroHandler()
    {
        Hide();
        transform.position = playerInitialPosition.position; // Se mueve a su posición inicial para que la cámara se acomode a esta nueva posición mientras está la pantalla oscura.
    }

    private void GameStartHandler()
    {
        StartCoroutine(ActivatePlayerWithDelay());
    }

    private void GameExitHandler()
    {
        SetActiveInput(false);
    }

    void SetActiveInput(bool value)
    {
        input.ClearInput();
        input.enabled = value;
    }

    IEnumerator ActivatePlayerWithDelay()
    {
        //transform.position = playerInitialPosition.position;
        yield return new WaitForSeconds(delayActivatePlayer);
        Show();
    }

    void Hide()
    {
        SetActivePlayer(false);
    }

    void Show()
    {
        SetActivePlayer(true);
    }

    void SetActivePlayer(bool value)
    {
        SetActiveInput(value);
        playerMovement.enabled = value;
        rb2D.simulated = value;
        spriteRenderer.enabled = value;
        anim.enabled = value;
        for (int i = 0; i < collider2DArray.Length; i++)
        {
            collider2DArray[i].enabled = value;
        }
    }
}
