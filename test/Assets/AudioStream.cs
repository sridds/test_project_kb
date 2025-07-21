using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "AudioStream")]
public class AudioStream : ScriptableObject
{
    [Header("References")]
    public AudioClip[] clips;

    [Header("Volume")]
    public float volumeScale = 1.0f;

    [Header("Pitch")]
    public bool randomizePitch;

    [ShowIf(nameof(randomizePitch))]
    public Vector2 pitchRange = new Vector2(1.0f, 1.0f);

    [HideIf(nameof(randomizePitch))]
    public float defaultPitch = 1.0f;

    public AudioClip GetClip() => clips[Random.Range(0, clips.Length)];

    public AudioClip GetClipAtIndex(int index) => clips[index];

    public float GetPitch() => randomizePitch ? Random.Range(pitchRange.x, pitchRange.y) : defaultPitch;
}