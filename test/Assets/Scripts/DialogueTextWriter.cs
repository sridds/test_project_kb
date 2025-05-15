using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum EDialogueAppearance
{
    Typewriter,
    Immediate,
}

[System.Serializable]
public class DialogueData
{
    public EDialogueAppearance Appearance;
    [TextArea]
    public string Text;
    public Sprite[] Portrait;
    public AudioClip Blit;

    [Header("Advanced Settings")]
    public bool AllowSkip = true;
}

public class DialogueTextWriter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textUI;

    [SerializeField]
    private float _defaultTextSpeed;

    [SerializeField]
    private float _defaultWaitTime = 0.3f;

    [SerializeField]
    private AudioSource _source;

    private bool isWriting;
    private Coroutine activeDialogueCoroutine;
    private DialogueData currentDialogueData;

    private Queue<DialogueData> dialoguePayload = new Queue<DialogueData>();

    public delegate void QueueEmpty();
    public delegate void Continue();

    public QueueEmpty OnQueueEmpty;
    public Continue OnContinue;

    public bool IsWriting { get { return isWriting; } }

    // Called externally to update the current text
    public void TryContinue()
    {
        // Continue to next line
        if (activeDialogueCoroutine == null && dialoguePayload.Count > 0)
        {
            Debug.Log($"Continued to next dialogue: " + dialoguePayload.Peek());

            activeDialogueCoroutine = StartCoroutine(IHandleText(dialoguePayload.Dequeue()));
            OnContinue?.Invoke();

            lastCount = dialoguePayload.Count;
        }

        // Close
        else if (activeDialogueCoroutine == null && dialoguePayload.Count == 0)
        {
            Debug.Log($"Failed to continue, dialogue queue is empty!");

            Clear();
            OnQueueEmpty?.Invoke();
            currentDialogueData = null;

            lastCount = dialoguePayload.Count;
        }
    }

    public void TrySkip()
    {
        if (!isWriting || !currentDialogueData.AllowSkip) return;

        // Skip to end of line
        StopCoroutine(activeDialogueCoroutine);
        _textUI.text = GetFullText(currentDialogueData);

        activeDialogueCoroutine = null;
        isWriting = false;
    }

    int lastCount = 0;
    public void QueueDialoguePayload(DialogueData data)
    {
        // If we can, immediately start handling the text
        if (activeDialogueCoroutine == null && lastCount == 0)
        {
            activeDialogueCoroutine = StartCoroutine(IHandleText(data));
        }
        // Otherwise, add to the queue
        else
        {
            dialoguePayload.Enqueue(data);
        }

        lastCount = dialoguePayload.Count;
    }

    public void Cleanup()
    {
        Clear();
        activeDialogueCoroutine = null;
        dialoguePayload.Clear();
    }

    private string GetFullText(DialogueData data)
    {
        string builder = "";

        for(int i = 0; i < data.Text.Length; i++)
        {
            // Skip over attributes
            if(data.Text[i] == '&')
            {
                continue;
            }

            builder += data.Text[i];
        }

        return builder;
    }

    private IEnumerator IHandleText(DialogueData data)
    {
        isWriting = true;

        // Setup
        currentDialogueData = data;
        Clear();

        yield return null;

        if (data.Appearance == EDialogueAppearance.Immediate)
        {
            _textUI.text = GetFullText(data);
        }

        else if (data.Appearance == EDialogueAppearance.Typewriter)
        {
            // Default typewriter effect
            for (int i = 0; i < data.Text.Length; i++)
            {
                if(data.Text[i] == '&')
                {
                    yield return new WaitForSeconds(_defaultWaitTime);
                    continue;
                }

                // Play type writer sound
                if(data.Blit != null && data.Text[i] != ' ')
                {
                    _source.PlayOneShot(data.Blit);
                }

                _textUI.text += data.Text[i];
                yield return new WaitForSeconds(_defaultTextSpeed);
            }
        }

        activeDialogueCoroutine = null;
        isWriting = false;
    }

    private void Clear()
    {
        _textUI.text = "";
    }
}
