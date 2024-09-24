using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [Header("UI VIEW elements")]
    [SerializeField] protected Selectable defaultSelectable;
    [SerializeField] private bool unpauseOnClose = false;
    [SerializeField] private bool closeOnNewView = true;
    [SerializeField] protected Button backButon;
    [SerializeField] private List<Selectable> selectables;

    protected const string VERTICAL_AXIS = "Vertical";
    protected const float INITIAL_DELAY = 0.5f;
    protected const float REPEAT_DELAY = 0.2f;

    protected int currentIndex = 0;
    protected bool isNavigating = false;
    protected bool isNavigatingFast = false;
    protected float inputHoldTimer = 0;
    private UIView parentView;

    private void OnEnable()
    {
        currentIndex = 0;
    }

    protected virtual void Awake()
    {
        backButon.onClick.AddListener(() => DisableView_OnClick(this));
    }

    protected virtual void Update()
    {
        if (selectables.Count == 0)
        {
            return;
        }

        float verticalInput = Input.GetAxisRaw(VERTICAL_AXIS);

        if (verticalInput != 0)
        {
            if (!isNavigating)
            {
                Navigate(verticalInput);
                isNavigating = true;
                inputHoldTimer = INITIAL_DELAY;
                isNavigatingFast = true;
            }
            else if (isNavigatingFast)
            {
                inputHoldTimer -= Time.deltaTime;
                if (inputHoldTimer <= 0)
                {
                    Navigate(verticalInput);
                    inputHoldTimer = REPEAT_DELAY;
                }
            }
        }
        else
        {
            isNavigating = false;
            isNavigatingFast = false;
            inputHoldTimer = 0;
        }
    }

    public virtual void HandleCancel()
    {
        backButon.onClick.Invoke();
    }

    public void ActiveView_OnClick(UIView viewToActive)
    {
        GUIController.Instance.OnUIViewOpened(viewToActive);
        viewToActive.SetParentView(this);
        viewToActive.ActiveView();
        this.ActiveView(!closeOnNewView);
    }

    private void DisableView_OnClick(UIView viewToDisable)
    {
        viewToDisable.DisableView();
        GUIController.Instance.OnUIViewExited(viewToDisable);
    }

    public void DestroyView_OnClick(UIView viewToDisable)
    {
        viewToDisable.DestroyView();
        GUIController.Instance.OnUIViewExited(viewToDisable);
    }

    public void SetParentView(UIView parentView)
    {
        this.parentView = parentView;
    }

    public void ActiveView(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void ActiveView(Action onBackButtonAction = null)
    {
        if (onBackButtonAction != null)
        {
            backButon.onClick.AddListener(() => onBackButtonAction());
        }

        if (!gameObject.activeSelf)
        {
            this.ActiveView(true);
        }
    }

    public void DisableView()
    {
        if (parentView != null)
        {
            parentView.ActiveView();
        }

        if (unpauseOnClose) 
        {
            GameControlller.Instance.IsPaused = false;
        }
        this.ActiveView(false);
    }

    public void DestroyView()
    {
        if (parentView != null)
        {
            parentView.ActiveView();
        }

        Destroy(this.gameObject);
    }

    public void DisableBackButton()
    {
        backButon.gameObject.SetActive(false);
    }

    public Button GetBackButton()
    {
        return backButon;
    }

    public virtual Selectable GetDefaultSelectable()
    {
        return defaultSelectable;
    }

    private void Navigate(float directionVertical)
    {
        if (directionVertical > 0)
        {
            NavigateUp();
        }
        else if (directionVertical < 0)
        {
            NavigateDown();
        }
    }

    private void NavigateDown()
    {
        currentIndex++;
        if (currentIndex >= selectables.Count)
        {
            currentIndex = 0;
        }
        if (selectables[currentIndex].IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(selectables[currentIndex].gameObject);
        }
        else
        {
            NavigateDown();
        }
    }

    private void NavigateUp()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = selectables.Count - 1;
        }

        if (selectables[currentIndex].IsInteractable())
        {
            GUIController.Instance.SetCurrentSelectedButton(selectables[currentIndex]);
        }
        else
        {
            NavigateUp();
        }
    }
}