using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorUI : MonoBehaviour
{
    public Text Text;

    public void SetLevel(int currentLevel)
    {
        Text.text = "Piso: " + currentLevel;
    }
}
