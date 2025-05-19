using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private AudioSource _musicSource;

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
