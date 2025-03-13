using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private RectTransform _holder;

    [SerializeField]
    private List<MenuOption> _options = new List<MenuOption>();

    private bool isInteractable;
    private bool isVisible;
    private int index;

    // Called externally
    public void UpdateMenu()
    {
        if (!isInteractable) return;

        GetInput();
        HoverOption();
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) index++;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) index--;

        // Ensure index doesn't go out of range
        if (index < 0) index = _options.Count - 1;
        index %= _options.Count;
    }

    private void HoverOption()
    {
        _options[index].Hover();

        for(int i = 0; i < _options.Count; i++)
        {
            if(i == index)
            {
                _options[index].Hover();
                continue;
            }

            _options[i].Unhover();
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        this.isInteractable = isInteractable;
    }

    public void SetVisibility(bool isVisible)
    {
        // No change, don't do anything
        if (isVisible == this.isVisible) return;

        // Show or hide based on the bool
        if (isVisible) Show();
        else Hide();

        // Set bool internally
        this.isVisible = isVisible;
    }

    private void Hide()
    {
        // replace with cool animation here
        _holder.gameObject.SetActive(false);
    }

    private void Show()
    {
        // replace with cool animation here
        _holder.gameObject.SetActive(true);
    }
}
