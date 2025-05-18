using System.Collections.Generic;
using UnityEngine;

public class DamageHelper : MonoBehaviour
{
    public enum EDamagePerformance
    {
        OK,
        GOOD,
        GREAT,
        PERFECT
    }

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

    // replace gameobject later w enemy script
    private Dictionary<GameObject, int> damageChains = new Dictionary<GameObject, int>();

    public void SpawnPerformanceHitmarker(EDamagePerformance performance)
    {
        switch (performance)
        {
            case EDamagePerformance.OK:
                Instantiate(_okayPerformanceMarker, transform.position, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_okClip);

                break;
            case EDamagePerformance.GOOD:
                Instantiate(_goodPerformanceMarker, transform.position, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_okClip);

                break;
            case EDamagePerformance.GREAT:
                Instantiate(_greatPerformanceMarker, transform.position, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_greatClip);

                break;
            case EDamagePerformance.PERFECT:
                Instantiate(_perfectPerformanceMarker, transform.position, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_perfectClip);

                break;
        }
    }

    public void StartDamageChain(GameObject key, int value)
    {
        // Spawn hitmarker at object position
    }

    public void EndDamageChain(GameObject key)
    {
        // show total damage
    }
}
