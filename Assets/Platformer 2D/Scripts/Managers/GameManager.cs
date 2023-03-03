using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState { Intro, Start, WinLevel, Exit }

    [SerializeField] private int gameVersion;
    [SerializeField] private int targetFrameRate = 30;        

    GameState currentState;

    GameState CurrentState
    {
        get => currentState;
        set
        {
            // Aquí pueden ir los métodos Exit de cada estado

            currentState = value;

            switch (currentState)
            {                
                case GameState.Intro:
                    IntroEnter();
                    break;
                case GameState.Start:
                    StartEnter();
                    break;             
                case GameState.WinLevel:
                    WinLevelEnter();
                    break;
                case GameState.Exit:
                    ExitEnter();
                    break;
            }
        }
    }

    private static GameManager instance = null;
    public static GameManager Instance => instance;

    // Nomenclatura de eventos: ejemplo
    //   OnClosing: a close event that is raised before a window is closed
    //   OnClosed: one that is raised after the window is closed 
    public event Action OnGameIntro;
    public event Action OnGameStart;
    public event Action OnGameExit;
    public event Action OnPlayerStartMoving;
    public event Action OnPlayerStopMoving;
    public event Action<bool> OnPlayerIsGroundedChanged;
    public event Action OnPlayerJumping;
    public event Action<Vector3> OnPlayerStepping;
    public event Action OnPlayerBeginFalling;
    public event Action<CoinPickup> OnCoinCatched;
    public event Action OnWinLevel;

    int coinsCatched; // Cuando coinsCatched == coinsTotalCount => WinLevel
    List<CoinPickup> coins = new();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }

    private void Start()
    {
        print("Game Version: " + gameVersion);
#if UNITY_EDITOR
        Application.targetFrameRate = targetFrameRate;
#endif
        CurrentState = GameState.Intro;
    }

    void IntroEnter()
    {
        OnGameIntro?.Invoke();
        for (int i = 0; i < coins.Count; i++)
        {
            coins[i].gameObject.SetActive(true);
        }
    }

    void WinLevelEnter()
    {        
        OnWinLevel?.Invoke();
    }

    void StartEnter()
    {
        coinsCatched = 0;
        OnGameStart?.Invoke();
        
    }


    public void PlayerStartMoving()
    {
        OnPlayerStartMoving?.Invoke();
    }

    public void PlayerStoptMoving()
    {
        OnPlayerStopMoving?.Invoke();
    }

    public void PlayerIsGroundedChanged(bool value)
    {
        OnPlayerIsGroundedChanged?.Invoke(value);
    }

    public void PlayerJumping()
    {
        OnPlayerJumping?.Invoke();
    }

    public void PlayerStep(Vector3 footPosition)
    {
        OnPlayerStepping?.Invoke(footPosition);
    }

    public void PlayerBeginFalling()
    {
        OnPlayerBeginFalling?.Invoke();
    }

    public void CoinCatched(CoinPickup coin)
    {
        coinsCatched++;
        coin.gameObject.SetActive(false);
        OnCoinCatched?.Invoke(coin);
        if (coinsCatched == coins.Count)
        {
            CurrentState = GameState.WinLevel;
        }
    }

    public void CoinCreated(CoinPickup coin)
    {
        coins.Add(coin);
    }

    public void VFXIntroDone()
    {
        CurrentState = GameState.Start;
        
    }

    public void VFXExitDone()
    {
        CurrentState = GameState.Intro;
    }


    public void PlayerOnExitDoor()
    {
        CurrentState = GameState.Exit;
    }

    void ExitEnter()
    {
        OnGameExit?.Invoke();
    }   


}
