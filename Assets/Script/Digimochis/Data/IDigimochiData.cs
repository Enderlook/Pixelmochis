using System;

public interface IDigimochiData
{
    string GetName();
    public string GetDigimochiType();
    DateTime GetLastBathTime();
    DateTime GetLastMealTime();
    DateTime GetLastMedicineTime();
    void Bath();
    void Feed();
    void Cure();
}
