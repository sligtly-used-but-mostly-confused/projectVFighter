using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatisticsFieldUIController : MonoBehaviour {

    public TextMeshProUGUI FieldNameText;
    public TextMeshProUGUI AmountText;
    
    public void DisplayField(string name, string value)
    {
        FieldNameText.text = name;
        AmountText.text = value;
    }
}
