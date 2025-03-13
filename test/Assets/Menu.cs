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
        MenuOption newOption = Instantiate(option, _holder);
        newOption.SetText(text);

        _options.Add(newOption);
    }

    // Called externally
    public void UpdateMenu()
    {
        if (!isInteractable) return;

        GetInput();

        if(index != lastIndex) HoverOption();

        lastIndex = index;
    }

    private void GetInput()
    {
        if (_options.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.DownArrow)) index++;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) index--;

        if(Input.GetKeyDown(KeyCode.Z))
        {
            OnMenuItemSelected?.Invoke(index);
        }

        // Ensure index doesn't go out of range
        if (index < 0) index = _options.Count - 1;
        index %= _options.Count;
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
