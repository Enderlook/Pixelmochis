using System;
using UnityEngine;

public interface IDigimochiData
{
    string GetName();
    public int GetDigimochiType();
    DateTime GetLastBathTime();
    DateTime GetLastMealTime();
    DateTime GetLastMedicineTime();
    
    // Agregar datos faltantes
}
