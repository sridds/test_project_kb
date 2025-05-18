using UnityEngine;


public class InteractionTrigger : MonoBehaviour
{
    private const float ACTIVATION_COOLDOWN = 0.3f;

    public enum ETriggerDestroy
    {
        DontDestroy,
        DestroyComponent,
        DestroyGameObject
    }

    [SerializeField]
    private Interactable interactable;
    [SerializeField]
    private bool _requireInteraction;
    [SerializeField]
    private ETriggerDestroy _destroySettings;

    private bool readyToInteract;
    private float activeTimer;

    void Update()
    {  
        // Tick timer up during cooldown
        if(activeTimer < ACTIVATION_COOLDOWN && !interactable.IsInteracting)
        {
            activeTimer += Time.deltaTime;
            return;
        }

        if (!readyToInteract) return;

        if(_requireInteraction && Input.GetKeyDown(KeyCode.Z) || !_requireInteraction)
        {
            HandleInteraction();

            readyToInteract = false;
            activeTimer = 0.0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("stay 1");

        if (interactable.IsInteracting) return;

        Debug.Log("stay 2");

        readyToInteract = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        readyToInteract = false;
    }

    private void HandleInteraction()
    {
        interactable.Interact();

        if (_destroySettings == ETriggerDestroy.DestroyComponent) Destroy(this);
        else if (_destroySettings == ETriggerDestroy.DestroyGameObject) Destroy(gameObject);
    }
}
