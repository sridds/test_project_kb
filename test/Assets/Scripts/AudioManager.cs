using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("References")]
    [SerializeField]
    private AudioSource _soundSource;

    [SerializeField]
    private AudioSource _musicSource;

    [Header("Defaults")]
    [SerializeField]
    private MusicStream _defaultBattleMusic;
    [SerializeField]
    private MusicStream _defaultAreaMusic;
    

    private MusicStream currentMusicTrack;
    public MusicStream CurrentMusicTrack { get { return currentMusicTrack; } }
    public MusicStream DefaultAreaMusic { get { return _defaultAreaMusic; } }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeOutMusic(float duration)
    {
        _musicSource.DOFade(0.0f, duration);
    }

    public void PlaySound(AudioStream stream)
    {
        _soundSource.pitch = stream.GetPitch();
        _soundSource.PlayOneShot(stream.GetClip(), stream.volumeScale);
    }

    public void PlayTrack(MusicStream clip)
    {
        currentMusicTrack = clip;

        _musicSource.mute = false;
        _musicSource.pitch = clip.defaultPitch;
        _musicSource.volume = clip.volumeScale;
        _musicSource.clip = clip.mainTrack;
        _musicSource.Play();
    }

    public void PauseMusic()
    {
        _musicSource.Pause();
    }

    public void PlayDefaultBattleMusic()
    {
        currentMusicTrack = _defaultBattleMusic;

        _musicSource.clip = _defaultBattleMusic.mainTrack;
        _musicSource.pitch = _defaultBattleMusic.defaultPitch;
        _musicSource.volume = _defaultBattleMusic.volumeScale;
        _musicSource.Play();
    }
}
