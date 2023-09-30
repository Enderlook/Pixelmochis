using System;
using UnityEngine;

public interface IDigimochiData
{
    string GetName();
    public string GetDigimochiType();
    DateTime GetLastBathTime();
    DateTime GetLastMealTime();
    DateTime GetLastMedicineTime();
}
