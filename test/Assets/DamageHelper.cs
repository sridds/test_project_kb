using UnityEngine;

public class DamageHelper : MonoBehaviour
{
    [SerializeField]
    private GameObject _perfectPerformanceMarker;

    [SerializeField]
    private GameObject _greatPerformanceMarker;

    [SerializeField]
    private GameObject _goodPerformanceMarker;

    [SerializeField]
    private GameObject _okayPerformanceMarker;

    [SerializeField]
    private AudioSource _performanceAudioSource;

    [SerializeField]
    private AudioClip _perfectClip;

    [SerializeField]
    private AudioClip _greatClip;

    [SerializeField]
    private AudioClip _okClip;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(_perfectPerformanceMarker, transform.position, Quaternion.identity);
            _performanceAudioSource.PlayOneShot(_perfectClip);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(_greatPerformanceMarker, transform.position, Quaternion.identity);
            _performanceAudioSource.PlayOneShot(_greatClip);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            Instantiate(_goodPerformanceMarker, transform.position, Quaternion.identity);
            _performanceAudioSource.PlayOneShot(_okClip);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Instantiate(_okayPerformanceMarker, transform.position, Quaternion.identity);
            _performanceAudioSource.PlayOneShot(_okClip);
        }
    }
}
