using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GettingButtonsAndNavigation : MonoBehaviour
{
    /// <summary>
    /// W TEJ KLASIE OBSŁUGUJE NAWIGACJE PO PRZYCIKSACH KODEM,
    /// czyli PORUSZANIE się po nich i SELECTOWANIE ich. Na konsolach aby mieć
    /// w pełni kontrolę nad tym co się dzieje w UI jest to częstro konieczne 
    /// 
    /// JEST TO TYLKO PRZYKŁAD, ROZWIĄZANIA TAKIE MOGĄ BYĆ WYKONANKE NA RÓŻNE SPOSOBY jak to w programnowaniu często bywa! :)
    /// </summary>


    //MAMY TUTAJ TYLKO PORUSZANIE GÓRA DÓŁ, WIĘC UŻYWAM TYLKO VERTICAL KTÓRY WPISUJE w edytorze
    // Więcej o tym czemu to string w metodzie UPDATE ODPISANE
    [SerializeField] private string VerticalAxis = "Vertical";

    [SerializeField] private GameObject ContentWithButtons;



   
    private bool DelayInputNavigation = false;



    private bool IsInitialized = false;

    private int CurrentSelectedIndex = -1;

    private List<Button> ButtonsList;


    private void Awake()
    {
        IniializeButtons();
    }



    // START może być typu VOID albo IEnumerator(kurotyna)
    // UŻYWAM JEJ PO TO. ABY PO INICJALIZACJI CAŁEGO UI I OGARNIĘCIU SIĘ UNITY USTAWIĆ PIERWSZY PRZYCISK JAKO WYBRANY
    private IEnumerator Start()
    {
        yield return null; // WSTRZYMUJE DZIAŁANIE METODY NA JEDNĄ KLATĘ
        IsInitialized = true;

        //ustawiam żeby wybrało pierwszy przycisk z listy 
        CurrentSelectedIndex = 0;
        SetCurrentSelectedGameObject();
    }


    private void Update()
    {
        //WSTRZUMUJE DZIALANIE UPDATE DOPÓKI SKRYPT NIE BEDZIE ZAINICJALIZOWANY
        if (!IsInitialized)
            return;

        //INPUT MOZE PRZYJMOWAĆ KeyCody (tak jak w poprzednim przykładzie) lub AXISY które definicuje się w project settings INPUTS
        //Do danego AXIS można przypisać POZYTYWNY PRZYCISK i NEGATYWNY PRZYCISK (Pozytynwy czyli 1, negatywny czyli -1) 
        // "Vertical" jest zdefiniowany by default przez Unity i ma przypisane STRZAŁKA W GÓRE jako pozytywny i STRZAŁKE W DÓŁ jako negatywny
        // INPUT.GetAxis co klatkę zwraca wartość jeżeli dany axis jest trzymany,
        //w tym celu jest użyty jeszcze DelayInputNavigation, aby zablkokować tymczasowo przeskakiwanie po nawigacji
        //(bez tego po naciśnieciu strzałki w dół skrypt będzie co klatkę chciał lecieć po przyciskach w danym kierunku
        // JEZELI CHCESZ ZOBACZYĆ O co mi chodzi USUŃ  - DelayInputNavigation - z warunku i zobacz na PlAY modzie co się dzieje z przyciskami
        Debug.Log(Input.GetAxis(VerticalAxis));
        if (Input.GetAxis(VerticalAxis) > 0 && !DelayInputNavigation)
        {
            NavigateUp();
        }
        else if (Input.GetAxis(VerticalAxis) < 0 && !DelayInputNavigation)
        {
            NavigateDown();
        }
    }

    private void NavigateUp()
    {
        CurrentSelectedIndex--;
        SetCurrentSelectedGameObject(true);
        


        //PRZYKŁAD UŻYCIA KUROTYNY
        StartCoroutine(SetupDelayInput());
    }

    private void NavigateDown()
    {
        CurrentSelectedIndex++;
        SetCurrentSelectedGameObject();


        //PRZYKŁAD UŻYCIA KUROTYNY
        StartCoroutine(SetupDelayInput());
    }



    //BLOKUJE NAWIGACJE na 0.5 sekundy
    private IEnumerator SetupDelayInput()
    {
        DelayInputNavigation = true;
        yield return new WaitForSecondsRealtime(0.5f);
        DelayInputNavigation = false;
    }



    /// <summary>
    ///  DO działania tej metody konieczne jest aby na scenie był EVENT_SYSTEM dodany, Obiekt od Unity który kontroluje UI
    ///  Metoda ta kontroluje granice listy, i wybiera przycisk na podstawie CurrentSelectedIndex
    ///  EventSystem SELECTUJE przycisk po podaniu mu GameObject dantego Przycisku (ButtonsList[CurrentSelectedIndex].gameObject) 
    /// </summary>
    private void SetCurrentSelectedGameObject(bool isGoingUp = false)
    {

        //KONTROLOWANIE ZAKRESU LISTY
        CheckIfIndexOufOfList();


        //SPRAWDZAM CZY PRZYCISK NA KTÓRY CHCE USTWWIĆ SELECT jest INTERAKTYWNY, JEZELI NIE SZUKAM NASTEPNEGO
        while (!ButtonsList[CurrentSelectedIndex].interactable)
        {
            if (isGoingUp)
            {
                CurrentSelectedIndex--;
            }
            else
            {
                CurrentSelectedIndex++;
            }

            CheckIfIndexOufOfList();
        }

        EventSystem.current.SetSelectedGameObject(ButtonsList[CurrentSelectedIndex].gameObject);
    }


    private void CheckIfIndexOufOfList()
    {
        if (CurrentSelectedIndex < 0)
            CurrentSelectedIndex = ButtonsList.Count - 1;
        else if (CurrentSelectedIndex >= ButtonsList.Count)
            CurrentSelectedIndex = 0;
    }

    private void IniializeButtons()
    {

        if (ContentWithButtons == null)
        {
            Debug.Log("NIE PRZYPISAŁEŚ RODZICA PRZYCISKÓW W EDYTORZE, PRZERYWAM DZIAŁANIE METODY");
            return;
        }

        ButtonsList = new List<Button>(); 
        foreach (Transform child in ContentWithButtons.transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                ButtonsList.Add(child.GetComponent<Button>());
            }
        }

    }
}
