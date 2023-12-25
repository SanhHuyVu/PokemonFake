using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIconIndicator : MonoBehaviour
{
    [SerializeField] Image indicator;
    [SerializeField] Image icon;
    public void SetFoeOrAlly(Sprite img, bool AllyOrNot)
    {
        if (AllyOrNot == true)
        {
            icon.sprite = img;
            indicator.color = Color.blue;
        }
        else
        {
            icon.sprite = img;
            indicator.color = Color.red;
        }
    }
}
