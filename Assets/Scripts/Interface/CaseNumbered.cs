using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseNumbered : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI neededValueText;
    public TextMeshProUGUI currentValueText;

    public void InitializeImageAndValues(Sprite sprite, int neededValue, int currentValue)
    {

        image.sprite = sprite;
        neededValueText.text = neededValue.ToString();
        currentValueText.text = currentValue.ToString();
    }

    public void UpdateCurrentValue(int currentValue)
    {
        currentValueText.text = currentValue.ToString();
    }

    public void Reset()
    {
        neededValueText.text = "";
        currentValueText.text = "";
        image.sprite = null;
    }
}
