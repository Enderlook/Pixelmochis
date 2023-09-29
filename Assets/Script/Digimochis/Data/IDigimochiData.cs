using System;
using UnityEngine;

public interface IDigimochiData
{
    string GetName();
    public int GetDigimochiType();
    DateTime GetLastBathTime();
    DateTime GetLastMedicineTime();
    
    // Agregar datos faltantes
}
