using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private RectTransform _holder;

    [SerializeField]
    private RectTransform _optionHolder;

    [SerializeField]
    private List<MenuOption> _options = new List<MenuOption>();

    private bool isInteractable;
    private bool isVisible;

    private int index;
    private int lastIndex = -1;

    public delegate void MenuItemSelected(int selectionIndex);
    public MenuItemSelected OnMenuItemSelected;

    public delegate void MenuItemHovered(int hoverIndex);
    public MenuItemHovered OnMenuItemHovered;

    public int CurrentIndex => index;

    private void Start()
    {
        foreach (MenuOption option in _options)
        {
            option.Unhover();
        }
    }

    public void AddMenuOption(MenuOption option, string text)
    {
        MenuOption newOption = Instantiate(option, _optionHolder);
        newOption.SetText(text);

        _options.Add(newOption);
    }

    public void SetTextAtIndex(string text, int index)
    {
        _options[index].SetText(text);
    }

    public void SetOptionActive(bool active, int index)
    {
        if (active) _options[index].Enable();
        else _options[index].Disable(false);
    }

    public int GetActiveCount()
    {
        int count = 0;

        for(int i = 0; i < _options.Count; i++)
        {
            if (_options[i].enabled)
            {
                count++;
            }
        }

        return count;
    }

    // Called externally
    public void UpdateMenu()
    {
        if (!isInteractable) return;

        GetInput();

        if(index != lastIndex) HoverOption();

        lastIndex = index;
    }

    public void Select()
    {
        OnMenuItemSelected?.Invoke(index);
    }

    private void GetInput()
    {
        if (_options.Count == 0) return;

        // Get next index
        if (Input.GetKeyDown(KeyCode.DownArrow)) index = GetNextValidIndex(false);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) index = GetNextValidIndex(true);
    }

    private int GetNextValidIndex(bool isReverse)
    {
        int startIndex = index;
        int currentIndex = index;

        do
        {
            currentIndex = (currentIndex + (isReverse ? -1 : 1)) % GetActiveCount();

            if (currentIndex < 0) currentIndex = GetActiveCount() - 1;

            if (_options[currentIndex].IsEnabled)
                return currentIndex;
        }
        while (currentIndex != startIndex); // Stop if we've looped back to the original index

        return -1; // No valid index found
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    private void HoverOption()
    {
        for (int i = 0; i < _options.Count; i++)
        {
            if(i == index)
            {
                _options[index].Hover();
                OnMenuItemHovered?.Invoke(index);
                continue;
            }

            _options[i].Unhover();
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        this.isInteractable = isInteractable;
    }

    public void SetLightDisable(bool lightDisable)
    {
        foreach (MenuOption option in _options)
        {
            if (lightDisable) option.Disable(false);
            else option.Enable();
        }
    }

    public void SetVisibility(bool isVisible)
    {
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
