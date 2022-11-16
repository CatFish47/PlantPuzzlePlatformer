using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxWater(float waterLevel)
    {
        slider.maxValue = waterLevel;
        slider.value = waterLevel;
    }

    public void SetWater(float waterLevel)
    {
        slider.value = waterLevel;
    }
}
