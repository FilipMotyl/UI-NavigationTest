using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{

    #region singleton
    public static GUIController Instance;

    private void Awake()
    {
        disableOnStartObject.SetActive(false);
        Instance = this;
    }
    #endregion

    [HideInInspector] public bool IsCombatStateActive = false;

    [SerializeField] private GameObject disableOnStartObject;
    [SerializeField] private RectTransform viewsParent;
    [SerializeField] private GameObject inGameGUIObject;
    [SerializeField] private PopUpView popUp;
    [SerializeField] private PopUpScreenBlocker screenBlocker;
    [SerializeField] private GameObject uIViewScreenBlocker;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private EnemiesController enemiesController;

    private Stack<UIView> viewStack = new Stack<UIView>();
    private UIView currentUIView = null;

    private const float INITIAL_DELAY = 0.5f;
    private const float REPEAT_DELAY = 0.2f;
    private bool isNavigating = false;
    private bool isNavigatingFast = false;
    private float inputHoldTimer = 0;


    private void Start()
    {
        GameEvents.EnemyKilled += DeactivateCombat;
        if (screenBlocker)
        {
            screenBlocker.InitBlocker();
        }
        SetCurrentSelectedButton(null);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (viewStack.Count == 0)
            {
                if (IsCombatStateActive)
                {
                    enemiesController.ExitCombatState();
                }
                else
                {
                    menuButton.onClick.Invoke();
                }
            }
            else
            {
                viewStack.Peek().HandleCancel();
            }
        }
        HandleGameNavigation();
    }

    public void ShowPopUpMessage(PopUpInformation popUpInfo)
    {
        PopUpView newPopUp = Instantiate(popUp, viewsParent) as PopUpView;
        newPopUp.ActivePopUpView(popUpInfo);
    }
   
    public void ActiveScreenBlocker(bool active, PopUpView popUpView)
    {
        if (active)
        {
            screenBlocker.AddPopUpView(popUpView);
        }
        else
        {
            screenBlocker.RemovePopUpView(popUpView);
        }
    }

    public Selectable GetMenuButton()
    {
        return menuButton;
    }

    public void OnUIViewOpened(UIView OpenedView)
     {
        viewStack.Push(OpenedView);
        currentUIView = OpenedView;
        UpdateScreenBlocker();
        SetCurrentSelectedButton(currentUIView.GetDefaultSelectable());
    }

    public void OnUIViewExited(UIView ExitedView)
     {
        if (viewStack.Contains(ExitedView))
        {
            viewStack.Pop();
        }

        if (viewStack.Count > 0)
        {
            currentUIView = viewStack.Peek();
            SetCurrentSelectedButton(currentUIView.GetDefaultSelectable());
        }
        else
        {
            currentUIView = null;
            SetCurrentSelectedButton(null);
        }

        UpdateScreenBlocker();
    }

    public void SetCurrentSelectedButton(Selectable toSelect)
    {
        if (toSelect != null)
        {
            EventSystem.current.SetSelectedGameObject(toSelect.gameObject);
        }
        else
        {
            if (viewStack.Count == 0)
            {
                EventSystem.current.SetSelectedGameObject(enemiesController.GetFirstAvailiableSelectableEnemyUI().gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }    

    public Stack<UIView> GetViewStack()
    {
        return viewStack;
    }

    #region IN GAME GUI Clicks
    public void InGameGUIButton_OnClick(UIView viewToActive)
    {
        viewToActive.ActiveView(() => ActiveInGameGUI(true));
        OnUIViewOpened(viewToActive);
        ActiveInGameGUI(false);
        GameControlller.Instance.IsPaused = true;
    }
    #endregion

    private void ActiveInGameGUI(bool active)
    {
        inGameGUIObject.SetActive(active);
    }

    private void DeactivateCombat(Enemy enemy)
    {
        IsCombatStateActive = false;
    }

    private void UpdateScreenBlocker()
    {
        if (viewStack.Count > 0)
        {
            uIViewScreenBlocker.SetActive(true);
        }
        else
        {
            uIViewScreenBlocker.SetActive(false);
        }
    }

    private void HandleGameNavigation()
    {
        if (viewStack.Count == 0 && !IsCombatStateActive)
        {

            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput != 0f) 
            {
                if (!isNavigating)
                {
                    Navigate(horizontalInput);
                    isNavigating = true;
                    inputHoldTimer = INITIAL_DELAY;
                    isNavigatingFast = true;
                }
                else if (isNavigatingFast)
                {
                    inputHoldTimer -= Time.deltaTime;
                    if (inputHoldTimer <= 0)
                    {
                        Navigate(horizontalInput);
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
    }
    

    private void Navigate(float horizontalInput)
    {

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (EventSystem.current.currentSelectedGameObject == inventoryButton.gameObject)
            {
                SetCurrentSelectedButton(menuButton);
                return;
            }
            else if (EventSystem.current.currentSelectedGameObject == menuButton.gameObject)
            {
                SetCurrentSelectedButton(enemiesController.GetFirstAvailiableSelectableEnemyUI());
                return;
            }

            Button button = enemiesController.TryFindButtonToTheRight(EventSystem.current.currentSelectedGameObject.transform.position.x);
            if (button != null)
            {
               SetCurrentSelectedButton(button);
            }
            else
            {
                SetCurrentSelectedButton(inventoryButton);
            }
        }
        if (Input.GetAxisRaw("Horizontal") < 0)
        {

            if (EventSystem.current.currentSelectedGameObject == menuButton.gameObject)
            {
                SetCurrentSelectedButton(inventoryButton);
                return;
            }

            Button button = enemiesController.TryFindButtonToTheLeft(EventSystem.current.currentSelectedGameObject.transform.position.x);
            if (button != null)
            {
                SetCurrentSelectedButton(button);
            }
            else
            {
                SetCurrentSelectedButton(menuButton);
            }
        }
    }
}