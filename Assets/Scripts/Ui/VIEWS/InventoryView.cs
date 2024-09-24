using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryState
{
    SelectSoul, ActionMenu, Popup
}
public class InventoryView : UIView
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const float GRID_SPACE_HEIGHT = 185;


    [Header("Inventory Elements")]
    [SerializeField] private SoulInformation soulItemPlaceholder;
    [SerializeField] private Text description;
    [SerializeField] private Text name;
    [SerializeField] private Image avatar;
    [SerializeField] private Button useButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject viewportContent;

    private RectTransform contentParent;
    private GameObject currentSelectedGameObject;
    private SoulInformation currentSoulInformation;
    private List<SoulInformation> availableSouls = new List<SoulInformation>();
    private InventoryState inventoryState = InventoryState.SelectSoul;
    private int currentScrolledPosition = 0;

    protected override void Awake()
    {
        base.Awake();
        contentParent = (RectTransform)soulItemPlaceholder.transform.parent;
        InitializeInventoryItems();
    }

    private void OnEnable()
    {
        ClearSoulInformation();
    }

    protected override void Update()
    {
        if (GUIController.Instance.GetViewStack().Peek() != this)
        {
            return;
        }

        if (inventoryState == InventoryState.SelectSoul)
        {
            float verticalInput = Input.GetAxisRaw(VERTICAL_AXIS);
            float horizontalInput = Input.GetAxisRaw(HORIZONTAL_AXIS);

            if (verticalInput != 0 || horizontalInput != 0)
            {
                if (!isNavigating)
                {
                    Navigate(verticalInput, horizontalInput);
                    isNavigating = true;
                    inputHoldTimer = INITIAL_DELAY;
                    isNavigatingFast = true;
                }
                else if (isNavigatingFast)
                {
                    inputHoldTimer -= Time.deltaTime;
                    if (inputHoldTimer <= 0)
                    {
                        Navigate(verticalInput, horizontalInput);
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
            if (Input.GetButtonDown("Submit")) 
            {
                GUIController.Instance.SetCurrentSelectedButton(TrySelectActionButton());
            }
        }
        if (inventoryState == InventoryState.Popup)
        {
            GUIController.Instance.SetCurrentSelectedButton(TrySelectSoulButton());
        }
    }

    public override void HandleCancel()
    {
        switch (inventoryState)
        {
            case InventoryState.SelectSoul:
                backButon.onClick.Invoke();
                break;
            case InventoryState.ActionMenu:
                inventoryState = InventoryState.SelectSoul;
                GUIController.Instance.SetCurrentSelectedButton(GetDefaultSelectable());
                break;
            case InventoryState.Popup:
                break;
        }
    }

    public void SetInactiveState()
    {
        inventoryState = InventoryState.Popup;
    }

    public override Selectable GetDefaultSelectable()
    {

        switch (inventoryState)
        {
            case InventoryState.SelectSoul:
                return TrySelectSoulButton();
            case InventoryState.ActionMenu:
                return TrySelectActionButton();
        }
        return null;
    }

    private void Navigate(float verticalInput, float horizontalInput)
    {

        int currentIndex = availableSouls.IndexOf(currentSoulInformation);
        int constraintCount = viewportContent.GetComponent<GridLayoutGroup>().constraintCount;

        if (verticalInput > 0)
        {
            currentIndex -= constraintCount;
        }
        else if (verticalInput < 0)
        {
            currentIndex += constraintCount;
        }

        if (horizontalInput > 0)
        {
            currentIndex += 1;
        }
        else if (horizontalInput < 0)
        {
            currentIndex -= 1;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, availableSouls.Count - 1);
        AdjustScrollViewPostion(currentIndex, constraintCount);

        Button button = availableSouls[currentIndex].GetComponent<Button>();
        GUIController.Instance.SetCurrentSelectedButton(button);
        button.onClick.Invoke();
    }

    private void AdjustScrollViewPostion(int currentIndex, int constraintCount)
    {
        if (currentIndex / constraintCount < currentScrolledPosition)
        {

            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + (GRID_SPACE_HEIGHT / (scrollRect.content.rect.height - scrollRect.viewport.rect.height)));
            currentScrolledPosition--;
        }
        else if (currentIndex / constraintCount >= currentScrolledPosition + 4)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - (GRID_SPACE_HEIGHT / (scrollRect.content.rect.height - scrollRect.viewport.rect.height)));
            currentScrolledPosition++;

        }
    }

    private void InitializeInventoryItems()
    {
        for (int i = 0, j = SoulController.Instance.Souls.Count; i < j; i++)
        {
            SoulInformation newSoul = Instantiate(soulItemPlaceholder.gameObject, contentParent).GetComponent<SoulInformation>();
            newSoul.SetSoulItem(SoulController.Instance.Souls[i], () => SoulItem_OnClick(newSoul));
            availableSouls.Add(newSoul);
        }
        soulItemPlaceholder.gameObject.SetActive(false);
    }

   

    private void ClearSoulInformation()
    {
        description.text = "";
        name.text = "";
        avatar.sprite = null;
        SetupUseButton(false);
        SetupDestroyButton(false);
        currentSelectedGameObject = null;
        currentSoulInformation = null;
    }

    public void SoulItem_OnClick(SoulInformation soulInformation)
    {
        currentSoulInformation = soulInformation;
        currentSelectedGameObject = soulInformation.gameObject;
        SetupSoulInformation(soulInformation.soulItem);
    }

    private void SetupSoulInformation(SoulItem soulItem)
    {
        description.text = soulItem.Description;
        name.text = soulItem.Name;
        avatar.sprite = soulItem.Avatar;
        SetupUseButton(soulItem.CanBeUsed);
        SetupDestroyButton(soulItem.CanBeDestroyed);
    }

    private void CantUseCurrentSoul()
    {
        PopUpInformation popUpInfo = new PopUpInformation
        {
            DisableOnConfirm = true,
            UseOneButton = true,
            Header = "CAN'T USE",
            Message = "THIS SOUL CANNOT BE USED IN THIS LOCALIZATION"
        };
        GUIController.Instance.ShowPopUpMessage(popUpInfo);
    }

    private void UseCurrentSoul(bool canUse)
    {
        if (!canUse)
        {
            CantUseCurrentSoul();
        }
        else
        {
            //USE SOUL
            availableSouls.Remove(currentSoulInformation);
            Destroy(currentSelectedGameObject);
            ClearSoulInformation();            
        }
    }


    private void DestroyCurrentSoul()
    {
        availableSouls.Remove(currentSoulInformation);
        Destroy(currentSelectedGameObject);
        ClearSoulInformation();
    }

    private void SetupUseButton(bool active)
    {
        useButton.onClick.RemoveAllListeners();
        if (active)
        {
            bool isInCorrectLocalization = GameControlller.Instance.IsCurrentLocalization(currentSoulInformation.soulItem.UsableInLocalization);
            PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = isInCorrectLocalization,
                UseOneButton = false,
                Header = "USE ITEM",
                Message = "Are you sure you want to USE: " + currentSoulInformation.soulItem.Name + " ?",
                Confirm_OnClick = () => UseCurrentSoul(isInCorrectLocalization)

            };
            useButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));
            useButton.onClick.AddListener(() => SetInactiveState());
        }
        useButton.gameObject.SetActive(active);
    }

    private void SetupDestroyButton(bool active)
    {
        destroyButton.onClick.RemoveAllListeners();
        if (active)    
        {
            PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = true,
                UseOneButton = false,
                Header = "DESTROY ITEM",
                Message = "Are you sure you want to DESTROY: " + name.text + " ?",
                Confirm_OnClick = () => DestroyCurrentSoul()

            };
            destroyButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));
            destroyButton.onClick.AddListener(() => SetInactiveState());
        }

        destroyButton.gameObject.SetActive(active);
    }

    private Selectable TrySelectActionButton()
    {
        if(useButton.IsActive())
        {
            inventoryState = InventoryState.ActionMenu;
            return useButton;
        }
        else if (destroyButton.IsActive())
        {
            inventoryState = InventoryState.ActionMenu;
            return destroyButton;
        }
        return TrySelectSoulButton();
    }

    private Selectable TrySelectSoulButton()
    {
        if (currentSoulInformation != null)
        {
            inventoryState = InventoryState.SelectSoul;
            Button soulButton = currentSoulInformation.GetComponent<Button>();
            GUIController.Instance.SetCurrentSelectedButton(soulButton);
            return soulButton;
        }
        else
        {
            foreach (Button soulButton in viewportContent.GetComponentsInChildren<Button>())
            {
                if (soulButton.IsActive() == true)
                {
                    inventoryState = InventoryState.SelectSoul;
                    soulButton.onClick.Invoke();
                    scrollRect.verticalNormalizedPosition = 1f;
                    currentScrolledPosition = 0;
                    return soulButton;
                }
            }
        }

        return null;
    }
}