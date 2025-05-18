using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static DialogueHandler;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField]
    private DialogueTextWriter _writer;

    [SerializeField]
    private RectTransform _dialogueBox;

    [SerializeField]
    private RectTransform _portraitContainer;

    [SerializeField]
    private Image _portrait;

    [SerializeField]
    private RectTransform _continueArrow;

    private int lastCount = 0;
    private bool isDialogueOpen = false;
    private DialogueData currentData;
    private Queue<DialogueData> dialoguePayload = new Queue<DialogueData>();

    public DialogueTextWriter Writer { get { return _writer; } }
    public delegate void QueueEmpty();
    public QueueEmpty OnQueueEmpty;


    private void Start()
    {
        //_writer.OnQueueEmpty += CloseDialogue;

        isDialogueOpen = false;

        _portraitContainer.gameObject.SetActive(false);
        _dialogueBox.gameObject.SetActive(false);
        _continueArrow.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueOpen) return;

        UpdatePortrait();
        UpdateInputs();
        UpdateContinueArrow();
    }

    private void UpdatePortrait()
    {
        if (currentData.Portrait == null || currentData.Portrait.Length == 0)
        {
            _portraitContainer.gameObject.SetActive(false);
        }
        else
        {
            _portraitContainer.gameObject.SetActive(true);
            _portrait.sprite = currentData.Portrait[0];
        }
    }

    private void UpdateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Z)) Continue();
        if (Input.GetKeyDown(KeyCode.X)) Skip();
    }

    private void Continue()
    {
        // Continue to next line

        if (!Writer.IsWriting && dialoguePayload.Count > 0)
        {
            Debug.Log($"Continued to next dialogue: " + dialoguePayload.Peek());

            currentData = dialoguePayload.Dequeue();
            Writer.WriteDialogue(currentData);

            lastCount = dialoguePayload.Count;
        }

        // Close
        else if (!Writer.IsWriting && dialoguePayload.Count == 0)
        {
            Debug.Log($"Failed to continue, dialogue queue is empty!");

            CloseDialogue();

            // Clean up everything, then close dialogue
            OnQueueEmpty?.Invoke();
            Writer.Cleanup();
            currentData = null;
            lastCount = dialoguePayload.Count;
        }
    }

    private void Skip()
    {
        if (!Writer.IsWriting || !currentData.AllowSkip) return;

        Writer.SkipToEnd();
    }

    private void UpdateContinueArrow()
    {
        // Show the continue arrow if ready to continue
        if (_writer.IsWriting && _continueArrow.gameObject.activeSelf)
        {
            _continueArrow.gameObject.SetActive(false);
        }
        // Otherwise, don't
        else if(!_writer.IsWriting && !_continueArrow.gameObject.activeSelf)
        {
            _continueArrow.gameObject.SetActive(true);
        }
    }
    
    public void HandleDialogue(DialogueData[] data)
    {
        // Open dialogue box if it isn't already
        if (!isDialogueOpen) OpenDialogue();

        foreach (DialogueData dialogue in data)
        {
            QueueDialoguePayload(dialogue);
        }
    }

    public void HandleDialogue(DialogueData data)
    {
        // Open dialogue box if it isn't already
        if (!isDialogueOpen) OpenDialogue();

        QueueDialoguePayload(data);
    }

    private void QueueDialoguePayload(DialogueData data)
    {
        // If we can, immediately start handling the text
        if (!Writer.IsWriting && lastCount == 0)
        {
            currentData = data;
            Writer.WriteDialogue(data);
        }
        // Otherwise, add to the queue
        else
        {
            dialoguePayload.Enqueue(data);
        }

        lastCount = dialoguePayload.Count;
    }

    private void OpenDialogue()
    {
        _continueArrow.gameObject.SetActive(false);
        isDialogueOpen = true;
        _dialogueBox.gameObject.SetActive(true);
    }

    private void CloseDialogue()
    {
        isDialogueOpen = false;
        _dialogueBox.gameObject.SetActive(false);
        _portraitContainer.gameObject.SetActive(false);
    }
}
