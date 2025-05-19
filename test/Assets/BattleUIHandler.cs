using UnityEngine;

public class BattleUIHandler : MonoBehaviour
{
    [SerializeField]
    private DialogueTextWriter _writer;

    [SerializeField]
    private RectTransform _dialogueBox;

    public void ShowFlavorText(DialogueData[] text, Battle.EFlavorTextAppearanceType appearanceType, int turnIndex)
    {
        _dialogueBox.gameObject.SetActive(true);
        DialogueData textToShow = null;

        switch(appearanceType)
        {
            case Battle.EFlavorTextAppearanceType.FirstThenShuffle:
                // Show first line first, other wise just pick randomly
                textToShow = turnIndex == 0 ? text[0] : text[Random.Range(0, text.Length)];
                break;
            case Battle.EFlavorTextAppearanceType.Shuffle:
                textToShow = text[Random.Range(0, text.Length)];
                break;
            case Battle.EFlavorTextAppearanceType.Loop:
                textToShow = text[Random.Range(0, text.Length)];
                break;
            default:
                Debug.Log(appearanceType + " was not accounted for as an appearanceType!");
                break;
        }

        _writer.WriteDialogue(textToShow);
    }
}
