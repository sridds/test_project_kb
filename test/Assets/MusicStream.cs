using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "MusicStream")]
public class MusicStream : ScriptableObject
{
    [Header("Default Settings")]
    public AudioClip mainTrack;
    public float defaultPitch = 1.0f;
    public float volumeScale = 1.0f;

    [Header("Intro")]
    public bool hasSpecialIntro;

    [ShowIf(nameof(hasSpecialIntro))]
    public AudioClip intro;
}
