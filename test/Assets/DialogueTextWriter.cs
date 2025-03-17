using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum EDialogueAppearance
{
    Typewriter,
    Immediate,
}

public struct DialogueData
{
    public string Text;
    public EDialogueAppearance Appearance;
}

public class DialogueTextWriter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textUI;

    [SerializeField]
    private float _defaultTextSpeed;

    private Coroutine activeDialogueCoroutine;
    private Queue<DialogueData> dialoguePayload = new Queue<DialogueData>();

    public delegate void QueueEmpty();
    public delegate void Continue();

    public QueueEmpty OnQueueEmpty;
    public Continue OnContinue;

    // Called externally to update the current text
    public void TryContinue()
    {
        // Continue to next line
        if (activeDialogueCoroutine == null && dialoguePayload.Count > 0)
        {
            Debug.Log($"Continued to next dialogue: " + dialoguePayload.Peek());

            activeDialogueCoroutine = StartCoroutine(IHandleText(dialoguePayload.Dequeue()));
            OnContinue?.Invoke();
        }

        // Close
        else if (dialoguePayload.Count == 0)
        {
            Debug.Log($"Failed to continue, dialogue queue is empty!");

            Clear();
            OnQueueEmpty?.Invoke();
        }
    }

    public void TrySkip()
    {
        Debug.Log($"Haven't written the code for skipping text yet :p");
    }

    public void QueueDialoguePayload(DialogueData data)
    {
        // If we can, immediately start handling the text
        if (activeDialogueCoroutine == null)
        {
            activeDialogueCoroutine = StartCoroutine(IHandleText(data));
        }
        // Otherwise, add to the queue
        else
        {
            dialoguePayload.Enqueue(data);
        }
    }

    public void Cleanup()
    {
        Clear();
        activeDialogueCoroutine = null;
        dialoguePayload.Clear();
    }

    private IEnumerator IHandleText(DialogueData data)
    {
        Clear();
        yield return null;

        if(data.Appearance == EDialogueAppearance.Immediate)
        {
            _textUI.text = data.Text;
        }
        else if(data.Appearance == EDialogueAppearance.Typewriter)
        {
            // Default typewriter effect
            for (int i = 0; i < data.Text.Length; i++)
            {
                _textUI.text += data.Text[i];

                yield return new WaitForSeconds(_defaultTextSpeed);
            }
        }

        activeDialogueCoroutine = null;
    }

    private void Clear()
    {
        _textUI.text = "";
    }
}
