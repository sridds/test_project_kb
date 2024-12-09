using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Option : MonoBehaviour
{
    public QuestionPrompt questionPromptReference;
    public TextMeshPro optionText;
    public BoxCollider2D boxCollider;

    private bool isCorrectOption;

    private void OnEnable()
    {
        Unhover();
    }

    private void Update()
    {
        // this is not good practice, but in this case it doesn't really matter since it wont lag the game like crazy. sets the size to be the same as the text with the help of the ContentSizeFitter component
        boxCollider.size = new Vector2(optionText.rectTransform.sizeDelta.x, 0.65f);
    }

    public void SetupOption(string text, bool isCorrectOption)
    {
        // set text
        optionText.text = text;

        // This keeps track of whether this option is the correct one
        this.isCorrectOption = isCorrectOption;
    }

    //KBC TODO: Make this sparkly :)
    public void Hover()
    {
        optionText.color = Color.yellow;
    }

    public void Unhover()
    {
        optionText.color = Color.white;
    }

    public void SelectOption()
    {
        // Tell the question prompt to select this option.

        questionPromptReference.SelectOption(isCorrectOption);
    }
}
