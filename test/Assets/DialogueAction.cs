using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueAction : SequencerAction
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData[] _dialogueData;

    private DialogueHandler _handler;

    public override async Task ExecuteActionAsync(Sequencer sequencer)
    {
        if(_handler == null) _handler = FindFirstObjectByType<DialogueHandler>();
        if (_dialogueData.Length == 0)
        {
            Debug.Log("No dialogue in array, failed to queue from Sequencer!");
            return;
        }

        //await _handler.HandleDialogueAsync(_dialogueData);
    }
}