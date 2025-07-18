using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private AudioSource _soundSource;

    [SerializeField]
    private AudioSource _musicSource;

    [Header("Defaults")]
    [SerializeField]
    private AudioClip _defaultBattleMusic;

    [SerializeField]
    private AudioClip _areaMusic;


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

    public void PlaySound(AudioStream stream)
    {
        _soundSource.pitch = stream.GetPitch();
        _soundSource.PlayOneShot(stream.GetClip(), stream.volumeScale);
    }

    public void PlayTrack(AudioClip clip)
    {
        _musicSource.mute = false;
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PauseMusic()
    {
        _musicSource.Pause();
    }

    public void PlayDefaultBattleMusic()
    {
        _musicSource.clip = _defaultBattleMusic;
        _musicSource.Play();
    }
}
