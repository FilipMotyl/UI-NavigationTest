using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GettingButtons : MonoBehaviour
{

    /// <summary>
    /// SerializeField - Pokazuje zmienna w edyotrze (PRIVATE rzeczy nie pokażą się w edytorze, chyba że dasz SerializeField atrybut)
    /// PUBLIC zmienne będa widoczne w edytorze bez konieczności dodawania tego atrybutu
    /// ContentWithButtons - Rodzic który w dzieciach ma przyciski
    /// </summary>
    [SerializeField] private GameObject ContentWithButtons;


    // WAŻNE, aby móc używać elemntów UI takich jak BUTTON, TEXT itp. Konieczna jest odpowiedni USING nad skryptem ( using UnityEngine.UI );


    /// <summary>
    /// PUBLICZA Lista z przyciskami, przyciski są przypisywane w edytorze (przeciągne do komponentu, PATRZ w edytorze)
    /// </summary>
    public List<Button> PublicButtonsList;


    /// <summary>
    /// Tablica z przyciskami, na start jest pusta trzeba ją wypełnić.
    /// </summary>
    private Button[] ButtonsArray;

    /// <summary>
    /// Lista z przyciskami, na start jest pusta trzeba ją wypełnić.
    /// </summary>
    private List<Button> ButtonsList;



    /// <summary>
    /// AWAKE - to metoda z Unity wywoływania pierwsza w kolejności w skrypcie, najelpiej używać do inicjalizacji potrzebnych rzeczy
    /// </summary>
    private void Awake()
    {
        IniializeButtons();
    }


    private void IniializeButtons()
    {

        //return w środku metody przerwie jej działanie, przydatne np. gdy operierasz jej działanie na jakimś obiekcie który musi być przypisany w edytorze
        if (ContentWithButtons == null)
        {
            Debug.Log("NIE PRZYPISAŁEŚ RODZICA PRZYCISKÓW W EDYTORZE, PRZERYWAM DZIAŁANIE METODY");
            return;
        }



        // SPOSOBY NA POBIERANIE PRZYCISKÓW DO LISTY

        //SPOSÓB 1

        if (PublicButtonsList != null)
        {
            Debug.Log("PublicButtonsList Count: " + PublicButtonsList.Count);
        }
        else
        {
            Debug.Log("Nie przypisałeś żadnych przecisków do publicznej listy w edyotrze");
        }



        // SPOSÓB 2

        //Jeżeli zmienna jest GameObject, możesz używać różnych moetod typu GetComponent
        //w tym przypadku szukam w dzieciach tego objektu wszyskich obiektów z komponetem Button który zwraca Tablice
        // GetComponentsInChildren -> SZUKA Tylko po głównych dzieciach obiektu, nie wchodzi w dzieci od dzieci
        ButtonsArray = ContentWithButtons.GetComponentsInChildren<Button>();





        // SPOSÓB 3      
        // SZUKA Tylko po głównych dzieciach obiektu, nie wchodzi w dzieci od dzieci
        ButtonsList = new List<Button>(); // Inicjacja listy
        foreach (Transform child in ContentWithButtons.transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                ButtonsList.Add(child.GetComponent<Button>());
            }
        }

    }



}
