using UnityEngine;

public class DrillingMinigame : Minigame
{
    [Header("Minigame Settings")]
    [SerializeField] private float xAcceleration = 0.5f;

    protected override void OnUpdateMinigame()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
    }
}
