using System;
using UnityEngine;
using System.Globalization;

[CreateAssetMenu(fileName = "DigimochiLocalData", menuName = "Digimochis/DigimochiLocalData")]
public class DigimochiDataProxy : ScriptableObject, IDigimochiData
{
    public string name;
    [SerializeField] private int digimochiType;
    [SerializeField] private string lastBathTimeString; // Stored as "ddMMyyyyHHmmss"
    [SerializeField] private string lastMedicineTimeString; // Stored as "ddMMyyyyHHmmss"

    private readonly CultureInfo provider = CultureInfo.InvariantCulture;
    private const string dateFormat = "ddMMyyyyHHmmss";

    public string GetName() => name;

    public int GetDigimochiType()
    {
        // Implementar averiguar que tipo de digimochi es (que especie).
        return digimochiType;
    }

    public DateTime GetLastBathTime()
    {
        return DateTime.ParseExact(lastBathTimeString, dateFormat, provider);
    }

    public void SetLastBathTime(DateTime dateTime)
    {
        lastBathTimeString = dateTime.ToString(dateFormat);
    }

    public DateTime GetLastMedicineTime()
    {
        return DateTime.ParseExact(lastMedicineTimeString, dateFormat, provider);
    }

    public void SetLastMedicineTime(DateTime dateTime)
    {
        lastMedicineTimeString = dateTime.ToString(dateFormat);
    }
}
