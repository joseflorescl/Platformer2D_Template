using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorController : MonoBehaviour
{

    Collider2D coll2D;
    SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        GameManager.Instance.OnGameIntro += GameIntroHandler;
        GameManager.Instance.OnWinLevel += WinLevelHandler;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnGameIntro -= GameIntroHandler;
        GameManager.Instance.OnWinLevel -= WinLevelHandler;
    }

    private void Awake()
    {
        coll2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo el player puede colisionar con la exit door, por lo tanto no es necesario validar el tag
        GameManager.Instance.PlayerOnExitDoor();
    }

    private void WinLevelHandler()
    {
        Show();
    }

    private void GameIntroHandler()
    {
        Hide();
    }


    void Show()
    {
        SetActiveExitDoor(true);
    }

    void Hide()
    {
        SetActiveExitDoor(false);
    }

    void SetActiveExitDoor(bool value)
    {
        coll2D.enabled = value;
        spriteRenderer.enabled = value;
    }
}
