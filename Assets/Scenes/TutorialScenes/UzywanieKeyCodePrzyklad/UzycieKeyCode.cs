using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UzycieKeyCode : MonoBehaviour
{
    //USTAWIANE W EDYTORZE
    [SerializeField] private KeyCode FirstButtonKeyCode = KeyCode.None;
    [SerializeField] private KeyCode SecondButtonKeyCode = KeyCode.None;

    //USTAWIANE W EDYTORZE
    [SerializeField] private Button FirstButton;
    [SerializeField] private Button SecondButton;


    private int CountClicks_FirstButton;
    private int CountClicks_SecondButton;



    private void Update()
    {
        ProcessFirstKeyCode();
        ProcessSecondKeyCode();
    }

    private void ProcessFirstKeyCode()
    {
        if (FirstButtonKeyCode == KeyCode.None)
        {
            Debug.Log("Pierwszy KeyCode nie został ustawiony w edytorze");
            return;
        }



        // Input.GetKeyDown sprawdza czy przycisk został naciśnięty w TEJ KLATCE
        //DOdatkowo jest jeszcze:
        //Input.GetKey lub Input.GetButton -> zwracają TRUE cały czas dopóki przycisk jest trzymany
        //Input.GetKeyUp lub Input.GetButtonUP -> zwracają true jeżeli przycisk został puszczony w TEJ KLATCE
        if (Input.GetKeyDown(FirstButtonKeyCode))
        {
            if (FirstButton != null)
            {
                FirstButton.onClick.Invoke();
            }
            else
            {
                Debug.Log("First Button nie został przypisany");
            }
        }
    }

    private void ProcessSecondKeyCode()
    {
        if (SecondButtonKeyCode == KeyCode.None)
        {
            Debug.Log("Drugi KeyCode nie został ustawiony w edytorze");
            return;
        }


        //Input.GetKeyDown sprawdza czy przycisk został naciśnięty w TEJ KLATCE
        //DOdatkowo jest jeszcze:
        //Input.GetKey lub Input.GetButton -> zwracają TRUE cały czas dopóki przycisk jest trzymany
        //Input.GetKeyUp lub Input.GetButtonUP -> zwracają true jeżeli przycisk został puszczony w TEJ KLATCE
        if (Input.GetKeyDown(SecondButtonKeyCode))
        {
            if (SecondButton != null)
            {
                SecondButton.onClick.Invoke();
            }
            else
            {
                Debug.Log("Second Button nie został przypisany");
            }
        }




    }



    //#region przydatne to organizacji kodu
    //Te metody są przypisane w edyotrze do przycisków na eventcie OnClicks
    #region Eventy klikniecia na przyciksach
    public void FirstButton_OnClick(Text ButtonText)
    {
        if (ButtonText == null)
        {
            Debug.Log("Text przucisku nie został przypisany jako argument eventu, zatrzymuje działanie metody");
            return;
        }
        

        CountClicks_FirstButton++;
        ButtonText.text = "Nacisnąłeś " + FirstButtonKeyCode + ": " + CountClicks_FirstButton;
    }

    public void SecondButton_OnClick(Text ButtonText)
    {
        if (ButtonText == null)
        {
            Debug.Log("Text przucisku nie został przypisany jako argument eventu, zatrzymuje działanie metody");
            return;
        }
        CountClicks_SecondButton++;
        ButtonText.text = "Nacisnąłeś " + SecondButtonKeyCode + ": " + CountClicks_SecondButton;
    }

    #endregion
}
