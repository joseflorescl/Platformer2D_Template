using System.Collections;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private VFXPlayable coinCatched;
    [SerializeField] private VFXPlayable bravo;
    [SerializeField] private VFXPlayable dustRun;
    [SerializeField] private VFXPlayable dustJump;
    [SerializeField] private VFXPlayable screenNoise;
    [SerializeField] private VFXAnimations irisTransition;
    [SerializeField] private Transform footsLanding;
    [SerializeField] private float secondsScreenNoise = 2f;    
    [SerializeField] private AnimationClip irisAnimation;
    [SerializeField] private float delayBeforeExit = 1f;

    private void OnEnable()
    {        
        GameManager.Instance.OnGameIntro += GameIntroHandler;
        GameManager.Instance.OnCoinCatched += CoinCatchedHandler;
        GameManager.Instance.OnWinLevel += WinLevelHandler;
        GameManager.Instance.OnPlayerStepping += PlayerSteppingHandler;
        GameManager.Instance.OnPlayerIsGroundedChanged += PlayerIsGroundedChangedHandler;
        GameManager.Instance.OnGameExit += GameExitHandler;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnGameIntro -= GameIntroHandler;
        GameManager.Instance.OnCoinCatched -= CoinCatchedHandler;
        GameManager.Instance.OnWinLevel -= WinLevelHandler;
        GameManager.Instance.OnPlayerStepping -= PlayerSteppingHandler;
        GameManager.Instance.OnPlayerIsGroundedChanged -= PlayerIsGroundedChangedHandler;
        GameManager.Instance.OnGameExit -= GameExitHandler;
    }

    private void GameExitHandler()
    {
        StartCoroutine(VFXExitRoutine());
    }


    IEnumerator VFXExitRoutine()
    {
        yield return new WaitForSeconds(delayBeforeExit);
        irisTransition.PlayOneShotInReverse();
        yield return new WaitForSeconds(irisAnimation.length);

        GameManager.Instance.VFXExitDone();        
    }


    private void GameIntroHandler()
    {
        StartCoroutine(VFXIntroRoutine());
    }

    IEnumerator VFXIntroRoutine()
    {
        screenNoise.Play();
        yield return new WaitForSeconds(secondsScreenNoise);
        screenNoise.Stop();
        irisTransition.PlayOneShot();
        GameManager.Instance.VFXIntroDone();
    }

    private void PlayerIsGroundedChangedHandler(bool value)
    {
        if (value)
        {
            dustJump.PlayOneShot(footsLanding.position);
        }
    }


    private void PlayerSteppingHandler(Vector3 footPosition)
    {
        dustRun.PlayOneShot(footPosition);
    }


    private void CoinCatchedHandler(IVFXEntity coin)
    {
        coinCatched.PlayOneShot(coin.Position);        
    }

    private void WinLevelHandler()
    {        
        bravo.PlayOneShot();
    }   

}
