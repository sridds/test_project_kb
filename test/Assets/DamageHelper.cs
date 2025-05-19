using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for taking damage data and instantiating hitmarkers and performance text
/// </summary>
public class DamageHelper : MonoBehaviour
{
    public enum EDamagePerformance
    {
        OK,
        GOOD,
        GREAT,
        PERFECT
    }

    [Header("References")]
    [SerializeField]
    private DamageTotal _damageTotalPrefab;

    [SerializeField]
    private SpriteNumbers _damageHitmarker;

    [SerializeField]
    private GameObject _perfectPerformanceMarker;

    [SerializeField]
    private GameObject _greatPerformanceMarker;

    [SerializeField]
    private GameObject _goodPerformanceMarker;

    [SerializeField]
    private GameObject _okayPerformanceMarker;

    [Header("Audio")]
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

    public void SpawnPerformanceHitmarker(EDamagePerformance performance, Vector2 point)
    {
        switch (performance)
        {
            case EDamagePerformance.OK:
                Instantiate(_okayPerformanceMarker, point, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_okClip);

                break;
            case EDamagePerformance.GOOD:
                Instantiate(_goodPerformanceMarker, point, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_okClip);

                break;
            case EDamagePerformance.GREAT:
                Instantiate(_greatPerformanceMarker, point, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_greatClip);

                break;
            case EDamagePerformance.PERFECT:
                Instantiate(_perfectPerformanceMarker, point, Quaternion.identity);
                _performanceAudioSource.PlayOneShot(_perfectClip);

                break;
        }
    }

    private void Update()
    {
        // debug - start chain
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 hit = FindFirstObjectByType<FieldEnemy>().transform.position + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y);

            DamageChain(FindFirstObjectByType<FieldEnemy>().gameObject, Random.Range(5, 15), hit);

            SpawnPerformanceHitmarker((EDamagePerformance)Random.Range(0, 4), hit + new Vector3(0, 3.0f));
        }
        // debug - end chain
        if (Input.GetKeyDown(KeyCode.F))
        {
            EndDamageChain(FindFirstObjectByType<FieldEnemy>().gameObject);
        }
    }

    public void DamageChain(GameObject key, int value, Vector3 hitPoint)
    {
        // Spawn hitmarker at object position

        SpriteNumbers hitmarker = Instantiate(_damageHitmarker, hitPoint, Quaternion.identity);
        hitmarker.SetValue(value);

        // Add value to key
        if (damageChains.ContainsKey(key))
        {
            damageChains[key] += value;
        }
        else
        {
            damageChains.Add(key, value);
        }
    }

    public void EndDamageChain(GameObject key)
    {
        if (!damageChains.ContainsKey(key)) return;

        // show total damage
        DamageTotal total = Instantiate(_damageTotalPrefab, key.transform.position, Quaternion.identity);
        total.Numbers.SetValue(damageChains[key]);

        damageChains.Remove(key);
    }
}
