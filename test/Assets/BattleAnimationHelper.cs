using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleAnimationHelper : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _jumpGravity;
    [SerializeField] private float _fallGravity;
    [SerializeField] private string _jumpHash;
    [SerializeField] private string _fallHash;

    private float currentYPlane;
    private bool isJumping;

    public IEnumerator JumpToPosition(Vector3 targetPosition, float arcHeight)
    {
        if (isJumping) yield break;

        _animator.Play(_jumpHash);

        isJumping = true;
        Vector3 startPosition = _target.transform.position;

        // Calculate the parabolic path parameters
        float horizontalDistance = Vector3.Distance(new Vector3(startPosition.x, 0, startPosition.z),
                                                   new Vector3(targetPosition.x, 0, targetPosition.z));

        // Calculate time to reach peak and total flight time
        float timeToReachPeak = Mathf.Sqrt(2f * arcHeight / _jumpGravity);
        float timeToFall = Mathf.Sqrt(2f * arcHeight / _fallGravity);
        float totalTime = timeToReachPeak + timeToFall;

        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            float t = elapsedTime / totalTime;

            // Interpolate horizontal position
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);

            // Calculate vertical position based on current phase
            float verticalOffset;

            if (elapsedTime <= timeToReachPeak)
            {
                // Ascending phase
                float ascendProgress = elapsedTime / timeToReachPeak;
                verticalOffset = arcHeight * (2f * ascendProgress - ascendProgress * ascendProgress);
            }
            else
            {
                // Descending phase
                float descendTime = elapsedTime - timeToReachPeak;
                float descendProgress = descendTime / timeToFall;
                verticalOffset = arcHeight * (1f - descendProgress * descendProgress);
            }

            currentPos.y = Mathf.Lerp(startPosition.y, targetPosition.y, t) + verticalOffset;

            _target.transform.position = currentPos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end exactly at target
        _animator.Play(_fallHash);
        _target.transform.position = targetPosition;
        isJumping = false;
    }

}
