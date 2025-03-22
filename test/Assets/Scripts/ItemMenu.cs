using System;
using System.Collections.Generic;
using UnityEngine;
using static Menu;

public class ItemMenu : MonoBehaviour
{
    [SerializeField]
    private RectTransform _holder;

    [SerializeField]
    private RectTransform _optionHolder;

    [SerializeField]
    private int _maxOptionsShownAtOnce;

    [SerializeField]
    private MenuOption _optionPrefab;

    [SerializeField]
    private GameObject _upArrowIndicator;

    [SerializeField]
    private GameObject _downArrowIndicator;

    private List<MenuOption> options = new List<MenuOption>();
    private int currentIndex;
    private int pageIndex;
    public int CurrentIndex => currentIndex;

    private void Start()
    {
        UpdateItems();

        foreach (MenuOption option in options)
        {
            option.Unhover();
        }
    }

    private void UpdateItems()
    {
        for(int i = 0; i < PartyManager.Instance.Bag.MaxSize(); i++)
        {
            MenuOption option = Instantiate(_optionPrefab, _optionHolder);
            options.Add(option);
            option.gameObject.SetActive(false);
        }
    }

    public void UpdateMenu()
    {
        GetInput();
        HoverOption();
    }

    private void GetInput()
    {
        if (options.Count == 0) return;

        // Get next index
        if (Input.GetKeyDown(KeyCode.DownArrow)) currentIndex++;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) currentIndex--;
        else return; // nothing else needs to happen here

        // Ensure index does not surpass bounds
        if (currentIndex < 0) currentIndex = 0;
        if (currentIndex > PartyManager.Instance.Bag.Count() - 1) currentIndex = PartyManager.Instance.Bag.Count() - 1;

        AdjustMenu();
    }

    private void AdjustMenu()
    {
        int page = (currentIndex / _maxOptionsShownAtOnce);
        int itemCount = PartyManager.Instance.Bag.Count();

        for (int i = 0; i < options.Count; i++)
        {
            if (i < itemCount)
            {
                int itemIndex = page * _maxOptionsShownAtOnce;

                options[i].SetText(PartyManager.Instance.Bag.Items[i].ItemName);

                if (i >= itemIndex && i < itemIndex + _maxOptionsShownAtOnce)
                {
                    options[i].gameObject.SetActive(true);
                }
                else
                {
                    options[i].gameObject.SetActive(false);
                }
            }

            else
            {
                options[i].gameObject.SetActive(false);
            }
        }

        if (options.Count == 0) return;

        if (options[0].gameObject.activeSelf)
        {
            _upArrowIndicator.SetActive(false);
            _downArrowIndicator.SetActive(true);
        }
        else
        {
            _upArrowIndicator.SetActive(true);
        }

        if (options[itemCount - 1].gameObject.activeSelf)
        {
            _downArrowIndicator.SetActive(false);
        }
        else
        {
            _downArrowIndicator.SetActive(true);
        }
    }

    public void SetIndex(int index)
    {
        this.currentIndex = index;
    }

    private void HoverOption()
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (i == currentIndex)
            {
                options[currentIndex].Hover();
                continue;
            }

            options[i].Unhover();
        }
    }

    public void SetVisibility(bool isVisible)
    {
        // Show or hide based on the bool
        if (isVisible) Show();
        else Hide();
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
        AdjustMenu();
    }
}
