using System;
using UnityEngine;
using System.Globalization;

// Clase que simula ser la data de la blockchain, pero sus metodos enves de obtener la data
// desde la blockchain, directamente la toma del valor seteado en el scriptableObject
[CreateAssetMenu(fileName = "DigimochiLocalData", menuName = "Digimochis/DigimochiLocalData")]
public class DigimochiDataProxy : ScriptableObject, IDigimochiData
{
    [SerializeField] private string digimochiName;
    [SerializeField] private string digimochiType;
    [SerializeField] private string lastBathTimeString; // Stored as "ddMMyyyyHHmmss"
    [SerializeField] private string lastMealTimeString; // Stored as "ddMMyyyyHHmmss"
    [SerializeField] private string lastMedicineTimeString; // Stored as "ddMMyyyyHHmmss"

    private readonly CultureInfo provider = CultureInfo.InvariantCulture;
    private const string dateFormat = "ddMMyyyyHHmmss";

    public string GetName() => digimochiName;

    public string GetDigimochiType() => digimochiType;

    public DateTime GetLastBathTime()
    {
        return DateTime.ParseExact(lastBathTimeString, dateFormat, provider);
    }

    public void SetLastBathTime(DateTime dateTime)
    {
        lastBathTimeString = dateTime.ToString(dateFormat);
    }

    public DateTime GetLastMealTime()
    {
        return DateTime.ParseExact(lastMedicineTimeString, dateFormat, provider);
    }

    public DateTime GetLastMedicineTime()
    {
        return DateTime.ParseExact(lastMealTimeString, dateFormat, provider);
    }

    public void SetLastMedicineTime(DateTime dateTime)
    {
        lastMedicineTimeString = dateTime.ToString(dateFormat);
    }
}
