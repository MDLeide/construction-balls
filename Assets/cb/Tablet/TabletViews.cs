using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


class TabletViews : MonoBehaviour
{
    public HorizontalLayoutGroup TopBar;
    public TMP_Text TitleText; 
    [Space]
    public TabletButton ButtonPrototype;

    [HideInInspector]
    public GameObject CurrentView;
    public Dictionary<GameObject, TabletButton> ViewList = new Dictionary<GameObject, TabletButton>();

    void Start()
    {
        ViewList = new Dictionary<GameObject, TabletButton>();
    }

    public void AddView(GameObject view, string buttonText, string titleText)
    {
        var button = Instantiate(ButtonPrototype);

        if (button.transform is RectTransform rt)
            rt.SetParent(TopBar.transform);

        button.ButtonText.text = buttonText;
        button.Button.onClick.AddListener(
            () =>
            {
                SetCurrentView(view, titleText);
            });
        
        ViewList.Add(view, button);
    }

    public void RemoveView(GameObject view)
    {
        var button = ViewList[view];
        ViewList.Remove(view);
        Destroy(button.gameObject);

        if (CurrentView == view)
        {
            if (ViewList.Any())
                CurrentView = ViewList.First().Key;
            else
                CurrentView = null;
        }
    }

    public void SetCurrentView(GameObject view, string titleText)
    {
        TitleText.text = titleText;
        if (CurrentView != null)
            CurrentView.SetActive(false);
        CurrentView = view;
        CurrentView.SetActive(true);
    }
}