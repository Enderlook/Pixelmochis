using EasyButtons;

using System;
using System.Globalization;
using System.Threading.Tasks;

using UnityEngine;

namespace Pixelmochis
{
    // Clase que simula ser la data de la blockchain, pero sus metodos enves de obtener la data
    // desde la blockchain, directamente la toma del valor seteado en el scriptableObject
    [CreateAssetMenu(fileName = "DigimochiLocalData", menuName = "Digimochis/DigimochiLocalData")]
    public class DigimochiDataProxy : ScriptableObject, IDigimochiData
    {
        [SerializeField] private string digimochiName;
        [SerializeField] private string digimochiType;
        [SerializeField, Tooltip("Format: " + dateFormat)] private string lastBathTimeString; // Stored as "ddMMyyyyHHmmss"
        [SerializeField, Tooltip("Format: " + dateFormat)] private string lastMealTimeString; // Stored as "ddMMyyyyHHmmss"
        [SerializeField, Tooltip("Format: " + dateFormat)] private string lastMedicineTimeString; // Stored as "ddMMyyyyHHmmss"

        private readonly CultureInfo provider = CultureInfo.InvariantCulture;
        private const string dateFormat = "ddMMyyyyHHmmss";

        public string GetName() => digimochiName;

        public string GetDigimochiType() => digimochiType;

        public DateTime GetLastBathTime()
        {
            return DateTime.ParseExact(lastBathTimeString, dateFormat, provider);
        }

        public DateTime GetLastMealTime()
        {
            return DateTime.ParseExact(lastMealTimeString, dateFormat, provider);
        }

        public DateTime GetLastMedicineTime()
        {
            return DateTime.ParseExact(lastMedicineTimeString, dateFormat, provider);
        }

        [Button]
        public Task<bool> Bath()
        {
            lastBathTimeString = DateTime.Today.ToString(dateFormat, provider);
            return Task.FromResult(true);
        }

        [Button]
        public Task<bool> Feed()
        {
            lastMealTimeString = DateTime.Today.ToString(dateFormat, provider);
            return Task.FromResult(true);
        }

        [Button]
        public Task<bool> Cure()
        {
            lastMedicineTimeString = DateTime.Today.ToString(dateFormat, provider);
            return Task.FromResult(true);
        }



    }
}