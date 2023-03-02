using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] protected AudioSource BGMAudioSource;
    [SerializeField] protected AudioSource SFXAudioSource;
    [SerializeField] private AudioManagerData data;
    [SerializeField] private AudioMixerSnapshot snapshotOn;
    [SerializeField] private AudioMixerSnapshot snapshotOff;
    [SerializeField] private float timeToReachSnapshotOff = 4f;


    protected Coroutine audioRoutine;
    HashSet<AudioClip> clipsPlayedThisFrame;

    private void OnEnable()
    {
        GameManager.Instance.OnGameIntro += GameIntroHandler;
        GameManager.Instance.OnGameStart += GameStartedHandler;
        GameManager.Instance.OnGameExit += GameExitHandler;
        GameManager.Instance.OnPlayerJumping += PlayerJumpingHandler;
        GameManager.Instance.OnPlayerIsGroundedChanged += PlayerIsGroundedChangedHandler;
        GameManager.Instance.OnPlayerStepping += PlayerSteppingHandler;
        GameManager.Instance.OnPlayerBeginFalling += PlayerBeginFallingHandler;
        GameManager.Instance.OnCoinCatched += CoinCatchedHandler;
        GameManager.Instance.OnWinLevel += WinLevelHandler;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnGameIntro -= GameIntroHandler;
        GameManager.Instance.OnGameStart -= GameStartedHandler;
        GameManager.Instance.OnGameExit -= GameExitHandler;
        GameManager.Instance.OnPlayerJumping -= PlayerJumpingHandler;
        GameManager.Instance.OnPlayerIsGroundedChanged -= PlayerIsGroundedChangedHandler;
        GameManager.Instance.OnPlayerStepping -= PlayerSteppingHandler;
        GameManager.Instance.OnPlayerBeginFalling -= PlayerBeginFallingHandler;
        GameManager.Instance.OnCoinCatched -= CoinCatchedHandler;
        GameManager.Instance.OnWinLevel -= WinLevelHandler;
    }


    protected void Awake() => clipsPlayedThisFrame = new HashSet<AudioClip>();

    protected void LateUpdate() => clipsPlayedThisFrame.Clear();

    private void GameExitHandler()
    {        
        snapshotOff.TransitionTo(timeToReachSnapshotOff);
    }
    private void GameIntroHandler()
    {
        snapshotOn.TransitionTo(0);
        float delay = PlayRandomMusic(data.noiseMusic, false);
    }

    private void WinLevelHandler()
    {
        float duration = PlayRandomSound(data.bravo, SFXAudioSource);
        PlayRandomMusicWithDelay(data.winLevelMusic, false, duration, true);
    }


    private void CoinCatchedHandler(IVFXEntity coin)
    {
        PlayRandomSound(data.coinPickup, SFXAudioSource);
    }

    private void PlayerBeginFallingHandler()
    {
        float volumeFalling = 0.6f;
        PlayRandomSound(data.playerFalling, SFXAudioSource, volumeFalling);
    }


    private void PlayerSteppingHandler(Vector3 value)
    {
        PlayRandomSound(data.playerFootsteps, SFXAudioSource);
    }

    private void PlayerIsGroundedChangedHandler(bool value)
    {
        if (value)
        {
            float volumeScaleLandingSound = 0.5f;
            PlayRandomSound(data.playerLand, SFXAudioSource, volumeScaleLandingSound);
        }
    }

    private void PlayerJumpingHandler()
    {
        PlayRandomSound(data.playerJump, SFXAudioSource);
    }

    private void GameStartedHandler()
    {
        PlayRandomMusic(data.mainMusic, true);
    }

    


    protected float PlayRandomMusic(AudioClip[] clips, bool loop)
    {
        StopAudioRoutine();
        var clip = GetRandomClip(clips);
        PlayBGMMusic(clip, loop);
        return clip.length;
    }

    protected void PlayRandomMusicWithDelay(AudioClip[] clips, bool loop, float delay, bool stopGameMusic)
    {
        if (stopGameMusic)
            StopGameMusic();
        audioRoutine = StartCoroutine(PlayRandomMusicWithDelayRoutine(clips, loop, delay));
    }

    IEnumerator PlayRandomMusicWithDelayRoutine(AudioClip[] clips, bool loop, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayRandomMusic(clips, loop);
    }


    protected void PlayRandomSoundWhitLoop(AudioClip[] clips, AudioSource audioSource)
    {
        if (clips == null || clips.Length == 0) return;
        var clip = GetRandomClip(clips);
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    protected float PlayRandomSound(AudioClip[] clips, AudioSource audioSource, float volumeScale = 1f)
    {
        if (clips == null || clips.Length == 0) return 0f;
        var clip = GetRandomClip(clips);
        SFXPlayOneShot(clip, audioSource, volumeScale);
        return clip.length;
    }

    protected void PlayRandomSoundWithDelay(AudioClip[] clips, AudioSource audioSource, float delay, float volumeScale = 1f)
    {
        StartCoroutine(PlayRandomSoundWithDelayRoutine(clips, audioSource, delay, volumeScale));
    }

    IEnumerator PlayRandomSoundWithDelayRoutine(AudioClip[] clips, AudioSource audioSource, float delay, float volumeScale = 1f)
    {
        yield return new WaitForSeconds(delay);
        PlayRandomSound(clips, audioSource, volumeScale);
    }

    protected AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        int randomIdx = Random.Range(0, audioClips.Length);
        return audioClips[randomIdx];
    }

    void SFXPlayOneShot(AudioClip clip, AudioSource audioSource, float volumeScale = 1f)
    {
        if (!clipsPlayedThisFrame.Contains(clip))
        {            
            audioSource.PlayOneShot(clip, volumeScale);
            clipsPlayedThisFrame.Add(clip);
        }
    }


    protected void PlayBGMMusic(AudioClip clip, bool loop)
    {
        BGMAudioSource.loop = loop;
        BGMAudioSource.Stop();
        BGMAudioSource.clip = clip;
        BGMAudioSource.Play();
    }

    protected void StopAudioRoutine()
    {
        if (audioRoutine != null)
            StopCoroutine(audioRoutine);
    }

    protected void StopGameMusic()
    {
        StopAudioRoutine();
        BGMAudioSource.Stop();
    }
}
