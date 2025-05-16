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

    public delegate void DialogueFinished(DialogueData data);
    public DialogueFinished OnDialogueFinished;

    public bool IsWriting { get { return isWriting; } }

    public void WriteDialogue(DialogueData data)
    {
        // Stop coroutine if one is currently running
        if(activeDialogueCoroutine != null) StopCoroutine(activeDialogueCoroutine);

        currentDialogueData = data;
        activeDialogueCoroutine = StartCoroutine(IHandleText(data));
    }

    public void SkipToEnd()
    {
        StopCoroutine(activeDialogueCoroutine);
        isWriting = false;
        _textUI.text = GetFullText(currentDialogueData);

        activeDialogueCoroutine = null;
    }

    public void Cleanup()
    {
        Clear();
        activeDialogueCoroutine = null;
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
