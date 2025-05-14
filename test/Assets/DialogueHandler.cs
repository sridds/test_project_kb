using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField]
    private DialogueTextWriter _writer;

    [SerializeField]
    private RectTransform _dialogueBox;

    private bool isDialogueOpen = false;

    private void Start()
    {
        _writer.OnQueueEmpty += CloseDialogue;

        isDialogueOpen = false;
        _dialogueBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) HandleDialogue(new DialogueData() { Text = "Here's the information " + Random.Range(500, 10000), Appearance = EDialogueAppearance.Typewriter });

        if (!isDialogueOpen) return;

        // Try to continue if the player presses mouse button
        if (Input.GetKeyDown(KeyCode.Z)) _writer.TryContinue();
    }

    public void HandleDialogue(DialogueData data)
    {
        // Open dialogue box if it isn't already
        if (!isDialogueOpen) OpenDialogue();

        _writer.QueueDialoguePayload(data);
    }

    private void OpenDialogue()
    {
        isDialogueOpen = true;
        _dialogueBox.gameObject.SetActive(true);
    }

    private void CloseDialogue()
    {
        isDialogueOpen = false;
        _dialogueBox.gameObject.SetActive(false);
    }
}
