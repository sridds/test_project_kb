using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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

    private bool isDialogueOpen = false;

    public DialogueTextWriter Writer { get { return _writer; } }

    private void Start()
    {
        _writer.OnQueueEmpty += CloseDialogue;

        isDialogueOpen = false;

        _portraitContainer.gameObject.SetActive(false);
        _dialogueBox.gameObject.SetActive(false);
        _continueArrow.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueOpen) return;

        if (Input.GetKeyDown(KeyCode.Z)) _writer.TryContinue();
        if (Input.GetKeyDown(KeyCode.X)) _writer.TrySkip();

        UpdateContinueArrow();
    }

    private void UpdateContinueArrow()
    {
        if (_writer.IsWriting && _continueArrow.gameObject.activeSelf)
        {
            _continueArrow.gameObject.SetActive(false);
        }
        else if(!_writer.IsWriting && !_continueArrow.gameObject.activeSelf)
        {
            _continueArrow.gameObject.SetActive(true);
        }
    }
    
    public void HandleDialogue(DialogueData[] data)
    {
        foreach (DialogueData dialogue in data)
        {
            HandleDialogue(dialogue);
        }
    }

    public void HandleDialogue(DialogueData data)
    {
        // Open dialogue box if it isn't already
        if (!isDialogueOpen) OpenDialogue();

        // Show portrait
        if(data.Portrait != null && data.Portrait.Length > 0)
        {
            _portraitContainer.gameObject.SetActive(true);

            _portrait.sprite = data.Portrait[0];
        }
        // Hide portrait
        else
        {
            _portraitContainer.gameObject.SetActive(false);
        }

        _writer.QueueDialoguePayload(data);
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
    }
}
