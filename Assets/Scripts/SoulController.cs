using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulController : MonoBehaviour
{
    private static SoulController instance;
    public static SoulController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SoulController>();
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

   


    public List<SoulItem> Souls;
}
