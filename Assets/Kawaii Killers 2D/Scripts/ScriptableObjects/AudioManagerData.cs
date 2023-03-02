using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AudioManager Data", menuName = "Platformerd 2D/Audio Manager Data")]
public class AudioManagerData : ScriptableObject
{
    [Header("BGM Sounds")]
    public AudioClip[] mainMusic;
    public AudioClip[] noiseMusic;
    public AudioClip[] winLevelMusic;

    [Space(10)]
    [Header("Player SFX Sounds")]
    public AudioClip[] playerFootsteps;
    public AudioClip[] playerJump;
    public AudioClip[] playerLand;
    public AudioClip[] playerFalling;

    [Space(10)]
    [Header("Gameplay SFX Sounds")]
    public AudioClip[] coinPickup;

    [Space(10)]
    [Header("Voice Sounds")]
    public AudioClip[] bravo;
}
