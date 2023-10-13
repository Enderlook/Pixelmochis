using System;
using System.Threading.Tasks;

public interface IDigimochiData
{
    string GetName();
    public string GetDigimochiType();
    DateTime GetLastBathTime();
    DateTime GetLastMealTime();
    DateTime GetLastMedicineTime();
    Task<bool> Bath();
    Task<bool> Feed();
    Task<bool> Cure();
}
