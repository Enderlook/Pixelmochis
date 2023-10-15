using System;
using System.Threading.Tasks;

namespace Pixelmotchis
{
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
}