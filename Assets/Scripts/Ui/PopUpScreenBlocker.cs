using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpScreenBlocker : MonoBehaviour
{

    [SerializeField] private Image ScreenBlocker;
    private List<PopUpView> ActivePopUps;
    private bool InitDone;

    private void Awake()
    {
        if (!InitDone)
        {
            ActivePopUps = new List<PopUpView>();
            ScreenBlocker.enabled = false;
        }
    }

    public void InitBlocker()
    {
        InitDone = true;
        ScreenBlocker.enabled = false;
        ActivePopUps = new List<PopUpView>();
        gameObject.SetActive(true);     
    }


    public void AddPopUpView(PopUpView popUp)
    {
        if(!ActivePopUps.Contains(popUp))
        {
            ActivePopUps.Add(popUp);
        }
        UpdateScreenBlockerState();
    }

    public void RemovePopUpView(PopUpView popUp)
    {
        if (ActivePopUps.Contains(popUp))
        {
            ActivePopUps.Remove(popUp);
        }
        UpdateScreenBlockerState();
    }


    public void UpdateScreenBlockerState()
    {
        ScreenBlocker.enabled = ActivePopUps.Count > 0;
    }

}
