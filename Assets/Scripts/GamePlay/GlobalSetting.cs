using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSetting : MonoBehaviour
{
    [SerializeField] Color highLightedColor;

    public Color HighLightedColor => highLightedColor;

    public static GlobalSetting i { get; set; }

    private void Awake()
    {
        i = this;
    }
}
