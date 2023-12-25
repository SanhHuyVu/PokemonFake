using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;
    List<Text> menuItems;

    public event Action<int> onMenuSelected;
    public event Action onBack;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
        //Debug.Log($"Menu items: {menuItems.Count}");
    }
    public void OpenMenu()
    {
        menu.SetActive(true);
    }
    public void CloseMenu()
    {
        menu.SetActive(false);
        onBack?.Invoke();
    }

    public void OptHover(int opt)
    {
        switch (opt)
        {
            case 0:
                menuItems[opt].color = GlobalSetting.i.HighLightedColor;
                break;
            case 1:
                menuItems[opt].color = GlobalSetting.i.HighLightedColor;
                break;
            case 2:
                menuItems[opt].color = GlobalSetting.i.HighLightedColor;
                break;
        }
    }
    public void OptHoverExit(int opt)
    {
        switch (opt)
        {
            case 0:
                menuItems[opt].color = Color.black;
                break;
            case 1:
                menuItems[opt].color = Color.black;
                break;
            case 2:
                menuItems[opt].color = Color.black;
                break;
        }
    }

    public void OptClicked(int opt)
    {
        onMenuSelected?.Invoke(opt);
    }
}
